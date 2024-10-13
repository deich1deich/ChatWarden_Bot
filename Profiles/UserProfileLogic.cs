using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PeaceDaBoll.Profiles.ProfileLogicXYI;
using File = System.IO.File;
using Telegram.Bot;
using static PeaceDaBoll.Profiles.UserProfileLogic;

namespace PeaceDaBoll.Profiles
{
    public class UserProfileLogic
    {
        public static readonly string[] Ranks = ["Рядовой", "Ефрейтор", "Сержант", "Старшина", "Прапорщик", "Лейтенант", "Капитан", "Майор", "Полковник", "Генерал майор", "Генерал лейтенант", "Генерал полковник", "Генерал армии"]; //Ранги
        public enum ProfileValueType
        {
            CustomName,
            currentRank,
            quantityMessage,
            LastActivity,
            quantityUserWarnings,
            quantityUserPoints
        }
        public static async Task AddUser(string name) //Создание профиля пользователя
        {
            UserProfile user = new();
            CustomLogicProfiles.AddNewProfile(user, name);
        }

        public static async Task AddMessageCount(string name) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityMessage, Convert.ToString(CustomLogicProfiles.GetProfile(name).quantityMessage + 1));
        //Добавить предупреждение у пользователя
        public static async Task AddWarningToUser(string name) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(CustomLogicProfiles.GetProfile(name).quantityUserWarnings) + 1));
        
        //Уменьшить предупреждения у пользователя
        public static async Task ReduceWarningToUser(string name) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserWarnings, Convert.ToString(Convert.ToInt32(CustomLogicProfiles.GetProfile(name).quantityUserWarnings) - 1));
        
        //Убрать предупреждения у пользователя
        public static async Task RemoveWarningToUser(string name) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserWarnings, "0");

        //Изменения очков пользователя
        public static async Task ChangePointsUser(string name, int value) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserPoints, Convert.ToString(CustomLogicProfiles.GetProfile(name).quantityUserPoints + value));
        
        //Добавление/изменение пользователю второго ника
        public static async Task EditCustomUsername(string name, string value) => CustomLogicProfiles.EditProfile(name, ProfileValueType.CustomName, value);
        
        //Присвоение ранга
        public static async Task GiveRank(string name, int value) => CustomLogicProfiles.EditProfile(name, ProfileValueType.currentRank, value.ToString());

        //Присвоение последней активности пользователя
        public static async Task SetLastDate(string name, DateTime time) => CustomLogicProfiles.EditProfile(name, ProfileValueType.LastActivity, time.ToString());
        
        //Получение данных профиля пользователя и их компоновки в нужно виде для вывода сообщения в чате
        public static async Task <string> ViewProfile(string name)
        {
            UserProfile user = CustomLogicProfiles.GetProfile(name);
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

//Это ебучий ужас, а не код
//Добавить очки пользователю
//public static void AddPointsToUser(string name, int value) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserPoints, Convert.ToString(CustomLogicProfiles.GetProfile(name).quantityUserPoints + value
////Отнять очки пользователя
//public static void TakePointsFromUser(string name, int value) => CustomLogicProfiles.EditProfile(name, ProfileValueType.quantityUserPoints, Convert.ToString(CustomLogicProfiles.GetProfile(name).quantityUserPoints - value));