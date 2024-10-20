using System.Text.RegularExpressions;
using static PeaceDaBoll.Profiles.FileProfiles;
namespace PeaceDaBoll.Profiles
{
    public class UserStat
    {
        public static readonly List<string> Ranks = ["Рядовой", "Ефрейтор", "Сержант", "Старшина", "Прапорщик", "Старший прапорщик", "Младший Лейтенант", "Лейтенант", "Старший лейтенант", "Майор", "Подполковник", "Полковник", "Генерал майор", "Генерал лейтенант", "Генерал полковник", "Генерал армии"]; //Ранги
        public static readonly List<int> Required = [100, 500, 1200, 2000, 2500, 5000, 6000, 9000, 12000, 14000, 20000, 25000, 32000, 40000, 50000];

        public static void AddUser(string name) // Создание профиля пользователя
        {
            UserProfile user = new();
            Add(user, name);
        }

        public static void RankUp(string name)
        {
            var user = Get(name);
            if (Required.Contains(user.quantityMessage))
            {
                ChangeRank(name, Required.IndexOf(user.quantityMessage) + 1);
            };
        }
        public static void ChangeMessageCount(string name) => Edit(name, ProfileValueType.quantityMessage, Convert.ToString(Get(name).quantityMessage + 1));
        //Добавить предупреждение у пользователя
        public static void ChangeWarning(string name, string messageText) => Edit(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(Get(name).quantityUserWarnings) + Convert.ToInt32(Regex.Match(messageText, "(?<=/warn )[-]?[0-9]+").Value)));

        //Изменения очков пользователя
        public static void ChangePoints(string name, string messageText) => Edit(name, ProfileValueType.quantityUserPoints, Convert.ToString(Get(name).quantityUserPoints + Convert.ToInt32(Regex.Match(messageText, "(?<=/point )[-]?[0-9]+").Value)));
        
        //Добавление/изменение пользователю второго ника
        public static void ChangeCustomNickname(string name, string messageText) => Edit(name, ProfileValueType.CustomName, Get(name).CustomName = Regex.Match(messageText, @"(?<=/editname )[A-Za-zА-Яа-я0-9]+").Value);
        
        //Присвоение ранга
        public static void ChangeRank(string name, int value) => Edit(name, ProfileValueType.currentRank, value.ToString());

        //Присвоение последней активности пользователя
        public static void ChangeLastDate(string name, DateTime time) => Edit(name, ProfileValueType.LastActivity, time.ToString());
        
        //Получение данных профиля пользователя и их компоновки в нужно виде для вывода сообщения в чате
        public static string ViewProfile(string name)
        {
            UserProfile user = Get(name);
            string text = 
            $"Профиль: {user.Username} {user.CustomName}" + Environment.NewLine +
            $"Звание: {Ranks[user.currentRank]}" + Environment.NewLine +
            $"Кол-во отправленных сообщений: {user.quantityMessage}" + Environment.NewLine +
            $"Кол-во полученных предупреждений: {user.quantityUserWarnings}/5" + Environment.NewLine +
            $"Последняя активность: {user.LastActivity}" + Environment.NewLine +
            $"Первая активность: {user.FirstActivity}" + Environment.NewLine +
            $"Баллы на счету: {user.quantityUserPoints}";
            return text;
        }
    }
}