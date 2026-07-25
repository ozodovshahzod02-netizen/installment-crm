namespace InstallmentCRM.Shared.Constants;

/// <summary>
/// Роли пользователей, используемые как в Identity-сидере (Persistence/API),
/// так и в [Authorize(Roles = "...")] атрибутах контроллеров.
/// Единственный источник правды, чтобы роль не "потерялась" в одном из мест.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Seller = "Seller";
    public const string Cashier = "Cashier";

    public static readonly string[] All =
    {
        Admin,
        Manager,
        Seller,
        Cashier
    };

    /// <summary>
    /// Роли, которые пользователь может выбрать сам при самостоятельной регистрации.
    /// Admin сознательно исключен - назначается вручную/через сидер.
    /// </summary>
    public static readonly string[] SelfRegisterable =
    {
        Manager,
        Seller,
        Cashier
    };
}
