namespace ECSina.Db.Entities.Auth;

public sealed class UserComponent : DataComponent
{
    public required string Login { get; set; }
    public required string NormalizedLogin { get; set; }
    public required byte[] PasswordHash { get; set; }

    public bool IsAdmin { get; set; }
}
