using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace PeaceDaBoll
{
    public partial class Form1 : Form
    {
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // Замените на ваш токен
        private const string MyChatId = "-1002279258485";
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;

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
            //if (update.Message.Chat.Id.ToString() == MyChatId) return;

            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                // Отображаем полученное сообщение в TextBox
                Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] {update.Message.From.Username} ({update.Message.From.Id}): {update.Message.Text}{Environment.NewLine}"));

                string messageText = update.Message.Text;

                if (messageText.StartsWith("/help"))
                {
                    await Bot.SendTextMessageAsync(
                    chatId: MyChatId,
                    text: "Список команд:\n1. /roll - Генерирует случайное число в диапазоне от 0 до указанного числа. Пример: /roll 100\nДля поддержки в развитии проекта:\nСБЕР 4274 3200 5645 0680.\nВсе полученные средства уйдут на развитие проекта.",
                    cancellationToken: cancellationToken);
                }
                else if (messageText.StartsWith("/roll") && int.TryParse(Regex.Match(messageText, "[0-9]+$").Value, out int value))
                {
                    await Bot.SendTextMessageAsync(
                        chatId: MyChatId,
                        text: new Random().Next(0, Convert.ToInt32(new Regex("[0-9]+$").Match(update.Message.Text).Value) + 1).ToString(),
                        cancellationToken: cancellationToken);
                }
                //else if (update.Message.Text.StartsWith("/voteban"))
                //{
                //    var parts = update.Message.Text.Split(' ');
                //    if (parts.Length != 2)
                //    {
                //        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Использование: /voteban @username", cancellationToken: cancellationToken);
                //        return;
                //    }

                //    var username = parts[1];
                //    var chatId = update.Message.Chat.Id;

                //    if (activeVotes.ContainsKey(chatId))
                //    {
                //        await botClient.SendTextMessageAsync(chatId, "Уже идет голосование за бан!", cancellationToken: cancellationToken);
                //        return;
                //    }

                //    var membersCount = await botClient.GetChatMembersCountAsync(chatId);
                //    var requiredVotes = (int)(membersCount * 0.6); // 60% от участников
                //    var votingSession = new VotingSession
                //    {
                //        Username = username,
                //        RequiredVotes = requiredVotes,
                //        Votes = new HashSet<long>(),
                //        EndTime = DateTime.UtcNow.AddMinutes(30)
                //    };

                //    activeVotes[chatId] = votingSession;

                //    await botClient.SendTextMessageAsync(chatId, $"Начато голосование за бан {username}. Необходимо {requiredVotes} голосов. У вас 30 минут.", cancellationToken: cancellationToken);
                //}
                //else if (update.Type == UpdateType.Message && update.Message.Text.StartsWith("/vote"))
                //{
                //    var chatId = update.Message.Chat.Id;

                //    if (!activeVotes.ContainsKey(chatId))
                //    {
                //        await botClient.SendTextMessageAsync(chatId, "Нет активного голосования.", cancellationToken: cancellationToken);
                //        return;
                //    }

                //    var votingSession = activeVotes[chatId];

                //    if (votingSession.EndTime < DateTime.UtcNow)
                //    {
                //        activeVotes.Remove(chatId);
                //        await botClient.SendTextMessageAsync(chatId, "Время голосования истекло.", cancellationToken: cancellationToken);
                //        return;
                //    }

                //    if (votingSession.Votes.Contains(update.Message.From.Id))
                //    {
                //        await botClient.SendTextMessageAsync(chatId, "Вы уже голосовали.", cancellationToken: cancellationToken);
                //        return;
                //    }

                //    votingSession.Votes.Add(update.Message.From.Id);
                //    await botClient.SendTextMessageAsync(chatId, $"{update.Message.From.Username} проголосовал за бан {votingSession.Username}.", cancellationToken: cancellationToken);

                //if (votingSession.Votes.Count >= votingSession.RequiredVotes)
                //{
                //    await BanUserAsync(botClient, chatId, votingSession.Username, cancellationToken);
                //    activeVotes.Remove(chatId);
                //}
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
                    if (DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds > 14400)
                    {
                        lastReceive = DateTime.Now;

                        await Bot.SendTextMessageAsync(
                   chatId: MyChatId,
                   text: "Список команд:\n1. /roll - Генерирует случайное число в диапазоне от 0 до указанного числа. Пример: /roll 100\nДля поддержки в развитии проекта: СБЕР 4274 3200 5645 0680. Все полученные средства уйдут на развитие проекта.",
                   cancellationToken: cts.Token);
                    }
                }
                await Task.Delay(1000);
            }
        }
    }

    public class VotingSession
    {
        public string Username { get; set; }
        public int RequiredVotes { get; set; }
        public HashSet<long> Votes { get; set; }
        public DateTime EndTime { get; set; }
    }
}