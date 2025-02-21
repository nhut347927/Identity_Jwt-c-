using demo1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using demo1.ViewModels;

namespace demo1.Controllers
{
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PermissionController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: Permission/ListUsers
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users;
            return View();
        }

        // GET: Permission/AssignPermissionsToRoles
        public async Task<IActionResult> AssignPermissionsToRoles(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var userRolePermissions = await _context.RolePermissions
                    .Where(rp => rp.UserId == user.Id)
                    .ToListAsync();

                var model = new UpdateUserPermissionsViewModel
                {
                    UserName = userName,
                    Permissions = roles.ToDictionary(
                        role => role.Id,
                        role => userRolePermissions.FirstOrDefault(rp => rp.RoleId == role.Id) ?? new RolePermission()
                    )
                };

                ViewBag.Roles = roles;
                return View(model);
            }
            return RedirectToAction(nameof(ListUsers));
        }

        // POST: Permission/UpdateUserPermissions
       [HttpPost]
public async Task<IActionResult> UpdateUserPermissions(UpdateUserPermissionsViewModel model)
{
    var user = await _userManager.FindByNameAsync(model.UserName);
    if (user == null)
    {
        return RedirectToAction(nameof(ListUsers));
    }

    foreach (var permissionEntry in model.Permissions)
    {
        var roleId = permissionEntry.Key;
        var newPermissions = permissionEntry.Value;

        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.UserId == user.Id && rp.RoleId == roleId);

        if (rolePermission == null)
        {
            rolePermission = new RolePermission
            {
                UserId = user.Id,
                RoleId = roleId,
                CanView = newPermissions.CanView,
                CanInsert = newPermissions.CanInsert,
                CanUpdate = newPermissions.CanUpdate,
                CanDelete = newPermissions.CanDelete
            };
            _context.RolePermissions.Add(rolePermission);
        }
        else
        {
            rolePermission.CanView = newPermissions.CanView;
            rolePermission.CanInsert = newPermissions.CanInsert;
            rolePermission.CanUpdate = newPermissions.CanUpdate;
            rolePermission.CanDelete = newPermissions.CanDelete;
            _context.RolePermissions.Update(rolePermission);
        }
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(ListUsers));
}

    }
}