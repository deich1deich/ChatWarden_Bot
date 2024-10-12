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
        public static readonly string[] Ranks = ["slave", "average boy", "master", "boss this gym"];

        public static async Task AddUser(string name) //Создание профиля пользователя
        {
            UserProfile user = new();
            CustomLogicProfiles.AddNewProfile(user, name);
        }
        //Добавить предупреждение у пользователя
        public async Task AddWarningToUser(string name) => CustomLogicProfiles.EditProfile(name, CustomLogicProfiles.ProfileValueType.quantityUserWarnings, "1");

        //Уменьшить предупреждения у пользователя
        public async Task ReduceWarningToUser(string name, int count) => CustomLogicProfiles.GetProfile(name).quantityUserWarnings = CustomLogicProfiles.GetProfile(name).quantityUserWarnings <= 0 ? 0 : CustomLogicProfiles.GetProfile(name).quantityUserWarnings - count;

        //Убрать предупреждения у пользователя
        public async Task RemoveWarningToUser(string name) => CustomLogicProfiles.GetProfile(name).quantityUserWarnings = 0;

        //Добавить очки пользователю
        public async Task AddPointsToUser(string name, int count) => CustomLogicProfiles.GetProfile(name).quantityUserPoints += count;

        public static string ViewProfile(string name)
        {
            UserProfile user = CustomLogicProfiles.GetProfile(name);
            string text = 
            $"Профиль: {user.Username} {user.CustomName}" + Environment.NewLine +
            $"Звание: {Ranks[user.currentRank]}" + Environment.NewLine +
            $"Кол-во отправленных сообщений: {user.quantityMessage}" + Environment.NewLine +
            $"Кол-во полученных предупреждений: {user.quantityUserWarnings}" + Environment.NewLine +
            $"Последняя активность: {user.LastActivity}" + Environment.NewLine +
            $"Первая активность: {user.FirstActivity}" + Environment.NewLine +
            $"Баллы на счету: {user.quantityUserPoints}" /*+*/
            /*$""*/;
            //MessageBox.Show("Сделано!");
            return text;
        }
    }
}
