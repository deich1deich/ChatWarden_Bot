using PeaceDaBoll.Profiles;
using System.Diagnostics.Contracts;
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
using PeaceDaBoll.Messages;
using PeaceDaBoll.Profiles.ProfileLogicXYI;
using Newtonsoft.Json.Linq;
using static PeaceDaBoll.Profiles.ProfileLogicXYI.CustomLogicProfiles;

namespace PeaceDaBoll
{
    // Добавить массив со всеми месседжами, который обнуляется в 00:00:00
    // Реакции из массива с месседжами
    //

    public partial class Form1 : Form
    {
        private static readonly string Token = "7665926697:AAFU7O64QE-jYfUSbjEG11ur8WkwAVolmbQ"; // Замените на ваш токен
        private const string MyChatId = "-1002397315613"; //cwars - -1002279258485 // тестовый чат - -1002397315613
        private static TelegramBotClient Bot;
        private CancellationTokenSource cts;
        private static bool isReceivingMessages = true;

        public Form1()
        {
            InitializeComponent();
            //EditProfile("fylax224luv", ProfileValueType.quantityMessage, "1488");
            //MessageBox.Show("Test");
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
                    if (chatMember.Status == ChatMemberStatus.Member)
                    {
                        await BadwordsCheck.MessageCheck(update.Message, MyChatId, Bot);
                    }

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
                        switch (ProfileExists(update.Message.From.Username.Replace("@", "")))
                        {
                            case true:
                                profile = UserProfileLogic.ViewProfile(update.Message.ReplyToMessage?.From.Username);
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    profile
                                    );
                                return;
                            case false:
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    "Профиля пользователя не существует! Сейчас будет создан профиль..."
                                    );
                                await UserProfileLogic.AddUser(update.Message.ReplyToMessage?.From.Username.Replace("@", ""));
                                profile = UserProfileLogic.ViewProfile(update.Message.ReplyToMessage?.From.Username);
                                await botClient.SendTextMessageAsync(
                                    MyChatId,
                                    profile
                                    );
                                return;
                        }

                    }
                    //else if (messageText.StartsWith("/create") && !ProfileExists(update.Message.From.Username.Replace("@", "")))
                    //{
                    //    CustomLogicProfiles.AddNewProfile(, update.Message.From.Username.Replace("@", ""));
                    //}

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