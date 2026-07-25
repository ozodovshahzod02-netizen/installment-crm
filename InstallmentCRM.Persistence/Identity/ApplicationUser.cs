using Microsoft.AspNetCore.Identity;

namespace InstallmentCRM.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}