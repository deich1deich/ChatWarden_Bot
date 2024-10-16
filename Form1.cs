using PeaceDaBoll.Messages;
using PeaceDaBoll.Profiles;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PeaceDaBoll
{
    public partial class Form1 : Form
    {
#nullable disable
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // токен бота
        private const string MyChatId = "-1002397315613"; //cwars - -1002279258485 || тестовый чат - -1002397315613
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;
        private static bool isReceivingMessages = true;
        private const string HELP_MESSAGE =
        "Список команд для пользователей:\n" +
        "1. /roll - Генерирует случайное число от 0 до значения которое вы указали.\n" +
        "Пример: /roll 100 Вывод - 52\n" +
        "2. /voteban - Начинает процесс голосования против участника чата. Против администратора создавать голосование нельзя.\n" +
        "Пример: /voteban должен быть ответом на сообщение пользователя.\n" +
        "Примечание: нельзя начинать по отношению к админам.\n" +
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

        public static async Task<bool> IsUserAdmin(long user)                     // S00QA ISPOLZUITE ETU HUETU, а лучше перепишите адекватно.
        {
            var chatMembers = await Bot.GetChatMemberAsync(MyChatId, user);

            return chatMembers.Status == ChatMemberStatus.Administrator || chatMembers.Status == ChatMemberStatus.Creator;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => cts.Cancel();

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //try
            //{
            var user = update.Message.From;
            string messageText = update.Message.Text;
            var chatMember = await Bot.GetChatMemberAsync(MyChatId, user.Id);
            string userReply = update.Message.ReplyToMessage?.From.Username.Replace("@", "");

            #region bespoleznaya_huita
            if (MessageType.Text == update.Message.Type)
            {
                WriteLog($"({user}) {messageText}");
            }
            else if (MessageType.Audio == update.Message.Type)
            {
                WriteLog($"({user}) отправил аудио длиной {update.Message.Audio.Duration} сек");
            }
            else if (MessageType.Voice == update.Message.Type)
            {
                WriteLog($"({user}) отправил аудио длиной {update.Message.Voice.Duration} сек");
            }
            else if (MessageType.Photo == update.Message.Type)
            {
                WriteLog($"({user}) отправил изображение");
            }
            #endregion

            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] {update.Message.From.Username} ({update.Message.From.Id}): {update.Message.Text}{Environment.NewLine}"));

                #region admin&&Creator
                if (IsUserAdmin(user.Id).Result || user.Username.Replace("@", "") == "BlastorChan" || user.Username.Replace("@", "") == "IamDeich")
                {
                    if (messageText.StartsWith("/on") && isReceivingMessages == false)
                    {
                        isReceivingMessages = true;
                        await Bot.SendTextMessageAsync(MyChatId, "Бот включен.");
                    }
                    else if (messageText.StartsWith("/off") && isReceivingMessages != false)
                    {
                        isReceivingMessages = false;
                        await Bot.SendTextMessageAsync(MyChatId, "Бот выключен.");
                    }
                    if (isReceivingMessages == true)
                    {
                        if (messageText.StartsWith("/point") && update.Message.ReplyToMessage != null) //Работа с очками пользователя
                        {
                            UserStat.ChangePoints(userReply, messageText);
                            await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: значение баллов изменено.");
                        }
                        else if (messageText.StartsWith("/point") && update.Message.ReplyToMessage == null)
                        {
                            await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
                        }
                        else if (messageText.StartsWith("/warn") && update.Message.ReplyToMessage != null)
                        {
                            if (chatMember.Status != ChatMemberStatus.Administrator || chatMember.Status != ChatMemberStatus.Creator)
                            {
                                UserStat.ChangeWarning(userReply, messageText);
                                await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: количество предупреждений изменено.");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(MyChatId, "Применять данные команды по отношению к админам нельзя");
                            }
                        }
                        else if (messageText.StartsWith("/warn") && update.Message.ReplyToMessage == null)
                        {
                            await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
                        }
                        else if (messageText.StartsWith("/editname") && update.Message.ReplyToMessage != null)
                        {
                            UserStat.ChangeCustomNickname(userReply, messageText);
                            await botClient.SendTextMessageAsync(MyChatId, $"{userReply}: ник изменён.");
                        }
                        else if (messageText.StartsWith("/editname") && update.Message.ReplyToMessage == null)
                        {
                            await botClient.SendTextMessageAsync(MyChatId, "Выделите пользователя ответом.");
                        }
                        else if (messageText.StartsWith("/badword"))
                        {
                            await BadwordsCheck.EnterBadword(Bot, update, MyChatId, Bot);
                        }
                        else if (messageText.StartsWith("/cancelvoteban"))
                        {
                            await Voting.CancelVoting(MyChatId, Bot);
                        }
                    }
                }
                #endregion

                #region Others
                if (isReceivingMessages == true)
                {
                    if (update.Type == UpdateType.Message)
                    {
                        string name = user.Username.Replace("@", "");
                        if (FileProfiles.Exists(name)) //Обработка последней активности пользователя исходя из отправленных сообщений
                        {
                            UserStat.ChangeLastDate(name, DateTime.Now); //Изменение даты последней активности
                            UserStat.ChangeMessageCount(name); //Обновление кол-ва сообщений отправленных пользователем ++
                            UserStat.RankUp(name); //Обновление ранга
                        }
                        else
                        {
                            UserStat.AddUser(name);
                            await botClient.SendTextMessageAsync(MyChatId, $"Профиль нового пользователя создан!" + Environment.NewLine + $"{UserStat.ViewProfile(name)}");
                            UserStat.ChangeMessageCount(name);
                        }
                        if (messageText.StartsWith("/help"))
                        {
                            await Bot.SendTextMessageAsync(MyChatId, HELP_MESSAGE);
                        }
                        else if (messageText.StartsWith("/roll") && int.TryParse(Regex.Match(messageText, "[0-9]+$").Value, out int value))
                        {
                            if (value <= int.MaxValue - 2)
                            {
                                await Bot.SendTextMessageAsync(MyChatId, new Random().Next(0, Convert.ToInt32(new Regex("[0-9]+$").Match(update.Message.Text).Value) + 1).ToString());
                            }
                            else if (value > int.MaxValue - 2)
                            {
                                await Bot.SendTextMessageAsync(MyChatId, "Введённое число больше максимально возможного.");
                            }
                        }
                        else if (messageText.StartsWith("/voteban") && update.Message.ReplyToMessage?.From.Id != null)
                        {
                            await Voting.StartVoting(MyChatId, update.Message, user.Id, Bot);
                        }
                        else if (messageText.StartsWith("/vote"))
                        {
                            await Voting.VotingProcessing(MyChatId, (int)user.Id, Bot);
                        }
                        else if (messageText.StartsWith("/profile"))
                        {
                            string profile = "";
                            if (FileProfiles.Exists(Regex.Match(messageText, @"(?<=/profile )[A-Za-z0-9]+").Value))
                            {
                                profile = UserStat.ViewProfile(messageText);
                                await botClient.SendTextMessageAsync(MyChatId, profile);
                            }
                            else if (messageText == "/profile")
                            {
                                profile = UserStat.ViewProfile(user.Username.Replace("@", ""));
                                await botClient.SendTextMessageAsync(MyChatId, profile);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(MyChatId, "Такого пользователя не существует!");
                            }
                        }
                        else if (chatMember.Status == ChatMemberStatus.Member)
                        {
                            await BadwordsCheck.MessageCheck(update.Message, MyChatId, Bot);
                        }
                    }
                }
                #endregion
            }
            //}
            //catch (Exception exception)
            //{
            //    //Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] Ошибка: {ex}"));
            //    //Chat_TextBox.Text += $"Ошибка: {ex}" + Environment.NewLine;
            //    await LoggingLogic.LoggingWriter($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}\nSource: {exception.Source}");
            //}
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