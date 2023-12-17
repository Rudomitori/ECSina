namespace ECSina.Core;

public static class StringNormalizer
{
    public static string NormalizeEntityName(string name) => name.Trim();

    public static string NormalizeLogin(string login) => NormalizeEntityName(login);

    public static string NormalizeLoginToIndex(string login) => NormalizeLogin(login).ToUpper();
}
