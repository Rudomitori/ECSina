namespace ECSina.Common.Core.Extensions;

public static class RandomExtensions
{
    public static string NextString(
        this Random random,
        int length = 16,
        string alphabet = "0123456789ABCDEF"
    )
    {
        var charsArr = new char[length];

        for (var i = 0; i < length; i++)
            charsArr[i] = alphabet[random.Next(alphabet.Length)];

        return new string(charsArr);
    }
}
