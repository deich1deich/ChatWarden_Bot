using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace PeaceDaBoll.Messages
{
    public class BadwordsCheck
    {
        private static string BadWordsFilePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\banwords.txt"; //Путь к файлу banwords
        private static char[] separators = [' ', ',', '-', '.']; //Разделители слов (для парсинга плохих слов ата та)
        public static async Task EnterBadword(Telegram.Bot.Types.Message message, ChatMember chatMember, string MyChatId, TelegramBotClient Bot)
        {
            var messageText = message.Text;
            var messageId = message.MessageId;
            if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
            {
                var badWord = messageText.Substring(8).Trim();
                if (!string.IsNullOrEmpty(badWord))
                {
                    System.IO.File.AppendAllText(BadWordsFilePath, badWord + Environment.NewLine);
                    await Bot.DeleteMessageAsync(chatId: MyChatId, messageId);
                    await Bot.SendTextMessageAsync(chatId: MyChatId, "Слово добавлено в черный список.");
                }
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId: MyChatId, "Вы не имеете доступа.");
            }
        }
        private static List<string> GetBadwords()
        {
            return System.IO.File.ReadAllLines(BadWordsFilePath).ToList();
        }
        public static async Task MessageCheck(Telegram.Bot.Types.Message message, string MyChatId, TelegramBotClient Bot)
        {

            bool result = false;
            message.Text.Split(separators).ToList().ForEach(userWord => result = GetBadwords().Contains(userWord.ToLower()) ? true : result ? true : false);
            switch (result)
            {
                case true:
                    await Bot.DeleteMessageAsync
                        (
                            MyChatId,
                            message.MessageId
                        );
                    await Bot.SendTextMessageAsync
                        (
                            MyChatId,
                            text: "Сообщение удалено, так как содержит запрещённое слово."
                        );
                    return;
            }
        }
    }
}
