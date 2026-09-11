using Microsoft.AspNetCore.Identity;

namespace Vizus.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}