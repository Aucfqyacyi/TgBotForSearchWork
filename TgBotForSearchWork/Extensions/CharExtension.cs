using System.Text;

namespace TgBotForSearchWork.Extensions;

internal static class CharExtension
{
    public static string Multiply(this char symbol, int count)
    {
        if (count == 0)
            return symbol.ToString();
        StringBuilder stringBuilder = new(count);
        for (int i = 0; i < count; i++)
        {
            stringBuilder.Append(symbol);
        }
        return stringBuilder.ToString();
    }

}
