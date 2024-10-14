using static PeaceDaBoll.Profiles.ProfileLogicXYI.CustomLogicProfiles;

namespace PeaceDaBoll.Profiles
{
    public class UserProfileLogic
    {
        public static readonly string[] Ranks = ["Рядовой", "Ефрейтор", "Сержант", "Старшина", "Прапорщик", "Лейтенант", "Капитан", "Майор", "Полковник", "Генерал майор", "Генерал лейтенант", "Генерал полковник", "Генерал армии"]; //Ранги
        public static async Task AddUser(string name) //Создание профиля пользователя
        {
            UserProfile user = new();
            AddNewProfile(user, name);
        }
        public static async Task AddMessageCount(string name) => EditProfile(name, ProfileValueType.quantityMessage, Convert.ToString(GetProfile(name).quantityMessage + 1));
        //Добавить предупреждение у пользователя
        public static async Task AddWarningToUser(string name) => EditProfile(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(GetProfile(name).quantityUserWarnings) + 1));
        
        //Уменьшить предупреждения у пользователя
        public static async Task ReduceWarningToUser(string name) => EditProfile(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(GetProfile(name).quantityUserWarnings) - 1));
        
        //Убрать предупреждения у пользователя
        public static async Task RemoveWarningToUser(string name) => EditProfile(name, ProfileValueType.quantityUserWarnings, "0");

        //Изменения очков пользователя
        public static async Task ChangePointsUser(string name, int value) => EditProfile(name, ProfileValueType.quantityUserPoints, Convert.ToString(GetProfile(name).quantityUserPoints + value));
        
        //Добавление/изменение пользователю второго ника
        public static async Task EditCustomUsername(string name, string value) => EditProfile(name, ProfileValueType.CustomName, value);
        
        //Присвоение ранга
        public static async Task GiveRank(string name, int value) => EditProfile(name, ProfileValueType.currentRank, value.ToString());

        //Присвоение последней активности пользователя
        public static async Task SetLastDate(string name, DateTime time) => EditProfile(name, ProfileValueType.LastActivity, time.ToString());
        
        //Получение данных профиля пользователя и их компоновки в нужно виде для вывода сообщения в чате
        public static async Task <string> ViewProfile(string name)
        {
            UserProfile user = GetProfile(name);
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

        public static string GetRank(int messages)
        {
            string rank = "";
            if (messages <= 100)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 10040)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 10500)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 10300)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 10100)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            else if (messages >= 101 && messages <= 1000)
                rank = "рядовой";
            return rank;
        }
    }
}

//Это ебучий ужас, а не код