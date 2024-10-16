using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace PeaceDaBoll.Messages
{
    public class BadwordsCheck
    {
        private static string BadWordsFilePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\banwords.txt"; //Путь к файлу banwords
        private static char[] separators = [' ', ',', '-', '.']; //Разделители слов (для парсинга плохих слов ата та)

        /// <summary>
        /// Принимает слово которое добавиться в файл с плохими словами
        /// </summary>
        /// <param name="message">Объект сообщения</param>
        /// <param name="chatMember">Объект участника чата</param>
        /// <param name="MyChatId">Id чата</param>
        /// <param name="Bot">Объект бота</param>
        public static async Task EnterBadword(TelegramBotClient Bot, Telegram.Bot.Types.Message message, ChatMember chatMember, string MyChatId)
        {
            var messageText = message.Text;
            var messageId = message.MessageId;
            if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
            {
                string badWord = messageText.Substring(8).Trim();
                if (!string.IsNullOrEmpty(badWord))
                {
                    File.AppendAllText(BadWordsFilePath, badWord + Environment.NewLine);
                    await Bot.DeleteMessageAsync(MyChatId, messageId);
                    await Bot.SendTextMessageAsync(MyChatId, @$"Слово <span class=""tg-spoiler"">{badWord}</span> добавлено в черный список.", parseMode: ParseMode.Html);
                }
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId: MyChatId, "Вы не имеете доступа.");
            }
        }

        /// <summary>
        /// Получает каждое слово из файла с плохими словами
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns>Возвращает список слов</returns>
        private static List<string> GetBadwords(string path) => File.ReadAllLines(path).ToList();

        /// <summary>
        /// Проверяет каждое слово из текста на наличие его в файле banwords.txt
        /// </summary>
        /// <param name="message">Объект сообщения</param>
        /// <param name="MyChatId">Id чата</param>
        /// <param name="Bot">Объект бота</param>
        public static async Task MessageCheck(TelegramBotClient Bot, Telegram.Bot.Types.Message message, string MyChatId)
        {
            var badWords = GetBadwords(BadWordsFilePath).Select(word => word.ToLower()).ToHashSet();
            bool result = message.Text.Split(separators).Any(userWord => badWords.Contains(userWord.ToLower()));
            if (result)
            {
                await Bot.DeleteMessageAsync(MyChatId, message.MessageId);
                await Bot.SendTextMessageAsync(MyChatId, "Сообщение удалено, так как содержит запрещенное слово.");
            }
        }

    }
}
