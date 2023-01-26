using System.Reflection.PortableExecutable;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Extensions;

namespace TgBotForSearchWork.Utilities;

internal class ResizedKeyboardMarkup : ReplyKeyboardMarkup
{
    public ResizedKeyboardMarkup(KeyboardButton button) : base(button)
    {
        SetResizeKeyboard();
    }

    public ResizedKeyboardMarkup(IEnumerable<KeyboardButton> keyboardRow) : base(keyboardRow)
    {
        SetResizeKeyboard();
    }

    public ResizedKeyboardMarkup(IEnumerable<IEnumerable<KeyboardButton>> keyboard) : base(keyboard)
    {
        SetResizeKeyboard();
    }

    private void SetResizeKeyboard()
    {
        ResizeKeyboard = true;
    }

    public static ResizedKeyboardMarkup MakeList(IEnumerable<object> strings, char prefix, 
                                                              int prefixCount = 0, int? siteId = null)
    {
        List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
        foreach (var @string in strings)
        {
            keyboardButtons.Add(new() { "|", prefix.Multiply(prefixCount) + siteId + ": " + @string, "|" });          
        }
        return new ResizedKeyboardMarkup(keyboardButtons);
    }
}
