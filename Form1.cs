using Microsoft.VisualBasic.ApplicationServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using File = System.IO.File;
using Message = Telegram.Bot.Types.Message;



namespace PeaceDaBoll
{
    public partial class Form1 : Form
    {
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // Замените на ваш токен
        private const string MyChatId = "-1002279258485"; //cwars
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;
        private static string BadWordsFilePath = "badwords.txt"; //Путь к файлу badwords
        public static char[] separators = [' ', ',', '-', '.']; //Разделители слов (для парсинга badwords)
        private static Voting? currentVoting;
        private static bool isReceivingMessages = true;

        public Form1()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            Bot = new TelegramBotClient(Token);
            cts = new CancellationTokenSource();
            // Запускаем получение обновлений
            Bot.StartReceiving(
               HandleUpdateAsync,
               HandleErrorAsync,
               new ReceiverOptions { AllowedUpdates = { } }, // Укажите типы обновлений, которые хотите получать
               cancellationToken: cts.Token
           );
            Update();
            await Task.Delay(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => cts.Cancel(); // Останавливаем получение обновлений при закрытии формы

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                // Отображаем полученное сообщение в TextBox
                Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] {update.Message.From.Username} ({update.Message.From.Id}): {update.Message.Text}{Environment.NewLine}"));
                var userId = update.Message.From.Id;
                var user = update.Message.From;
                string messageText = update.Message.Text;
                var chatMember = await Bot.GetChatMemberAsync(chatId: MyChatId, userId);
                if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
                {
                    if (messageText.StartsWith("/on"))
                    {
                        isReceivingMessages = true;
                        await Bot.SendTextMessageAsync(MyChatId, "Бот включен.");
                    }
                    else if (messageText.StartsWith("/off"))
                    {
                        isReceivingMessages = false;
                        await Bot.SendTextMessageAsync(MyChatId, "Бот выключен.");
                    }
                }
                if (isReceivingMessages == true)
                {
                    if (messageText.StartsWith("/help"))
                    {
                        await Bot.SendTextMessageAsync(
                        chatId: MyChatId,
                        text: "Список команд:\n1. /roll - Генерирует случайное число в диапазоне от 0 до указанного числа. " +
                        "Пример: /roll 100\n2. /voteban - Создаёт голосование за бан пользователя(нельзя создать против администратора)\n" +
                        "3. /vote - проголосовать за бан, когда голосование открыто\n\n" +
                        "Для поддержки в развитии проекта: СБЕР 4274 3200 5645 0680. Все полученные средства уйдут на развитие проекта.\n\n" +
                        "Для админов: \n" +
                        "/badword [слово] - вписать слово в список запрещённых\n" +
                        "/votebancancel - отменить голосование за бан",
                        cancellationToken: cancellationToken);
                    }
                    else if (messageText.StartsWith("/roll") && int.TryParse(Regex.Match(messageText, "[0-9]+$").Value, out int value))
                    {
                        await Bot.SendTextMessageAsync(
                            chatId: MyChatId,
                            text: new Random().Next(0, Convert.ToInt32(new Regex("[0-9]+$").Match(update.Message.Text).Value) + 1).ToString(),
                            cancellationToken: cancellationToken);
                    }
                    else if (messageText.StartsWith("/badword"))
                    {
                        await BadwordsCheck.EnterBadword(update.Message, chatMember);
                    }
                    else if (messageText.StartsWith("/voteban") && update.Message.ReplyToMessage?.From.Id != null)
                    {
                        await StartVoting(MyChatId, update.Message, userId);
                    }
                    else if (messageText.StartsWith("/cancelvoteban"))
                    {
                        await CancelVoting(MyChatId);
                    }
                    else if (messageText.StartsWith("/vote"))
                    {
                        await VotingProcessing(MyChatId, (int)userId);
                    }
                    else if (chatMember.Status == ChatMemberStatus.Member)
                    {
                        await BadwordsCheck.MessageCheck(update.Message, separators);
                    }
                }


            }
        }

        //else if(update.Type == UpdateType.ChatMember)
        //{
        //    MessageBox.Show("update with chatMember");
        //    if(update.ChatMember.NewChatMember.Status == ChatMemberStatus.Member)
        //    {
        //        MessageBox.Show("update with newchatMember");
        //        await Bot.SendTextMessageAsync(
        //            chatId: MyChatId,
        //            text: $"{update.ChatMember.NewChatMember.User.Username}, добро пожаловать!",
        //            cancellationToken: cancellationToken);
        //    }
        //    else if(update.ChatMember.OldChatMember.Status == ChatMemberStatus.Left)
        //    {
        //        MessageBox.Show("update with leftChatMember");
        //        await Bot.SendTextMessageAsync(
        //            chatId: MyChatId,
        //            text: "И не возвращайся.",
        //            cancellationToken: cancellationToken);
        //    } 
        //}
        //else if(update.Type == UpdateType.Unknown)
        //{
        //    await Bot.SendTextMessageAsync(
        //            chatId: MyChatId,
        //            text: "Message.Type.Unknown",
        //            cancellationToken: cancellationToken);
        //}
        //else if(update.Type == UpdateType.MyChatMember)
        //{
        //    await Bot.SendTextMessageAsync(
        //            chatId: MyChatId,
        //            text: "Message.Type.MyChatMember",
        //            cancellationToken: cancellationToken);
        //}
        //}

        //private Dictionary<long, VotingSession> activeVotes = new();

        //private async Task BanUserAsync(ITelegramBotClient botClient, User user, CancellationToken cancellationToken)
        //{
        //    user.
        //    // Получаем ID пользователя по username
        //    if (user != null && user. != ChatMemberStatus.Left && user.Status != ChatMemberStatus.Kicked)
        //    {
        //        await botClient.BanChatMemberAsync(MyChatId, user.Id, cancellationToken: cancellationToken);
        //        await botClient.SendTextMessageAsync(MyChatId, $"{user.Username} был забанен.", cancellationToken: cancellationToken);
        //    }
        //}

        public class BadwordsCheck
        {
            public static async Task EnterBadword(Message message, ChatMember chatMember)
            {
                var messageText = message.Text;
                var messageId = message.MessageId;
                if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
                {
                    var badWord = messageText.Substring(8).Trim();
                    if (!string.IsNullOrEmpty(badWord))
                    {
                        File.AppendAllText(BadWordsFilePath, badWord + Environment.NewLine);
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
                return File.ReadAllLines(BadWordsFilePath).ToList();
            }
            public static async Task MessageCheck(Message message, char[] sep)
            {

                bool result = false;
                message.Text.Split(sep).ToList().ForEach(userWord => result = GetBadwords().Contains(userWord.ToLower()) ? true : result ? true : false);
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

        private static async Task StartVoting(string chatId, Message message, long userId)
        {
            if (currentVoting != null)
            {
                await Bot.SendTextMessageAsync(chatId, "Уже идет голосование!");
                return;
            }
            var userToBan = (long?)message.ReplyToMessage?.From.Id;
            var userToBanUsername = message.ReplyToMessage?.From.Username;
            var chatMember = await Bot.GetChatMemberAsync(MyChatId, (long)userToBan);

            if (userToBan == userId)
            {
                await Bot.SendTextMessageAsync(chatId, "Вы не можете голосовать против себя.");
                return;
            }
            if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
            {
                await Bot.SendTextMessageAsync(chatId, "Вы не можете голосовать против администратора.");
            }
            else if (chatMember.Status != ChatMemberStatus.Administrator || chatMember.Status != ChatMemberStatus.Creator)
            {
                currentVoting = new Voting((int)userToBan, (int)userId);
                await Bot.SendTextMessageAsync(chatId, $"Голосование за бан @{userToBanUsername} начато. Голосуйте с помощью /vote");
                currentVoting.Voters.Add((int)userId);
            }
        }

        private static async Task VotingProcessing(string chatId, int userid)
        {
            if (currentVoting != null && !currentVoting.Voters.Contains(userid))
            {
                currentVoting.VotersCount++;
                await Bot.SendTextMessageAsync(MyChatId, $"Вы проголосовали. Статус голосования: {currentVoting.VotersCount}/10");
                return;
            }
            else if (currentVoting != null && currentVoting.Voters.Contains(userid))
            {
                await Bot.SendTextMessageAsync(MyChatId, "Вы уже голосовали.");
                return;
            }
            else if (currentVoting == null)
            {
                await Bot.SendTextMessageAsync(MyChatId, "Нет активного голосования.");
                return;
            }
            if (currentVoting != null && currentVoting.VotersCount >= 10)
            {
                await Bot.SendTextMessageAsync(chatId, "По итогам голосования пользователь забанен.");
                await Bot.BanChatMemberAsync(chatId, (long)currentVoting.TargetUserId);
                currentVoting = null;
            }
        }


        private static async Task CancelVoting(string chatId)
        {
            if (currentVoting == null)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет активного голосования.");
                return;
            }

            currentVoting = null;
            await Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
        }


        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Обработка ошибок
            MessageBox.Show($"Ошибка: {exception.Message}\nStackTrace: {exception.StackTrace}\nSource: {exception.Source}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return Task.CompletedTask;
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string message = TextMessage_TextBox.Text;
            if (!string.IsNullOrEmpty(message))
            {
                // Отправка сообщения в чат
                await Bot.SendTextMessageAsync(
                    chatId: MyChatId, // Замените на ваш chat ID
                    text: message
                );

                // Отображаем отправленное сообщение в ListBox
                Chat_TextBox.Text += $"Отправлено: {message}" + Environment.NewLine;
                TextMessage_TextBox.Clear();
            }
        }

        bool isStartProcess = false;
        DateTime lastReceive = DateTime.Now;

        private void button2_Click(object sender, EventArgs e) => isStartProcess = isStartProcess ? false : true;

        private async void Update()
        {
            while (true)
            {
                if (isStartProcess)
                {
                    if (DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds > 28800)
                    {
                        lastReceive = DateTime.Now;

                        await Bot.SendTextMessageAsync(
                   chatId: MyChatId,
                   text: "Список команд:\n1. /roll - Генерирует случайное число в диапазоне от 0 до указанного числа. " +
                    "Пример: /roll 100\n2. /voteban - Создаёт голосование за бан пользователя(нельзя создать против администратора)\n" +
                    "3. /vote - проголосовать за бан, когда голосование открыто\n\n" +
                    "Для поддержки в развитии проекта: СБЕР 4274 3200 5645 0680. Все полученные средства уйдут на развитие проекта.\n\n" +
                    "Для админов: \n" +
                    "/badword [слово] - вписать слово в список запрещённых\n" +
                    "/votebancancel - отменить голосование за бан",
                   cancellationToken: cts.Token);
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}
public class Voting
{
    public int? TargetUserId { get; set; }
    public long? CreatorUserId { get; set; }
    public HashSet<int> Voters { get; set; }
    public int VotersCount { get; set; }

    public Voting(int targetUserId, int creatorUserId)
    {
        TargetUserId = targetUserId;
        CreatorUserId = creatorUserId;
        Voters = new HashSet<int>();
        VotersCount = 1;
    }
}
