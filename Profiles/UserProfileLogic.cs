using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaceDaBoll.Profiles
{
    public class UserProfileLogic
    {
        private Dictionary<string, UserProfile> profiles = [];
        public readonly string[] Ranks = ["1", "2", "3", "4"];

        public UserProfileLogic()
        {

        }

        public void AddUser(string name, string datenow) //Создание профиля пользователя
        {
            UserProfile user = new(name, datenow);
            profiles.Add(name, user);
        }
        //Добавить предупреждение у пользователя
        public void AddWarningToUser(string name) => profiles[name].quantityUserWarnings += 1;

        //Уменьшить предупредения у пользователя
        public void ReduceWarningToUser(string name, int count) => profiles[name].quantityUserWarnings = profiles[name].quantityUserWarnings <= 0 ? 0 : profiles[name].quantityUserWarnings - count;

        //Убрать предупреждения у пользователя
        public void RemoveWarningToUser(string name) => profiles[name].quantityUserWarnings = 0;

        //Добавить очки пользователю
        public void AddPointsToUser(string name, int count) => profiles[name].quantityUserPoints += count;

        // tyt kakayato hernya
        public string GetProfile(string name) =>
               $"Профиль: {profiles[name].Username.Substring(1)} {profiles[name].SecondUsername}" + Environment.NewLine +
               $"Звание: {Ranks[profiles[name].currentRank]}" + Environment.NewLine +
               $"Кол-во отправленных сообщений: {profiles[name].quantityMessage}" + Environment.NewLine +
               $"Кол-во полученых предупреждений: {profiles[name].quantityUserWarnings}" + Environment.NewLine +
               $"Последняя активность: {profiles[name].LastActivity}" + Environment.NewLine +
               $"Первая активность: {profiles[name].FirstActivity}" + Environment.NewLine +
               $"Баллы на счету: {profiles[name].quantityUserPoints}" + Environment.NewLine +
               $"" + Environment.NewLine;
    }
}
