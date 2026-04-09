// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Brainstorm.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Brainstorm.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Full name")]
            [StringLength(100)]
            public string FullName { get; set; }

            [Display(Name = "Avatar URL")]
            [Url(ErrorMessage = "Avatar must be a valid URL")]
            [StringLength(500)]
            public string AvatarUrl { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = claims.FirstOrDefault(c => c.Type == SD.Claim_FullName)?.Value,
                AvatarUrl = claims.FirstOrDefault(c => c.Type == SD.Claim_AvatarUrl)?.Value
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error while updating phone number.";
                    return RedirectToPage();
                }
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var currentFullName = claims.FirstOrDefault(c => c.Type == SD.Claim_FullName);
            var currentAvatar = claims.FirstOrDefault(c => c.Type == SD.Claim_AvatarUrl);

            if (string.IsNullOrWhiteSpace(Input.FullName))
            {
                if (currentFullName != null)
                {
                    await _userManager.RemoveClaimAsync(user, currentFullName);
                }
            }
            else if (currentFullName == null)
            {
                await _userManager.AddClaimAsync(user, new Claim(SD.Claim_FullName, Input.FullName.Trim()));
            }
            else if (currentFullName.Value != Input.FullName.Trim())
            {
                await _userManager.ReplaceClaimAsync(user, currentFullName, new Claim(SD.Claim_FullName, Input.FullName.Trim()));
            }

            if (string.IsNullOrWhiteSpace(Input.AvatarUrl))
            {
                if (currentAvatar != null)
                {
                    await _userManager.RemoveClaimAsync(user, currentAvatar);
                }
            }
            else if (currentAvatar == null)
            {
                await _userManager.AddClaimAsync(user, new Claim(SD.Claim_AvatarUrl, Input.AvatarUrl.Trim()));
            }
            else if (currentAvatar.Value != Input.AvatarUrl.Trim())
            {
                await _userManager.ReplaceClaimAsync(user, currentAvatar, new Claim(SD.Claim_AvatarUrl, Input.AvatarUrl.Trim()));
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated successfully.";
            return RedirectToPage();
        }
    }
}
