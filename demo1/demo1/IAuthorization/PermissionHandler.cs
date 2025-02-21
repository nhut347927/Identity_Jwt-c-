using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using demo1.Models;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public PermissionHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa tới đây 111111111111111111111");

        var userId = _userManager.GetUserId(context.User);
        Console.WriteLine($"User ID từ claim: {userId}");
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == userId);

        if (user == null)
        {
            Console.WriteLine("Không tìm thấy user trong database!");
            return;
        }

        Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa tới đây 2222222222222222222222");
        // Lấy danh sách RolePermission của User, kèm Role để lấy Role.Name
        var rolePermissions = _context.RolePermissions != null
            ? await _context.RolePermissions
                .Where(rp => rp.UserId == user.Id)
                .Include(rp => rp.Role)
                .ToListAsync()
            : new List<RolePermission>();
        if (!rolePermissions.Any())
        {
            return;
        }
        // Duyệt qua từng RolePermission để kiểm tra quyền
        foreach (var rolePermission in rolePermissions)
        {
            string roleName = rolePermission.Role.Name;
            // Danh sách các quyền cần kiểm tra
            var permissions = new List<(bool HasPermission, string PermissionName)>
            {
                (rolePermission.CanView, $"{roleName}-View"),
                (rolePermission.CanInsert, $"{roleName}-Insert"),
                (rolePermission.CanUpdate, $"{roleName}-Update"),
                (rolePermission.CanDelete, $"{roleName}-Delete")
            };
             // Kiểm tra từng quyền
            foreach (var (hasPermission, permissionName) in permissions)
            {
                Console.WriteLine($"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa {permissionName} = {requirement.Permission}");
                if (hasPermission && permissionName == $"{requirement.Permission}")
                {

                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }
}
