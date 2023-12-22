namespace ECSina.Db.Entities.Auth;

public sealed class PasswordComponent : DataComponent
{
    public required byte[] PasswordHash { get; set; }
}
