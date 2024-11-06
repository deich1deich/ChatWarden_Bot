using PeaceDaBoll.Profiles;
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
        public static long TargetUserId = 0;

        /// <summary>
        /// null - нет голосования, false - голосование против ChatMemberStatus.Member, true - голосование против ChatMemberStatus.Admin.
        /// </summary>
        public static bool? VotingStatus = null;

        /// <summary>
        /// Хранит ID пользователей, проголосовавших за бан.
        /// </summary>
        public static HashSet<long> Voters = new HashSet<long>();

        /// <summary>
        /// Начало голосования. В зависимости от ChatMemberStatus Высчитывается VotingStatus, либо, если VotingStatus != null, возвращает собщение о уже проходящем голосовании
        /// </summary>
        /// <param name="Bot">Закрытый в классе Telegram.Bot</param>
        /// <param name="message">Сообщение, при котором вызывается этот метод</param>
        /// <param name="chatId">Id текущего чата</param>
        /// <returns></returns>
        public static async Task StartVoting(ITelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
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
                        int counter = 0;
                        foreach (var user in FileProfiles.GetAll())
                        {
                            if (user.Value.currentRank >= 6)
                            {
                                counter++;
                            }
                        }
                        if (counter >= MIN_VOTERS)
                        {
                            VotingStatus = true;
                            TargetUserId = mark.Id;
                            Voters.Add(initiator.Id);
                            await Bot.SendTextMessageAsync(chatId, $"Голосование против {mark} создано. \nГолосовать могут только пользователи, достигшие звания младшего лейтенанта. \nГолосуйте с помощью /vote");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(chatId, "Пользователей, имеющих необходимый ранг, недостаточно. голосование не начато.");
                        }
                        break;
                    case ChatMemberStatus.Administrator or ChatMemberStatus.Member or ChatMemberStatus.Creator when tgMark.Status == ChatMemberStatus.Member:
                        VotingStatus = false;
                        TargetUserId = mark.Id;
                        Voters.Add(initiator.Id);
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
        public static async Task VotingProcessing(ITelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
        {

            int countToBan = 0;
            int memberCount = await Bot.GetChatMemberCountAsync(chatId);
            if (!(memberCount < MIN_VOTERS))
            {
                UserProfile currentVoter = FileProfiles.Get(message.From.Username);
                long currentVoterId = message.From.Id;
                if (VotingStatus == null)
                {
                    await Bot.SendTextMessageAsync(chatId, "Нет активного голосования.");
                }
                else if (VotingStatus == false)
                {
                    if (memberCount < MAX_VOTERS && memberCount > MIN_VOTERS)
                    {
                        countToBan = memberCount / 2;
                    }
                    else if (memberCount > MAX_VOTERS)
                    {
                        countToBan = VOTERS_LIMIT;
                    }
                    if (!Voters.Contains(Convert.ToInt32(currentVoterId)))
                    {
                        Voters.Add(currentVoterId);
                        await Bot.SendTextMessageAsync(chatId, $"Вы проголосовали. Статус голосования: {Voters.Count}/{countToBan}.");
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(chatId, "Вы уже голосовали.");
                    }
                    if (Voters.Count >= countToBan)
                    {
                        await Bot.BanChatMemberAsync(chatId, TargetUserId, untilDate: DateTime.Now.AddDays(7));
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
                    if (counter <= MIN_VOTERS * 2)
                    {
                        countToBan = counter / 2;
                    }
                    else if (counter >= VOTERS_LIMIT)
                    {
                        countToBan = counter / 3;
                    }
                    else if (counter <= MIN_VOTERS)
                    {
                        countToBan = -1;
                    }
                    #endregion
                    if (countToBan != -1)
                    {
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
                            await Bot.BanChatMemberAsync(chatId, TargetUserId, untilDate: DateTime.Now.AddDays(7));
                            await Bot.SendTextMessageAsync(chatId, "Администратор забанен.");
                            Voters.Clear();
                            TargetUserId = 0;
                            VotingStatus = null;
                        }
                    }
                    else
                    {
                        Voters.Clear();
                        TargetUserId = 0;
                        VotingStatus = null;
                        await Bot.SendTextMessageAsync(chatId, "Голосование отменено: пользователей, имеющих необходимый ранг, недостаточно.");
                    }
                    
                    
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
        public static async Task CancelVoting(ITelegramBotClient Bot, Telegram.Bot.Types.Message message, string chatId)
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