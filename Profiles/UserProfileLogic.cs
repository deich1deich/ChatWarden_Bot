using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PeaceDaBoll.Profiles.ProfileLogicXYI;
using File = System.IO.File;
using Telegram.Bot;

namespace PeaceDaBoll.Profiles
{
    public class UserProfileLogic
    {
        //private static Dictionary<string, UserProfile> profiles = [];
        public static readonly string[] Ranks = ["1", "2", "3", "4"];

        public static async Task AddUser(string name) //Создание профиля пользователя
        {
            UserProfile user = new();
            CustomLogicProfiles.AddNewProfile(user, name);
        }
        //Добавить предупреждение у пользователя
        public async Task AddWarningToUser(string name) => CustomLogicProfiles.GetProfile(name).quantityUserWarnings += 1;

        //Уменьшить предупредения у пользователя
        public async Task ReduceWarningToUser(string name, int count) => CustomLogicProfiles.GetProfile(name).quantityUserWarnings = CustomLogicProfiles.GetProfile(name).quantityUserWarnings <= 0 ? 0 : CustomLogicProfiles.GetProfile(name).quantityUserWarnings - count;

        //Убрать предупреждения у пользователя
        public async Task RemoveWarningToUser(string name) => CustomLogicProfiles.GetProfile(name).quantityUserWarnings = 0;

        //Добавить очки пользователю
        public async Task AddPointsToUser(string name, int count) => CustomLogicProfiles.GetProfile(name).quantityUserPoints += count;

        // tyt kakayato hernya
        public static string ViewProfile(string name)
        {
            UserProfile user = CustomLogicProfiles.GetProfile(name);
            string text = $"Профиль: {user.Username.Substring(1)}{user.CustomName}" +
            $"Звание: {user.currentRank}" +
            $"Кол-во отправленных сообщений: {user.quantityMessage}" +
            $"Кол-во полученых предупреждений: {user.quantityUserWarnings}" +
            $"Последняя активность: {user.LastActivity}" +
            $"Первая активность: {user.FirstActivity}" +
            $"Баллы на счету: {user.quantityUserPoints}" /*+*/
            /*$""*/;
            MessageBox.Show("Сделано!");
            return text;
        }
    }
}
