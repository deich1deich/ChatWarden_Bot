﻿using Newtonsoft.Json;
using static PeaceDaBoll.Profiles.ProfileLogicXYI.CustomLogicProfiles;

namespace PeaceDaBoll.Profiles
{
    public class UserProfileLogic
    {
        public static readonly List<string> Ranks = ["Рядовой", "Ефрейтор", "Сержант", "Старшина", "Прапорщик", "Лейтенант", "Капитан", "Майор", "Полковник", "Генерал майор", "Генерал лейтенант", "Генерал полковник", "Генерал армии"]; //Ранги
        public static readonly List<int> Required = [100, 500, 1200, 2000, 2500, 5000, 6000, 9000, 12000, 14000, 20000, 25000, 32000, 40000, 50000];

        public static async Task AddUser(string name) //Создание профиля пользователя
        {
            UserProfile user = new();
            AddNewProfile(user, name);
        }

        public static async Task RankUp(string name)
        {
            var user = GetProfile(name);
            int q = 0;
            if (Required.Contains(user.quantityMessage))
            {
                q = Required.IndexOf(user.quantityMessage);
                await GiveRank(name, q + 1);
            };
        }

        public static async Task AddMessageCount(string name) => EditProfile(name, ProfileValueType.quantityMessage, Convert.ToString(GetProfile(name).quantityMessage + 1));
        //Добавить предупреждение у пользователя
        public static async Task AddWarningToUser(string name) => EditProfile(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(GetProfile(name).quantityUserWarnings) + 1));

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
    }
}