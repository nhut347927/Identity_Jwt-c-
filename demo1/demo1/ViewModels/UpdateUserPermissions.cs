using demo1.Models;

namespace demo1.ViewModels
{
    public class UpdateUserPermissionsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public Dictionary<string, RolePermission> Permissions { get; set; } = new Dictionary<string, RolePermission>();
    }
}