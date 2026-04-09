(function () {
    const form = document.getElementById('ideaSubmitForm');
    if (!form) {
        return;
    }

    const maxFiles = 5;
    const maxFileSize = 10 * 1024 * 1024;
    const allowedExtensions = new Set(['.pdf', '.doc', '.docx', '.txt', '.png', '.jpg', '.jpeg']);
    const allowedMimeTypes = new Set([
        'application/pdf',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        'text/plain',
        'image/png',
        'image/jpeg'
    ]);

    const elements = {
        title: document.getElementById('title'),
        description: document.getElementById('description'),
        departmentId: document.getElementById('departmentId'),
        categories: document.getElementById('categories'),
        tagsInput: document.getElementById('tagsInput'),
        files: document.getElementById('files'),
        anonymous: document.getElementById('anonymous'),
        agreeTerms: document.getElementById('agreeTerms'),
        fileList: document.getElementById('fileList'),
        uploadProgressWrapper: document.getElementById('uploadProgressWrapper'),
        uploadProgressBar: document.getElementById('uploadProgressBar'),
        submitButton: document.getElementById('submitButton'),
        resetButton: document.getElementById('resetButton'),
        formStatus: document.getElementById('formStatus'),
        closureAlert: document.getElementById('closureAlert'),
        tagsPreview: document.getElementById('tagsPreview')
    };

    const errorElements = {
        title: document.getElementById('titleError'),
        description: document.getElementById('descriptionError'),
        departmentId: document.getElementById('departmentError'),
        categories: document.getElementById('categoriesError'),
        tags: document.getElementById('tagsError'),
        files: document.getElementById('filesError'),
        agreeTerms: document.getElementById('agreeTermsError')
    };

    let selectedFiles = [];
    let isClosed = false;

    function setStatus(type, message, ideaId) {
        const cssClass = type === 'success'
            ? 'alert alert-success'
            : type === 'error'
                ? 'alert alert-danger'
                : type === 'info'
                    ? 'alert alert-info'
                    : 'alert alert-secondary';

        let content = message;
        if (type === 'success' && ideaId) {
            content += ` <div class="mt-2"><strong>Idea ID:</strong> ${ideaId}</div>`;
            content += ` <div class="mt-2"><a class="btn btn-sm btn-outline-success" href="/Staff/Home/Views/${ideaId}">View details</a></div>`;
        }

        elements.formStatus.className = cssClass;
        elements.formStatus.innerHTML = content;
    }

    function clearStatus() {
        elements.formStatus.className = '';
        elements.formStatus.textContent = '';
    }

    function setFieldError(fieldName, message) {
        const el = errorElements[fieldName];
        if (!el) {
            return;
        }

        el.textContent = message;
    }

    function clearFieldErrors() {
        Object.values(errorElements).forEach((el) => {
            if (el) {
                el.textContent = '';
            }
        });
    }

    function focusFirstError(errors) {
        const order = ['title', 'description', 'departmentId', 'categories', 'tags', 'files', 'agreeTerms'];
        for (const field of order) {
            if (errors[field]) {
                const input = elements[field] || (field === 'tags' ? elements.tagsInput : null);
                if (input && typeof input.focus === 'function') {
                    input.focus();
                }
                break;
            }
        }
    }

    function getTags() {
        const raw = (elements.tagsInput.value || '');
        const tags = raw
            .split(',')
            .map((t) => t.trim())
            .filter((t) => t.length > 0);

        return tags.slice(0, 10);
    }

    function renderTagsPreview() {
        const allTags = (elements.tagsInput.value || '')
            .split(',')
            .map((t) => t.trim())
            .filter((t) => t.length > 0);

        const tags = allTags.slice(0, 10);
        elements.tagsPreview.innerHTML = tags.map((tag) => `<span class="badge bg-secondary">${tag}</span>`).join('');

        if (allTags.length > 10) {
            setFieldError('tags', 'Only the first 10 tags will be used.');
        } else {
            setFieldError('tags', '');
        }
    }

    function humanSize(bytes) {
        return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
    }

    function renderFileList() {
        elements.fileList.innerHTML = '';

        selectedFiles.forEach((file, index) => {
            const li = document.createElement('li');
            li.className = 'list-group-item d-flex justify-content-between align-items-center gap-2';
            li.innerHTML = `
                <span class="text-break">${file.name} <small class="text-muted">(${humanSize(file.size)})</small></span>
                <button type="button" class="btn btn-sm btn-outline-danger" data-remove-index="${index}" aria-label="Remove file ${file.name}">Remove</button>
            `;
            elements.fileList.appendChild(li);
        });
    }

    function getFileError(file) {
        const fileName = file.name || '';
        const extensionIndex = fileName.lastIndexOf('.');
        const extension = extensionIndex >= 0 ? fileName.substring(extensionIndex).toLowerCase() : '';

        if (!allowedExtensions.has(extension)) {
            return `File ${fileName}: invalid format (${extension || 'unknown'}).`;
        }

        if (file.size > maxFileSize) {
            return `File ${fileName}: exceeds 10MB.`;
        }

        if (file.type && !allowedMimeTypes.has(file.type)) {
            return `File ${fileName}: invalid MIME type (${file.type}).`;
        }

        return '';
    }

    function validateForm() {
        clearFieldErrors();
        clearStatus();

        const errors = {};

        const title = (elements.title.value || '').trim();
        const description = (elements.description.value || '').trim();

        if (!title) {
            errors.title = 'Title is required.';
        } else if (title.length > 150) {
            errors.title = 'Maximum 150 characters.';
        }

        if (!description) {
            errors.description = 'Description is required.';
        } else if (description.length > 5000) {
            errors.description = 'Description must be 5000 characters or less.';
        }

        if (!elements.departmentId.value) {
            errors.departmentId = 'Please select a department.';
        }

        if (!elements.agreeTerms.checked) {
            errors.agreeTerms = 'You must agree to the terms.';
        }

        if (selectedFiles.length > maxFiles) {
            errors.files = `You can upload up to ${maxFiles} files.`;
        } else {
            const fileErrors = selectedFiles.map(getFileError).filter(Boolean);
            if (fileErrors.length > 0) {
                errors.files = fileErrors.join(' ');
            }
        }

        Object.entries(errors).forEach(([field, message]) => setFieldError(field, message));

        if (Object.keys(errors).length > 0) {
            focusFirstError(errors);
            return false;
        }

        return true;
    }

    function setUploadingState(uploading) {
        elements.submitButton.disabled = uploading || isClosed;
        elements.resetButton.disabled = uploading || isClosed;
        Array.from(form.elements).forEach((el) => {
            if (el.id !== 'submitButton' && el.id !== 'resetButton') {
                el.disabled = uploading || isClosed;
            }
        });

        if (uploading) {
            elements.uploadProgressWrapper.classList.remove('d-none');
            elements.uploadProgressBar.style.width = '0%';
            elements.uploadProgressBar.textContent = '0%';
        } else {
            elements.uploadProgressWrapper.classList.add('d-none');
            elements.uploadProgressBar.style.width = '0%';
            elements.uploadProgressBar.textContent = '0%';
        }
    }

    async function loadLookups() {
        try {
            const [departmentsRes, categoriesRes] = await Promise.all([
                fetch('/api/departments', { credentials: 'include' }),
                fetch('/api/categories', { credentials: 'include' })
            ]);

            if (departmentsRes.ok) {
                const departments = await departmentsRes.json();
                elements.departmentId.innerHTML = '<option value="">-- Select department --</option>';
                departments.forEach((d) => {
                    const option = document.createElement('option');
                    option.value = d.id;
                    option.textContent = d.name;
                    elements.departmentId.appendChild(option);
                });
            } else {
                elements.departmentId.innerHTML = '<option value="">Unable to load departments</option>';
            }

            if (categoriesRes.ok) {
                const categories = await categoriesRes.json();
                elements.categories.innerHTML = '';
                categories.forEach((c) => {
                    const option = document.createElement('option');
                    option.value = c.id;
                    option.textContent = c.name;
                    elements.categories.appendChild(option);
                });
            } else {
                elements.categories.innerHTML = '<option value="">Unable to load categories</option>';
            }
        } catch {
            elements.departmentId.innerHTML = '<option value="">Unable to load departments</option>';
            elements.categories.innerHTML = '<option value="">Unable to load categories</option>';
        }
    }

    async function checkClosureDate() {
        try {
            const res = await fetch('/api/config', { credentials: 'include' });
            if (!res.ok) {
                return;
            }

            const config = await res.json();
            const closureValue = config?.newIdeasClosureDate;
            if (!closureValue) {
                return;
            }

            const closureDate = new Date(closureValue);
            if (Number.isNaN(closureDate.getTime())) {
                return;
            }

            const now = new Date();
            if (now > closureDate) {
                isClosed = true;
                setUploadingState(false);
                elements.closureAlert.textContent = 'The idea submission deadline has passed. The form is locked.';
                elements.closureAlert.classList.remove('d-none');
                setStatus('error', 'New idea submissions are closed.');
            }
        } catch {
            // ignore config error
        }
    }

    function handleServerErrors(payload) {
        if (!payload || !Array.isArray(payload.errors)) {
            return false;
        }

        const mapped = {};
        payload.errors.forEach((item) => {
            if (!item || !item.field) {
                return;
            }

            const field = String(item.field).toLowerCase();
            const message = item.message || 'Invalid data';

            if (field === 'title') mapped.title = message;
            else if (field === 'description') mapped.description = message;
            else if (field === 'departmentid') mapped.departmentId = message;
            else if (field === 'categories') mapped.categories = message;
            else if (field === 'tags') mapped.tags = message;
            else if (field === 'files' || field === 'files[]') mapped.files = message;
            else if (field === 'agreeterms') mapped.agreeTerms = message;
        });

        Object.entries(mapped).forEach(([field, message]) => setFieldError(field, message));
        if (Object.keys(mapped).length > 0) {
            focusFirstError(mapped);
            return true;
        }

        return false;
    }

    form.addEventListener('submit', (event) => {
        event.preventDefault();

        if (isClosed) {
            setStatus('error', 'New idea submissions are closed.');
            return;
        }

        if (!validateForm()) {
            setStatus('error', 'Please review and fix the highlighted errors.');
            return;
        }

        const formData = new FormData();
        const selectedCategoryIds = Array.from(elements.categories.selectedOptions).map((opt) => opt.value);
        const tags = getTags();

        formData.append('title', elements.title.value.trim());
        formData.append('description', elements.description.value.trim());
        formData.append('departmentId', elements.departmentId.value);
        formData.append('categories', JSON.stringify(selectedCategoryIds));
        formData.append('tags', JSON.stringify(tags));
        formData.append('anonymous', elements.anonymous.checked ? 'true' : 'false');
        formData.append('agreeTerms', elements.agreeTerms.checked ? 'true' : 'false');
        selectedFiles.forEach((file) => formData.append('files[]', file, file.name));

        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/api/ideas', true);
        xhr.withCredentials = true;

        setUploadingState(true);
        setStatus('info', 'Uploading and submitting data...');

        xhr.upload.onprogress = (e) => {
            if (!e.lengthComputable) {
                return;
            }

            const percent = Math.round((e.loaded / e.total) * 100);
            elements.uploadProgressBar.style.width = `${percent}%`;
            elements.uploadProgressBar.textContent = `${percent}%`;
        };

        xhr.onload = () => {
            setUploadingState(false);

            let payload = null;
            try {
                payload = xhr.responseText ? JSON.parse(xhr.responseText) : null;
            } catch {
                payload = null;
            }

            if (xhr.status === 201) {
                const ideaId = payload?.id;
                setStatus('success', 'Idea submitted successfully.', ideaId);
                form.reset();
                selectedFiles = [];
                renderFileList();
                renderTagsPreview();
                return;
            }

            if (xhr.status === 400) {
                const mapped = handleServerErrors(payload);
                setStatus('error', mapped ? 'Please fix the highlighted errors.' : 'Invalid data.');
                return;
            }

            if (xhr.status === 401 || xhr.status === 403) {
                setStatus('error', 'You are not authorized to perform this action. Please sign in again.');
                return;
            }

            setStatus('error', 'A system error occurred. Please try again later.');
        };

        xhr.onerror = () => {
            setUploadingState(false);
            setStatus('error', 'Unable to connect to the server.');
        };

        xhr.send(formData);
    });

    elements.files.addEventListener('change', () => {
        const incoming = Array.from(elements.files.files || []);
        if (incoming.length === 0) {
            return;
        }

        const merged = selectedFiles.concat(incoming);
        const fileErrors = [];

        if (merged.length > maxFiles) {
            fileErrors.push(`You can upload up to ${maxFiles} files.`);
        }

        const validFiles = [];
        merged.slice(0, maxFiles).forEach((file) => {
            const err = getFileError(file);
            if (err) {
                fileErrors.push(err);
                return;
            }

            validFiles.push(file);
        });

        selectedFiles = validFiles;
        renderFileList();

        setFieldError('files', fileErrors.join(' '));
        elements.files.value = '';
    });

    elements.fileList.addEventListener('click', (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        const removeButton = target.closest('[data-remove-index]');
        if (!(removeButton instanceof HTMLElement)) {
            return;
        }

        const index = Number(removeButton.getAttribute('data-remove-index'));
        if (Number.isNaN(index)) {
            return;
        }

        selectedFiles.splice(index, 1);
        renderFileList();
        setFieldError('files', '');
    });

    elements.tagsInput.addEventListener('input', renderTagsPreview);

    form.addEventListener('reset', () => {
        window.setTimeout(() => {
            clearFieldErrors();
            clearStatus();
            selectedFiles = [];
            renderFileList();
            renderTagsPreview();
        }, 0);
    });

    loadLookups();
    checkClosureDate();
})();
