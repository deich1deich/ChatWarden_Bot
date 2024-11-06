using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;
using Message = Telegram.Bot.Types.Message;

namespace PeaceDaBoll.Messages
{
    public class Badwords
    {
        private static readonly string BadWordsFilePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\banwords.txt"; //Путь к файлу banwords
        private static readonly char[] separators = [' ', ',', '-', '.']; //Разделители слов

        public static Dictionary<int,DateTime> NotificationMessages = [];
        public static Message messageBot;

        /// <summary>
        /// Получает каждое слово из файла с плохими словами
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns>Возвращает список слов</returns>
        private static List<string> GetBadwords(string path) => File.ReadAllLines(path).ToList();

        /// <summary>
        /// Принимает слово которое добавится в файл с плохими словами
        /// </summary>
        /// <param name="message">Объект сообщения</param>
        /// <param name="chatMember">Объект участника чата</param>
        /// <param name="MyChatId">Id чата</param>
        /// <param name="Bot">Объект бота</param>
        public static async Task EnterBadword(ITelegramBotClient Bot, Telegram.Bot.Types.Message message, ChatMember chatMember, string MyChatId)
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
        /// Проверяет каждое слово из текста на наличие его в файле banwords.txt
        /// </summary>
        /// <param name="message">Объект сообщения</param>
        /// <param name="MyChatId">Id чата</param>
        /// <param name="Bot">Объект бота</param>
        public static async Task MessageCheck(ITelegramBotClient Bot, Message message, string MyChatId)
        {
            var badWords = GetBadwords(BadWordsFilePath).Select(word => word.ToLower()).ToHashSet();
            bool result = message.Text.Split(separators).Any(userWord => badWords.Contains(userWord.ToLower()));
            if (result)
            {
                await Bot.DeleteMessageAsync(MyChatId, message.MessageId);
                messageBot = await Bot.SendTextMessageAsync(MyChatId, $"Сообщение {message.From.Username} удалено, так как содержит запрещенное слово.");
                NotificationMessages.Add(messageBot.MessageId, Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss")).AddMinutes(1));
            }
        }
        /// <summary>
        /// Проверка времени отправленных сообщений ботом для их удаления
        /// </summary>
        /// <param name="Bot">Объект бота</param>
        /// <param name="MyChatId">Id чата</param>
        /// <returns></returns>
        public static async Task CheckDict(ITelegramBotClient Bot, string MyChatId)
        {
            if (NotificationMessages != null)
            {
                foreach (var item in NotificationMessages)
                {
                    if (item.Value <= Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss")))
                    {
                        await Bot.DeleteMessageAsync(MyChatId, item.Key);
                        NotificationMessages.Remove(item.Key);
                        await Bot.SendTextMessageAsync(MyChatId, $"Сообщение бота удалено в {DateTime.Now.ToString("HH:mm:ss")}");
                    }
                }
            }
        }
    }
}
