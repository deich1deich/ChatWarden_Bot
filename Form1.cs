using PeaceDaBoll.Profiles;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PeaceDaBoll.Messages;
using static PeaceDaBoll.Profiles.ProfileLogicXYI.CustomLogicProfiles;
using PeaceDaBoll.Messages.Logging;
using Microsoft.VisualBasic.ApplicationServices;

namespace PeaceDaBoll
{
    // �������� ������ �� ����� ����������, ������� ���������� � 00:00:00
    // ������� �� ������� � ����������
    // Напоминание
    // сделать логику присвоения нового ранга
    // Сделать логирование ошибок и сообщений и т.д.

    public partial class Form1 : Form
    {
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // токен бота
        private const string MyChatId = "-1002397315613"; //cwars - -1002279258485 || тестовый чат - -1002397315613
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;
        private static bool isReceivingMessages = true;

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

        public static async Task<bool> IsUserAdmin(ITelegramBotClient botClien, string chatId, long user)
        {
            var chatMembers = await botClien.GetChatMemberAsync(chatId, user);

            return chatMembers.Status == ChatMemberStatus.Administrator || chatMembers.Status == ChatMemberStatus.Creator;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => cts.Cancel();

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var userId = update.Message.From.Id;
                var user = update.Message.From;
                string messageText = update.Message.Text;
                var chatMember = await Bot.GetChatMemberAsync(chatId: MyChatId, userId);

                if (MessageType.Text == update.Message.Type)
                {
                    await LoggingLogic.LoggingWriter($"({user}) {messageText}");
                }
                else if (MessageType.Audio == update.Message.Type)
                {
                    int dur = update.Message.Audio.Duration;
                    await LoggingLogic.LoggingWriter($"({user}) отправил аудио длиной {dur} сек");
                }
                else if (MessageType.Voice == update.Message.Type)
                {
                    int dur = update.Message.Voice.Duration;
                    await LoggingLogic.LoggingWriter($"({user}) отправил аудио длиной {dur} сек");
                }
                else if (MessageType.Photo == update.Message.Type)
                {
                    await LoggingLogic.LoggingWriter($"({user}) отправил изображение");
                }

                if (update.Type == UpdateType.Message && update.Message?.Text != null)
                {
                    Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] {update.Message.From.Username} ({update.Message.From.Id}): {update.Message.Text}{Environment.NewLine}"));

                    if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator || user.Username.Replace("@", "") == "BlastorChan" || user.Username.Replace("@", "") == "IamDeich")
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

                        if (messageText.StartsWith("/point")) //Работа с очками пользователя
                        {
                            string userReply = update.Message.ReplyToMessage?.From.Username.Replace("@", "");
                            int value = Convert.ToInt32(Regex.Match(messageText, "(?<=/point )[-]?[0-9]+").Value);
                            if (value != 0)
                            {
                                await UserProfileLogic.ChangePointsUser(userReply, value);
                                await botClient.SendTextMessageAsync(MyChatId, $"{userReply} добавлено {value} очков");
                            }
                        }

                        if (update.Message.ReplyToMessage != null && messageText.StartsWith('/')) //Работа с выдачей предупреждений пользователю
                        {
                            string userReply = update.Message.ReplyToMessage?.From.Username.Replace("@", "");
                            long userReplyId = update.Message.ReplyToMessage.From.Id;
                            string messageReply = update.Message.ReplyToMessage.Text;
                            if (!await IsUserAdmin(botClient, MyChatId, userReplyId))
                            {
                                if (messageText.StartsWith("/warn"))
                                {
                                    int warn = Convert.ToInt32(Regex.Match(messageText, "(?<=/warn )[-]?[0-9]+").Value);
                                    if (warn > 0)
                                    {
                                        await UserProfileLogic.AddWarningToUser(userReply);
                                        await botClient.SendTextMessageAsync(MyChatId, $"Предупреждение(я) для {userReply}");
                                    }
                                    else if (warn < 0)
                                    {
                                        await UserProfileLogic.AddWarningToUser(userReply);
                                        await botClient.SendTextMessageAsync(MyChatId, $"Предупреждение(я) снято(ы) для {userReply}");
                                    }
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(MyChatId, "Применять данные команды по отношению к админам нельзя");
                            }

                            if (messageText.StartsWith("/editname"))
                            {
                                string newName = Regex.Match(messageText, @"(?<=/editname )[A-Za-zА-Яа-я0-9]+").Value;
                                await UserProfileLogic.EditCustomUsername(update.Message.ReplyToMessage?.From.Username.Replace("@", ""), newName);
                                await botClient.SendTextMessageAsync(MyChatId, $"Второй ник пользователя изменен на {newName}");
                            }
                        }
                    }

                    if (isReceivingMessages == true)
                    {
                        if (chatMember.Status == ChatMemberStatus.Member)
                        {
                            await BadwordsCheck.MessageCheck(update.Message, MyChatId, Bot);
                        }
                        if (update.Type == UpdateType.Message)
                        {
                            string name = user.Username.Replace("@", "");
                            if (ProfileExists(name)) //Обработка последней активности пользователя исходя из отправленных сообщений
                            {
                                await UserProfileLogic.SetLastDate(name, DateTime.Now); //Изменение даты последней активности
                                await UserProfileLogic.AddMessageCount(name); //Обновление кол-ва сообщений отправленных пользователем ++
                                UserProfile profile = GetProfile(name);
                                if (profile.quantityMessage > profile.quantityMessage * 1000)
                                {
                                    EditProfile(name, ProfileValueType.currentRank, (profile.currentRank++).ToString());
                                }
                            }
                            else
                            {
                                await UserProfileLogic.AddUser(name);
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    $"Профиль нового пользователя создан!" + Environment.NewLine +
                                    $"{await UserProfileLogic.ViewProfile(name)}"
                                    );
                                await UserProfileLogic.AddMessageCount(name);
                            }
                        }
                        if (messageText.StartsWith("/help"))
                        {
                            await Bot.SendTextMessageAsync(
                            chatId: MyChatId,
                            text:
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
                            "Для поддержки в развитии проекта: \r\nСБЕР 4274 3200 5645 0680 \r\nВсе полученные средства уйдут на развитие проекта.",
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
                            await BadwordsCheck.EnterBadword(update.Message, chatMember, MyChatId, Bot);
                        }
                        else if (messageText.StartsWith("/voteban") && update.Message.ReplyToMessage?.From.Id != null)
                        {
                            await Voting.StartVoting(MyChatId, update.Message, userId, Bot);
                        }
                        else if (messageText.StartsWith("/cancelvoteban"))
                        {
                            await Voting.CancelVoting(MyChatId, Bot);
                        }
                        else if (messageText.StartsWith("/vote"))
                        {
                            await Voting.VotingProcessing(MyChatId, (int)userId, Bot);
                        }
                        else if (messageText.StartsWith("/profile"))
                        {
                            string profile = "";
                            string username = Regex.Match(messageText, @"(?<=/profile )[A-Za-z0-9]+").Value;
                            if (ProfileExists(username))
                            {
                                profile = await UserProfileLogic.ViewProfile(username);
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    profile
                                    );
                            }
                            else if (messageText == "/profile")
                            {
                                profile = await UserProfileLogic.ViewProfile(user.Username.Replace("@", ""));
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    profile
                                    );
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    "Такого пользователя не существует!"
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //Invoke((MethodInvoker)(() => Chat_TextBox.Text += $"[{DateTime.Now:G}] Ошибка: {ex}"));
                //Chat_TextBox.Text += $"Ошибка: {ex}" + Environment.NewLine;
                await LoggingLogic.LoggingWriter($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}\nSource: {exception.Source}");
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            LoggingLogic.LoggingWriter($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}\nSource: {exception.Source}");
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

        private void button2_Click(object sender, EventArgs e) => ((Button)sender).Text = (isStartProcess = isStartProcess ? false : true).ToString();

        private async void Update()
        {
            while (true)
            {
                if (isStartProcess && isReceivingMessages)
                {
                    label1.Text = "Прошедшее время: " + (Math.Round((DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds), 0));
                    if (DateTime.Now.TimeOfDay.TotalSeconds - lastReceive.TimeOfDay.TotalSeconds > 2600) //28800
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
//else if (messageText.StartsWith("/create"))
//{
//    //string username = Regex.Match(update.Message.Text, @"(?<=/profile )[A-Za-z0-9]+").Value;
//    string username = update.Message.From.Username.Replace("@", "");
//    if (!ProfileExists(username))
//    {
//        UserProfileLogic.AddUser(username);
//        await botClient.SendTextMessageAsync(
//            MyChatId,
//            $"Профиль {username} создан!"
//            );
//    }
//}

//else if (messageText.StartsWith("/create") && !ProfileExists(update.Message.From.Username.Replace("@", "")))
//{
//    CustomLogicProfiles.AddNewProfile(, update.Message.From.Username.Replace("@", ""));
//}
//else if(update.Type == UpdateType.ChatMember)
//{
//    MessageBox.Show("update with chatMember");
//    if(update.ChatMember.NewChatMember.Status == ChatMemberStatus.Member)
//    {
//        MessageBox.Show("update with newchatMember");
//        await Bot.SendTextMessageAsync(
//            chatId: MyChatId,
//            text: $"{update.ChatMember.NewChatMember.User.Username}, ����� ����������!",
//            cancellationToken: cancellationToken);
//    }
//    else if(update.ChatMember.OldChatMember.Status == ChatMemberStatus.Left)
//    {
//        MessageBox.Show("update with leftChatMember");
//        await Bot.SendTextMessageAsync(
//            chatId: MyChatId,
//            text: "� �� �����������.",
//            cancellationToken: cancellationToken);
//    } 
//}

//switch (ProfileExists(update.Message.ReplyToMessage?.From.Username))
//{
//    case true:
//        profile = UserProfileLogic.ViewProfile(update.Message.ReplyToMessage?.From.Username);
//        await botClient.SendTextMessageAsync(
//            MyChatId,
//            profile
//            );
//        return;
//    case false:
//        await botClient.SendTextMessageAsync(
//            MyChatId,
//            "У данного пользователя не существует профиля. Создание профиля..."
//            );
//        await UserProfileLogic.AddUser(update.Message.ReplyToMessage?.From.Username);
//        profile = UserProfileLogic.ViewProfile(update.Message.ReplyToMessage?.From.Username);
//        await botClient.SendTextMessageAsync(
//            MyChatId,
//            profile
//            );
//        return;
//}

//if (messageText.StartsWith("/addpoint"))
//{
//    string userReplay = update.Message.ReplyToMessage?.From.Username.Replace("@", "");
//    int value = Convert.ToInt32(Regex.Match(messageText, "(?<=/addpoint )[0-9]+").Value);
//    if (value != 0)
//    {
//        UserProfileLogic.AddPointsToUser(userReplay, value);
//    }
//}
//else if (messageText.StartsWith("/reducepoint"))//Эта часть кода просто ебучий ужас
//{
//    string userReplay = update.Message.ReplyToMessage?.From.Username.Replace("@", "");
//    int value = Convert.ToInt32(Regex.Match(messageText, "(?<=/reducepoint )[0-9]+").Value);
//    if (value != 0)
//    {
//        UserProfileLogic.TakePointsFromUser(userReplay, value);
//    }
//}