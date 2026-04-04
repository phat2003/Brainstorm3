using Brainstorm.DataAccess.Repository.IRepository;
using Brainstorm.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brainstorm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;

        // Constructor: Nhận ApplicationDbContext để thao tác với CSDL
        public TopicController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Action hiển thị danh sách Topic
        public IActionResult Index()
        {
            IEnumerable<Topic> objTopicList = _unitOfWork.Topic.GetAll();
            return View(objTopicList);
        }

        // 2. Action hiển thị Form thêm mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. Action xử lý dữ liệu thêm mới (POST)
        [HttpPost]
        public IActionResult Create(Topic obj)
        {
            if (obj.ClosureDate > obj.FinalClosureDate)
            {
                ModelState.AddModelError("FinalClosureDate", "Hạn đóng bình luận không được sớm hơn Hạn nộp ý tưởng.");
            }
            // Kiểm tra tính hợp lệ của dữ liệu
            if (ModelState.IsValid)
            {
                _unitOfWork.Topic.Add(obj); // Thêm vào danh sách chờ
                _unitOfWork.Save();   // Lưu vào SQL Server
                return RedirectToAction("Index"); // Quay về trang danh sách
            }
            return View(obj); // Nếu lỗi (ví dụ chưa nhập ngày), hiện lại form
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)//nếu id null hoặc = 0 thì trả về notfound (không tìm thấy).
            {
                return NotFound();
            }
            //var categoryfromDb = _db.Categories.Find(id);//tạo biến var categoryfromDb và cho = find id để tìm tới id của nó trong database
            var topicfromDbFirst = _unitOfWork.Topic.GetFirstOrDefault(u => u.Id == id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (topicfromDbFirst == null)//ở đây do đã set categoryfromDb = id nên id null hoặc = 0 thì categoryfromDb cũng null và trả về notfound giống id.
            {
                return NotFound();
            }
            return View(topicfromDbFirst);//trả về view dù cho có đáp ứng 2 điều kiện trên hay không.
        }

        //post
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Edit(Topic obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The Name must not same displayorder");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Topic.Update(obj);
                _unitOfWork.Save();
                TempData["Sucess"] = "Category Edit sucessfully";
                return RedirectToAction("index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryfromDb = _db.Categories.Find(id);
            var topicfromDbFirst = _unitOfWork.Topic.GetFirstOrDefault(u => u.Id == id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (topicfromDbFirst == null)
            {
                return NotFound();
            }
            return View(topicfromDbFirst);

        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Topic.GetFirstOrDefault(u => u.Id == id);
            //var categoryfromDbFirst = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryfromDbsingle = _db.Categories.SingleOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Topic.Remove(obj);
                _unitOfWork.Save();
                TempData["Sucess"] = "Category Delete sucessfully";
                return RedirectToAction("index");
            }
            return View(obj);

        }
    }
}
