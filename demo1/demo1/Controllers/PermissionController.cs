using demo1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        // GET: Permission/AddPermission
        public IActionResult AddPermission()
        {
            return View();
        }

        // POST: Permission/AddPermission
        [HttpPost]
        public async Task<IActionResult> AddPermission(string permissionName)
        {
            if (!string.IsNullOrEmpty(permissionName))
            {
                var permission = new Permission { Name = permissionName };
                if (_context.Permissions != null)
                {
                    _context.Permissions.Add(permission);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddPermission));
            }
            return View();
        }

        // GET: Role/AddRole
        public IActionResult AddRole()
        {
            return View();
        }

        // POST: Role/AddRole
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = new IdentityRole { Name = roleName };
                await _roleManager.CreateAsync(role);
                return RedirectToAction(nameof(AddRole));
            }
            return View();
        }

        // GET: Permission/AssignPermissionsToRoles
        public IActionResult AssignPermissionsToRoles()
        {
            var roles = _roleManager.Roles.ToList();
            
            var permissions = _context.Permissions?.ToList() ?? new List<Permission>();
            ViewBag.Roles = roles;
            ViewBag.Permissions = permissions;
            return View();
        }

        // POST: Permission/AssignPermissionsToRoles
        [HttpPost]
        public async Task<IActionResult> AssignPermissionsToRoles(string userName, List<string> rolePermissions)
        {
            if (!string.IsNullOrEmpty(userName) && rolePermissions != null)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    foreach (var rolePermission in rolePermissions)
                    {
                        var parts = rolePermission.Split('-');
                        var roleName = parts[0];
                        var permissionName = parts[1];

                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            if (!await _userManager.IsInRoleAsync(user, roleName))
                            {
                                await _userManager.AddToRoleAsync(user, roleName);
                            }

                            var permission = _context.Permissions?.FirstOrDefault(p => p.Name == permissionName);
                            if (permission != null)
                            {
                                var rolePermissionEntity = new RolePermission
                                {
                                    RoleId = role.Id,
                                    PermissionId = permission.Id
                                };
                                if (_context.RolePermissions != null)
                                {
                                    _context.RolePermissions.Add(rolePermissionEntity);
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AssignPermissionsToRoles));
                }
            }
            return View();
        }
    }
}