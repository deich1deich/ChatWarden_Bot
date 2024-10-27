using PeaceDaBoll.Messages;
using PeaceDaBoll.Profiles;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace PeaceDaBoll
{
    public partial class Form1 : Form
    {
#nullable disable
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // токен бота
        private const string MyChatId = "-1002328750691"; //cwars - -1002279258485 || тестовый чат - -1002397315613 || ещё чатик - -1002328750691
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;
        private static bool isReceivingMessages = true;
        private const string HELP_MESSAGE =
        "Список команд для пользователей:\n" +
        "1. /roll - Генерирует случайное число от 0 до значения которое вы указали.\n" +
        "Пример: /roll 100 Вывод - 52\n" +
        "2. /voteban - Начинает процесс голосования против участника чата.\n" +
        "Пример: /voteban должен быть ответом на сообщение пользователя.\n" +
        "Примечание: нельзя начинать по отношению к админам, за исключением случаев, когда голосование начинает создатель чата.\n" +
        "3. /vote - Голосование за бан в текущий момент голосования.\n" +
        "4. /profile - Показывает ваш профиль или профиль другого пользователя.\n" +
        "Пример: /profile показывает ваш профиль. /profile [имя_пользователя] - показывает профиль другого пользователя.\n" +
        "\n" +
        "Список команд для админов:\n" +
        "1. /editname - Изменяет второй ник пользователя в профиле.\n" +
        "Пример: /editname [новый_ник] должен быть ответом сообщение пользователя чей ник нужно изменить\n" +
        "2. /point - добавляет или отнимает кол-во очков на счету пользователя.\n" +
        "Пример: /point [число] прибавляет введенное кол-во очков. /point [-число] отнимает введенное кол-во очков у пользователя, должно быть ответом на сообщение пользователя чьё кол-во очков нужно изменить.\n" +
        "3. /warn - Добавляет или отнимает предупреждения у пользователя\n" +
        "Пример: /warn [число] добавляет, /warn [-число] убавляет.\n" +
        "4. /badword - Добавляет слово в черный список и после удаляется при появлении в чате.\n" +
        "Пример: /badword [слово]\n" +
        "Для поддержки в развитии проекта: \r\nСБЕР 4274 3200 5645 0680 \r\nВсе полученные средства уйдут на развитие проекта.";

        public Form1()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            Bot = new TelegramBotClient(Token);
            cts = new CancellationTokenSource();
            Bot.StartReceiving(
               HandleUpdateAsync,
               HandleErrorAsync,
               new ReceiverOptions { AllowedUpdates = { } },
               cancellationToken: cts.Token
           );
            Update();
            await Task.Delay(1);
        }

        public static async Task<bool> IsUserAdmin(long user)
        {
            var chatMembers = await Bot.GetChatMemberAsync(MyChatId, user);

            return chatMembers.Status == ChatMemberStatus.Administrator || chatMembers.Status == ChatMemberStatus.Creator;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => cts.Cancel();

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type != UpdateType.Message || update.Message == null || update.Message.Text == null)
                    return;

                User user = update.Message.From;
                string messageText = update.Message.Text;
                var chatMember = await botClient.GetChatMemberAsync(MyChatId, user.Id);
                string userReply = update.Message.ReplyToMessage?.From.Username?.Replace("@", "") ?? "";

                LogMessage(user, messageText, update.Message);

                if (await IsUserAdmin(user.Id) || IsSpecialUser(user.Username))
                {
                    await HandleAdminCommands(botClient, messageText, update.Message, userReply, chatMember);
                }

                if (isReceivingMessages)
                {
                    await HandleUserCommands(botClient, messageText, update.Message, user);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибок
                WriteLog($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}\nSource: {ex.Source}");
            }
        }

        private void LogMessage(User user, string messageText, Message message)
        {
            string logMessage;

            switch (message.Type)
            {
                case MessageType.Text:
                    logMessage = $"({user}) {messageText}";
                    break;
                case MessageType.Audio:
                    logMessage = $"({user}) отправил аудио длиной {message.Audio.Duration} сек";
                    break;
                case MessageType.Voice:
                    logMessage = $"({user}) отправил голосовое сообщение длиной {message.Voice.Duration} сек";
                    break;
                case MessageType.Photo:
                    logMessage = $"({user}) отправил изображение";
                    break;
                default:
                    return; // Ignore other message types
            }

            WriteLog(logMessage);
            Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] {user.Username} ({user.Id}): {messageText}{Environment.NewLine}"));
        }

        private bool IsSpecialUser(string username)
        {
            return username == "BlastorChan" || username == "IamDeich";
        }

        private async Task HandleAdminCommands(ITelegramBotClient botClient, string messageText, Message message, string userReply, ChatMember chatMember)
        {
            if (messageText.StartsWith("/on") && !isReceivingMessages)
            {
                isReceivingMessages = true;
                await botClient.SendTextMessageAsync(MyChatId, "Бот включен.");
            }
            else if (messageText.StartsWith("/off") && isReceivingMessages)
            {
                isReceivingMessages = false;
                await botClient.SendTextMessageAsync(MyChatId, "Бот выключен.");
            }

            if (isReceivingMessages)
            {
                if (messageText.StartsWith("/point"))
                {
                    await HandlePointCommand(botClient, message, userReply, messageText);
                }
                else if (messageText.StartsWith("/warn"))
                {
                    await HandleWarnCommand(botClient, message, userReply, chatMember);
                }
                else if (messageText.StartsWith("/editname"))
                {
                    await HandleEditNameCommand(botClient, message, userReply, messageText);
                }
                else if (messageText.StartsWith("/badword"))
                {
                    await BadwordsCheck.EnterBadword(botClient, message, chatMember, MyChatId);
                }
                else if (messageText.StartsWith("/cancelvoteban"))
                {
                    await Vote.CancelVoting(botClient, message, MyChatId);
                }
            }
        }

        private async Task HandlePointCommand(ITelegramBotClient botClient, Message message, string userReply, string messageText)
        {
            if (message.ReplyToMessage != null)
            {
                UserStat.ChangePoints(userReply, messageText);
                await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: значение баллов изменено.");
            }
            else
            {
                await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
            }
        }

        private async Task HandleWarnCommand(ITelegramBotClient botClient, Message message, string userReply, ChatMember chatMember)
        {
            if (IsUserAdmin(message.From.Id).Result)
            {
                if (message.ReplyToMessage != null)
                {
                    UserStat.ChangeWarning(userReply, message.Text);
                    await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: количество предупреждений изменено.");
                }
                else
                {
                    await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(MyChatId, "Применять данные команды по отношению к админам нельзя");
            }
        }

        private async Task HandleEditNameCommand(ITelegramBotClient botClient, Message message, string userReply, string messageText)
        {
            if (message.ReplyToMessage != null)
            {
                UserStat.ChangeCustomNickname(userReply, messageText);
                await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: ник изменён.");
            }
            else
            {
                await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
            }
        }

        private async Task HandleUserCommands(ITelegramBotClient botClient, string messageText, Message message, User user)
        {
            string username = user.Username.Replace("@", "");
            if (FileProfiles.Exists(username))
            {
                UserStat.ChangeLastDate(username, DateTime.Now);
                UserStat.ChangeMessageCount(username);
                UserStat.RankUp(username);
            }
            else
            {
                UserStat.AddUser(username);
                await botClient.SendTextMessageAsync(MyChatId, $"Профиль нового пользователя создан!" + Environment.NewLine + $"{UserStat.ViewProfile(username)}");
                UserStat.ChangeMessageCount(username);
            }

            if (messageText.StartsWith("/help"))
            {
                await botClient.SendTextMessageAsync(MyChatId, HELP_MESSAGE);
            }
            else if (messageText.StartsWith("/roll") && TryGetRollValue(messageText, out int rollValue))
            {
                await botClient.SendTextMessageAsync(MyChatId, new Random().Next(0, rollValue + 1).ToString());
            }
            else if (messageText.StartsWith("/voteban") && message.ReplyToMessage?.From.Id != null)
            {
                await Vote.StartVoting(botClient, message, MyChatId);
            }
            else if (messageText.StartsWith("/vote"))
            {
                await Vote.VotingProcessing(botClient, message, MyChatId);
            }
            else if (messageText.StartsWith("/profile"))
            {
                await HandleProfileCommand(botClient, messageText, user);
            }
            else
            {
                await BadwordsCheck.MessageCheck(botClient, message, MyChatId);
            }
        }

        private bool TryGetRollValue(string messageText, out int value)
        {
            string numberPart = Regex.Match(messageText, "[0-9]+$").Value;
            return int.TryParse(numberPart, out value) && value <= int.MaxValue - 2;
        }

        private async Task HandleProfileCommand(ITelegramBotClient botClient, string messageText, User user)
        {
            string targetUsername = Regex.Match(messageText, @"(?<=/profile )[A-Za-z0-9_]+").Value;

            if (string.IsNullOrEmpty(targetUsername))
            {
                targetUsername = user.Username.Replace("@", "");
            }

            if (FileProfiles.Exists(targetUsername))
            {
                string profile = UserStat.ViewProfile(targetUsername);
                await botClient.SendTextMessageAsync(MyChatId, profile);
            }
            else
            {
                await botClient.SendTextMessageAsync(MyChatId, "Такого пользователя не существует!");
            }
        }

        private async Task<bool> IsUserMember(ChatMember chatMember)
        {
            return chatMember.Status == ChatMemberStatus.Member;
        }

        #region HandleError
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            WriteLog($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}\nSource: {exception.Source}");
            return Task.CompletedTask;
        }
        #endregion

        #region Button_Send
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
        #endregion

        #region StartReceiving
        bool isStartProcess = false;
        DateTime lastReceive = DateTime.Now;

        private void button2_Click(object sender, EventArgs e) => ((Button)sender).Text = (isStartProcess = isStartProcess ? false : true).ToString();

        private new async void Update()
        {
            while (true)
            {
                if (isStartProcess && isReceivingMessages)
                {
                    label1.Text = "Прошедшее время: " + (Math.Round((DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds), 0));
                    if (DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds > 28800) // 28800 - 8ч  |  2400 - 40мин
                    {
                        lastReceive = DateTime.Now;

                        await Bot.SendTextMessageAsync(
                        chatId: MyChatId,
                        text: textBox2.Text,
                        cancellationToken: cts.Token);
                    }
                }
                await Task.Delay(1000);
            }
        }
        #endregion

        #region Tray
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;
            }
        }
        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        #endregion

        public static void WriteLog(string message) => System.IO.File.AppendAllText(Path.GetDirectoryName(Application.ExecutablePath) + @$"\log-{DateTime.Now:dd.MM.yyy}.txt", $"{DateTime.Now:G}: {message}\n");

    }
}