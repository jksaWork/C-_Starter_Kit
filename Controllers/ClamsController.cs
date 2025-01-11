using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentiyFreamwork.data;
using IdentiyFreamwork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyFreamwork.Controllers
{
    public class ClamsController : Controller
    {
        public readonly UserManager<IdentityUser> _userManager;
        public ClamsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Edit(string userId)
        {
            IdentityUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var model = new EditClamsViewModel
            {
                UserId = userId
            };

            var existClaimns = await _userManager.GetClaimsAsync(user);

          foreach (Claim claim in ClamsRepository.ClamsList)
            {
                bool isSelected = existClaimns.Any(c => c.Type == claim.Type);
                Console.WriteLine($"Claim Type: {claim.Type}, IsSelected: {isSelected}");
                var userClaim = new UserClaims
                {
                    ClaimType = claim.Type,
                    IsSelected = isSelected
                };
                model.UserClaims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(EditClamsViewModel model)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Get existing claims for the user
            var existingClaims = await _userManager.GetClaimsAsync(user);

            // Remove all current claims
            var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, existingClaims);
            if (!removeClaimsResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove existing claims.");
                return View(model);
            }

            // Add selected claims to the user
            var selectedClaims = model.UserClaims
                .Where(c => c.IsSelected)
                .Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()));

            var addClaimsResult = await _userManager.AddClaimsAsync(user, selectedClaims);
            if (!addClaimsResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add selected claims.");
                return View(model);
            }

            // Display success message and redirect
            TempData["Success"] = "Claims updated successfully";
            return RedirectToAction(nameof(Index), "User"); // Replace 'Index' with your desired action
        }
    }
}