using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace PeaceDaBoll.Messages
{
    internal class Voting
    {
        public int? TargetUserId { get; set; }
        public long? CreatorUserId { get; set; }
        public HashSet<int> Voters { get; set; }
        public int VotersCount { get; set; }
        private const int MAX_VOTES = 10;

        public Voting(int targetUserId, int creatorUserId)
        {
            TargetUserId = targetUserId;
            CreatorUserId = creatorUserId;
            Voters = new HashSet<int>();
            VotersCount = 1;
        }


        private static Voting? currentVoting;
        public static async Task StartVoting(string chatId, Telegram.Bot.Types.Message message, long userId, TelegramBotClient Bot)
        {
            if (currentVoting != null)
            {
                await Bot.SendTextMessageAsync(chatId, "Уже идет голосование!");
                return;
            }
            var userToBan = (long?)message.ReplyToMessage?.From.Id;
            var userToBanUsername = message.ReplyToMessage?.From.Username;
            var chatMember = await Bot.GetChatMemberAsync(chatId, (long)userToBan);
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

        public static async Task VotingProcessing(string chatId, int userid, TelegramBotClient Bot)
        {
            int memberCount = await Bot.GetChatMemberCountAsync(chatId);
            int needed = 0;
            if (memberCount > 20)
            {
                needed = MAX_VOTES;
            }
            else if (memberCount < 20)
            {
                needed = memberCount / 2; // 50% of group
            }
            if (currentVoting != null && !currentVoting.Voters.Contains(userid))
            {
                currentVoting.VotersCount++;
                await Bot.SendTextMessageAsync(chatId, $"Вы проголосовали. Статус голосования: {currentVoting.VotersCount}/{needed}");
                return;
            }
            else if (currentVoting != null && currentVoting.Voters.Contains(userid))
            {
                await Bot.SendTextMessageAsync(chatId, "Вы уже голосовали.");
                return;
            }
            else if (currentVoting == null)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет активного голосования.");
                return;
            }
            if (currentVoting != null && currentVoting.VotersCount >= needed)
            {
                await Bot.SendTextMessageAsync(chatId, "По итогам голосования пользователь забанен.");
                await Bot.BanChatMemberAsync(chatId, (long)currentVoting.TargetUserId);
                currentVoting = null;
            }
        }
        public static async Task CancelVoting(string chatId, ITelegramBotClient Bot)
        {
            if (currentVoting == null)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет активного голосования.");
                return;
            }

            currentVoting = null;
            await Bot.SendTextMessageAsync(chatId, "Голосование отменено.");
        }
    }
}


