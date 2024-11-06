using System.Text.RegularExpressions;
using static PeaceDaBoll.Profiles.FileProfiles;
namespace PeaceDaBoll.Profiles
{
    public class UserStat
    {
        public static readonly List<string> Ranks = ["Рядовой", "Ефрейтор","Младший сержант", "Сержант", "Старший сержант", "Старшина", "Прапорщик", "Старший прапорщик", "Младший лейтенант", "Лейтенант", "Старший лейтенант", "Капитан", "Майор", "Подполковник", "Полковник", "Генерал", "Генерал-майор", "Генерал-лейтенант", "Генерал полковник", "Генерал армии", "y6uBaLLIKa"]; //Ранги
        public static readonly List<int> Required = [100, 500, 1200, 2000, 2500, 5000, 6000, 9000, 12000, 14000, 20000, 25000, 32000, 40000, 50000, 60000, 70000, 80000, 90000, 100000, 110000, 120000, 150000];

        /// <summary>
        /// Создание профиля пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        public static void AddUser(string name)
        {
            UserProfile user = new();
            Add(user, name);
        }

        /// <summary>
        /// Поднятие ранга
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        public static void RankUp(string name)
        {
            var user = Get(name);
            if (Required.Contains(user.quantityMessage))
            {
                ChangeRank(name, Required.IndexOf(user.quantityMessage) + 1);
            };
        }

        /// <summary>
        /// Прибавление сообщений к общему количеству написанных сообщений пользователем
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        public static void ChangeMessageCount(string name) => Edit(name, ProfileValueType.quantityMessage, Convert.ToString(Get(name).quantityMessage + 1));

        /// <summary>
        /// Добавить предупреждение у пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="messageText">Присваиваемое значение предупреждений</param>
        public static void ChangeWarning(string name, string messageText) => Edit(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(Get(name).quantityUserWarnings) + Convert.ToInt32(Regex.Match(messageText, "(?<=/warn )[-]?[0-5]{1,1}").Value)));

        /// <summary>
        /// Изменения очков пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="messageText">Присваиваемое значение очков</param>
        public static void ChangePoints(string name, string messageText) => Edit(name, ProfileValueType.quantityUserPoints, Convert.ToString(Get(name).quantityUserPoints + Convert.ToInt32(Regex.Match(messageText, "(?<=/point )[-]?[0-9]+").Value)));

        /// <summary>
        /// Добавление/изменение пользователю второго ника
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="messageText">Присваиваемый второй ник</param>
        public static void ChangeCustomNickname(string name, string messageText) => Edit(name, ProfileValueType.CustomName, Get(name).CustomName = Regex.Match(messageText, @"(?<=/editname )[A-Za-zА-Яа-я0-9_]+").Value);

        /// <summary>
        /// Присвоение ранга пользователю
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="value">Присваиваемый ранг</param>
        public static void ChangeRank(string name, int value) => Edit(name, ProfileValueType.currentRank, CheckEnterRank(value));

        /// <summary>
        /// Присвоение последней активности пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="time">Текущее время</param>
        public static void ChangeLastDate(string name, DateTime time) => Edit(name, ProfileValueType.LastActivity, time.ToString());

        /// <summary>
        /// Получение данных профиля пользователя и их компоновки в нужно виде для вывода сообщения в чате
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <returns>Строка со всей информацией о профиле</returns>
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

        /// <summary>
        /// Ограничение вводимого ранга в пределах списка рангов
        /// </summary>
        /// <param name="rankEnter">Вводимый ранг</param>
        /// <returns></returns>
        private static string CheckEnterRank(int rankEnter)
        {
            if (0 > rankEnter)
            {
                return "0";
            }
            else if (Convert.ToInt32(rankEnter) > Ranks.Count)
            {
                return (Ranks.Count - 1).ToString();
            }
            else
            {
                return rankEnter.ToString();
            }
        }
    }
}