using Telegram.Bot.Types.ReplyMarkups;

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
}
