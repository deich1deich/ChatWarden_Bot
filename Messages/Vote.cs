using PeaceDaBoll.Profiles;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace PeaceDaBoll.Messages
{
    internal class Vote
    {
        /// <summary>
        /// Максимально допустимое количество потенциальных Voters.
        /// </summary>
        private const int MAX_VOTERS = 50;
        /// <summary>
        /// Минимально допустимое количество потенциальных Voters.
        /// </summary>
        private const int MIN_VOTERS = 5;
        /// <summary>
        /// Применяется, если Voters.Count / 2 превышает эту константу.
        /// </summary>
        private const int VOTERS_LIMIT = 20;

        /// <summary>
        /// Id пользователя, выступающего целью голосования.
        /// </summary>
        public static int TargetUserId = 0;
        /// <summary>
        /// null - нет голосования, false - голосование против ChatMemberStatus.Member, true - голосование против ChatMemberStatus.Admin.
        /// </summary>
        public static bool? VotingStatus = null;

        /// <summary>
        /// Хранит ID пользователей, проголосовавших за бан.
        /// </summary>
        public static HashSet<int> Voters = new HashSet<int>();

        /// <summary>
        /// Начало голосования. В зависимости от ChatMemberStatus Высчитывается VotingStatus, либо, если VotingStatus != null, возвращает собщение о уже проходящем голосовании
        /// </summary>
        /// <param name="Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task StartVoting(TelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            User mark = message.ReplyToMessage.From;
            User initiator = message.From;
            ChatMember tgMark = await Bot.GetChatMemberAsync(chatId, mark.Id);
            ChatMember tgInitiator = await Bot.GetChatMemberAsync(chatId, initiator.Id);

            if (VotingStatus == null)
            {
                switch (tgInitiator.Status)
                {
                    case ChatMemberStatus.Creator when tgMark.Status == ChatMemberStatus.Administrator:
                        VotingStatus = true;
                        TargetUserId = Convert.ToInt32(mark.Id);
                        Voters.Add(Convert.ToInt32(initiator.Id));
                        await Bot.SendTextMessageAsync(chatId, $"Голосование против {mark} создано. \nГолосовать могут только пользователи, достигшие звания младшего лейтенанта. \nГолосуйте с помощью /vote");
                        break;

                    case ChatMemberStatus.Administrator or ChatMemberStatus.Member or ChatMemberStatus.Creator when tgMark.Status == ChatMemberStatus.Member:
                        VotingStatus = false;
                        TargetUserId = Convert.ToInt32(mark.Id);
                        Voters.Add(Convert.ToInt32(initiator.Id));
                        await Bot.SendTextMessageAsync(chatId, $"Голосование против {mark} создано. \nГолосовать с помощью /vote.");
                        break;
                    case ChatMemberStatus.Member when tgMark.Status == ChatMemberStatus.Administrator || tgMark.Status == ChatMemberStatus.Creator:
                        await Bot.SendTextMessageAsync(chatId, "Вы не можете начать голосование против администратора/создателя.");
                        break;
                }
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId, "Голосование уже идёт.");
            }
        }
        /// <summary>
        /// Процесс голосвания. Производится вычисление необходимого кол-ва голосов при разных статусах голосования.
        /// </summary>
        /// <param name="Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task VotingProcessing(TelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            int countToBan = 0;
            int memberCount = await Bot.GetChatMemberCountAsync(chatId);
            UserProfile currentVoter = FileProfiles.Get(message.From.Username);
            int currentVoterId = Convert.ToInt32(message.From.Id);
            if (VotingStatus == null)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет активного голосования.");
            }
            else if (VotingStatus == false) 
            {
                if (!Voters.Contains(Convert.ToInt32(currentVoterId)))
                {
                    if (memberCount < MAX_VOTERS && memberCount > MIN_VOTERS)
                    {
                        countToBan = memberCount / 2;
                    }
                    else if (memberCount > MAX_VOTERS)
                    {
                        countToBan = VOTERS_LIMIT;
                    }
                    else if (memberCount < MIN_VOTERS)
                    {
                        countToBan = MIN_VOTERS;
                    }
                    Voters.Add(currentVoterId);
                    await Bot.SendTextMessageAsync(chatId, $"Вы проголосовали. Статус голосования: {Voters.Count}/{countToBan}.");
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, "Вы уже голосовали.");
                }
                if (Voters.Count >= countToBan)
                {
                    await Bot.BanChatMemberAsync(chatId, TargetUserId);
                    await Bot.SendTextMessageAsync(chatId, "Пользователь забанен.");
                    Voters.Clear();
                    TargetUserId = 0;
                    VotingStatus = null;
                }
            }
            else 
            {
                #region GetAllRank6Users
                    int counter = 0;
                    foreach (var user in FileProfiles.GetAll())
                    {
                        if (user.Value.currentRank >= 6)
                        {
                            counter++;
                        }
                    }
                    countToBan = counter / 3;
                    #endregion
                if (currentVoter.currentRank >= 6 && !Voters.Contains(currentVoterId))
                    {
                        Voters.Add(currentVoterId);
                        await Bot.SendTextMessageAsync(chatId, $"Вы проголосвали. Статус голосования: {Voters.Count}/{countToBan}");
                    }
                else
                    {
                        await Bot.SendTextMessageAsync(chatId, "Ваш ранг не подходит для голосования.");
                    }
                if (Voters.Count >= countToBan)
                    {
                        await Bot.BanChatMemberAsync(chatId, TargetUserId);
                        await Bot.SendTextMessageAsync(chatId, "Администратор забанен.");
                        Voters.Clear();
                        TargetUserId = 0;
                        VotingStatus = null;
                    }
            }
        }
        /// <summary>
        /// Метод отмены голосования, допускающий отмену пользователем creator всех голосований, а пользователем admin - отмену голосвания против member
        /// </summary>
        /// <param name="Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task CancelVoting(TelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            ChatMember member = await Bot.GetChatMemberAsync(chatId, message.From.Id);
            if (VotingStatus == null)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет голосования в текущий момент. \nНачать можно с помощью /voteban.");
            }
            else if (VotingStatus == false)
            {
                if (member.Status == ChatMemberStatus.Administrator || member.Status == ChatMemberStatus.Creator && message.From.Id != TargetUserId)
                {
                    Voters.Clear();
                    TargetUserId = 0;
                    await Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
                    VotingStatus = null;
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, "Вы не можете отменить голосование.");
                }
            }
            else
            {
                if (member.Status == ChatMemberStatus.Creator)
                {
                    Voters.Clear();
                    TargetUserId = 0;
                    await Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
                    VotingStatus = null;
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, "Вы не можете отменить голосование.");
                }
            }
        }
    }
}