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
        /// HashSet, который хранит ID пользователей, проголосовавших за бан.
        /// </summary>
        public static HashSet<int> Voters = new HashSet<int>();
        private static TelegramBotClient _Bot;

        /// <summary>
        /// Начало голосования. В зависимости от ChatMemberStatus Высчитывается VotingStatus, либо, если VotingStatus != null, возвращает собщение о уже проходящем голосовании
        /// </summary>
        /// <param name="_Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task StartVoting(TelegramBotClient _Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            User mark = message.ReplyToMessage.From;
            User initiator = message.From;
            ChatMember tgMark = await _Bot.GetChatMemberAsync(chatId, mark.Id);
            ChatMember tgInitiator = await _Bot.GetChatMemberAsync(chatId, initiator.Id);

            if (VotingStatus == null)
            {
                switch (tgInitiator.Status)
                {
                    case ChatMemberStatus.Creator when tgMark.Status == ChatMemberStatus.Administrator:
                        VotingStatus = true;
                        TargetUserId = Convert.ToInt32(mark.Id);
                        Voters.Add(Convert.ToInt32(initiator.Id));
                        await _Bot.SendTextMessageAsync(chatId, $"Голосование против администратора {mark} Начато. \nГолосовать могут только пользователи, достигшие звания младшего лейтенанта. \nГолосуйте с помощью /vote");
                        break;

                    case ChatMemberStatus.Administrator or ChatMemberStatus.Member or ChatMemberStatus.Creator when tgMark.Status == ChatMemberStatus.Member:
                        VotingStatus = false;
                        TargetUserId = Convert.ToInt32(mark.Id);
                        Voters.Add(Convert.ToInt32(initiator.Id));
                        await _Bot.SendTextMessageAsync(chatId, $"Начало голосования против {mark} начато. \nГолосовать с помощью /vote.");
                        break;
                    case ChatMemberStatus.Member when tgMark.Status == ChatMemberStatus.Administrator || tgMark.Status == ChatMemberStatus.Creator:
                        await _Bot.SendTextMessageAsync(chatId, "Вы не можете начать голосование против администратора/создателя");
                        break;
                }
            }
            else
            {
                await _Bot.SendTextMessageAsync(chatId, "Голосование уже идёт.");
            }
        }
        /// <summary>
        /// Процесс голосвания. Производится вычисление необходимого кол-ва голосов при разных статусах голосования.
        /// </summary>
        /// <param name="_Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task VotingProcessing(TelegramBotClient _Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            int needed = 0;
            int memberCount = await _Bot.GetChatMemberCountAsync(chatId);
            UserProfile currentVoter = FileProfiles.Get(message.From.Username);
            int currentVoterId = Convert.ToInt32(message.From.Id);
            switch (VotingStatus)
            {
                case null:
                    await _Bot.SendTextMessageAsync(chatId, "Нет активного голосования");
                    break;
                case false:
                    if (!Voters.Contains(Convert.ToInt32(currentVoterId)))
                    {
                        if (memberCount < MAX_VOTERS)
                        {
                            needed = memberCount / 2;
                        }
                        else
                        {
                            needed = VOTERS_LIMIT;
                        }
                        Voters.Add(currentVoterId);
                        await _Bot.SendTextMessageAsync(chatId, $"Вы проголосвали. Статус голосования: {Voters.Count}/{needed}");
                    }
                    else
                    {
                        await _Bot.SendTextMessageAsync(chatId, "Вы уже голосовали");
                    }
                    if (Voters.Count >= needed)
                    {
                        await _Bot.BanChatMemberAsync(chatId, TargetUserId);
                        await _Bot.SendTextMessageAsync(chatId, "Пользователь забанен.");
                        Voters.Clear();
                        TargetUserId = 0;
                        VotingStatus = null;
                    }
                    break;

                case true:
                    #region GetAllRank6Users
                    int counter = 0;
                    FileProfiles.GetAll();
                    foreach (var user in FileProfiles.GetAll())
                    {
                        if (user.Value.currentRank >= 6)
                        {
                            counter++;
                        }
                    }
                    needed = counter / 3;
                    #endregion
                    if (currentVoter.currentRank >= 6 && !Voters.Contains(currentVoterId))
                    {
                        Voters.Add(currentVoterId);
                        await _Bot.SendTextMessageAsync(chatId, $"Вы проголосвали. Статус голосования: {Voters.Count}/{needed}");
                    }
                    else
                    {
                        await _Bot.SendTextMessageAsync(chatId, "Ваш ранг не подходит для голосования.");
                    }
                    if (Voters.Count >= needed)
                    {
                        await _Bot.BanChatMemberAsync(chatId, TargetUserId);
                        await _Bot.SendTextMessageAsync(chatId, "Администратор забанен.");
                        Voters.Clear();
                        TargetUserId = 0;
                        VotingStatus = null;
                    }
                    break;
            }
        }
        /// <summary>
        /// Метод отмены голосования, допускающий отмену пользователем creator всех голосований, а пользователем admin - отмену голосвания против member
        /// </summary>
        /// <param name="_Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task CancelVoting(TelegramBotClient _Bot, Telegram.Bot.Types.Message message, string chatId)
        {
            ChatMember member = await _Bot.GetChatMemberAsync(chatId, message.From.Id);
            switch (VotingStatus)
            {
                case null:
                    await _Bot.SendTextMessageAsync(chatId, "Нет голосования в текущий момент. \nНачать можно с помощью /voteban");
                    break;
                case false:
                    if (member.Status == ChatMemberStatus.Administrator || member.Status == ChatMemberStatus.Creator && message.From.Id != TargetUserId)
                    {
                        Voters.Clear();
                        TargetUserId = 0;
                        await _Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
                        VotingStatus = null;
                    }
                    else
                    {
                        await _Bot.SendTextMessageAsync(chatId, "Вы не можете отменить голосование.");
                    }
                    break;
                case true:
                    if (member.Status == ChatMemberStatus.Creator)
                    {
                        Voters.Clear();
                        TargetUserId = 0;
                        await _Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
                        VotingStatus = null;
                    }
                    else
                    {
                        await _Bot.SendTextMessageAsync(chatId, "Вы не можете отменить голосование");
                    }
                    break;

            }
        }
    }
}