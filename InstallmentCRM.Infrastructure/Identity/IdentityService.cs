using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Persistence.Identity;
using InstallmentCRM.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace InstallmentCRM.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> RegisterAsync(
        string fullName,
        string email,
        string password,
        string? role)
    {
        var exists = await _userManager.FindByEmailAsync(email);

        if (exists != null)
            throw new ConflictException("Пользователь уже существует.");

        // Роль выбирается из ограниченного, самостоятельно доступного списка.
        // Admin намеренно недоступен для self-service регистрации.
        var normalizedRole = string.IsNullOrWhiteSpace(role)
            ? Roles.Seller
            : Roles.SelfRegisterable.FirstOrDefault(
                r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

        if (normalizedRole is null)
        {
            throw new ValidationException(
                $"Invalid role. Allowed roles: {string.Join(", ", Roles.SelfRegisterable)}.");
        }

        var user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new ValidationException(string.Join(
                ", ",
                result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, normalizedRole);

        return "Пользователь успешно зарегистрирован.";
    }

    public async Task<string> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new ValidationException("Неверный логин или пароль.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            false);

        if (!result.Succeeded)
            throw new ValidationException("Неверный логин или пароль.");

        var roles = await _userManager.GetRolesAsync(user);

        return _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email!,
            roles);
    }
}
