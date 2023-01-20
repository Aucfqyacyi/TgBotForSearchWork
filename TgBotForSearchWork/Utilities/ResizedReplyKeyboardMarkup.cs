using Telegram.Bot.Types.ReplyMarkups;

namespace TgBotForSearchWork.Utilities;

internal class ResizedReplyKeyboardMarkup : ReplyKeyboardMarkup
{

    public ResizedReplyKeyboardMarkup(KeyboardButton button) : base(button)
    {
        SetResizeKeyboard();
    }

    public ResizedReplyKeyboardMarkup(IEnumerable<KeyboardButton> keyboardRow) : base(keyboardRow)
    {
        SetResizeKeyboard();
    }

    public ResizedReplyKeyboardMarkup(IEnumerable<IEnumerable<KeyboardButton>> keyboard) : base(keyboard)
    {
        SetResizeKeyboard();
    }

    private void SetResizeKeyboard()
    {
        ResizeKeyboard = true;
    }
}
