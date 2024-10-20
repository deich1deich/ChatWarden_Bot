using System.Text.RegularExpressions;
using File = System.IO.File;

namespace PeaceDaBoll.Profiles
{
    internal class FileProfiles
    {
        private static readonly string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\Profiles.xyi";

        public static Dictionary<string, UserProfile> Profiles;

        public enum ProfileValueType
        {
            CustomName,
            currentRank,
            quantityMessage,
            LastActivity,
            quantityUserWarnings,
            quantityUserPoints
        }

        /// <summary>
        /// Добавляет в файл новый профиль с указанным ником.
        /// </summary>
        /// <param name="user">Все параметры профиля.</param>
        /// <param name="name">Ник юзера без собаки.</param>
        public static void Add(UserProfile user, string name)
        {
            File.AppendAllText(
                path,
                $"@\"{name}\"" + Environment.NewLine +
                $"{{" + Environment.NewLine +
                $"{name} CustomName: <{user.CustomName}>" + Environment.NewLine +
                $"{name} currentRank: <{user.currentRank}>" + Environment.NewLine +
                $"{name} quantityMessage: <{user.quantityMessage}>" + Environment.NewLine +
                $"{name} FirstActivity: <{user.FirstActivity}>" + Environment.NewLine +
                $"{name} LastActivity: <{user.LastActivity}>" + Environment.NewLine +
                $"{name} quantityUserWarnings: <{user.quantityUserWarnings}>" + Environment.NewLine +
                $"{name} quantityUserPoints: <{user.quantityUserPoints}>" + Environment.NewLine +
                $"}}" + Environment.NewLine + Environment.NewLine);
        }
        
        /// <summary>
        /// Возвращает данные конкретного профиля.
        /// </summary>
        /// <param name="Username">Ник юзера без собаки.</param>
        /// <returns>UserProfile, содержащий в себе все параметры из файла.</returns>
        public static UserProfile Get(string Username)
        {
            string text = File.ReadAllText(path);
            UserProfile profile = new UserProfile()
            {
                Username = Username,
                CustomName = Regex.Match(text, @$"(?<={Username} CustomName: <)[A-Za-zА-Яа-я0-9]+(?=>)").Value,
                currentRank = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} currentRank: <)[0-9]+(?=>)").Value),
                quantityMessage = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityMessage: <)[0-9]+(?=>)").Value),
                FirstActivity = Regex.Match(text, @$"(?<={Username} FirstActivity: <)[0-9]+\.[0-9]+\.[0-9]{{4}} [0-9]+:[0-9]+:[0-9]+(?=>)").Value,
                LastActivity = Regex.Match(text, @$"(?<={Username} LastActivity: <)[0-9]+\.[0-9]+\.[0-9]{{4}} [0-9]+:[0-9]+:[0-9]+(?=>)").Value, 
                quantityUserWarnings = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityUserWarnings: <)[0-9]+(?=>)").Value),
                quantityUserPoints = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityUserPoints: <)[0-9]+(?=>)").Value)
            };
            return profile;
        }

        /// <summary>
        /// Возвращает все существующие профили.
        /// </summary>
        /// <returns>Dictionary<username, UserProfile> username - ник без собаки, UserProfile - объект, содержащий в себе все параметры из файла.</returns>
        public static Dictionary<string, UserProfile> GetAll()
        {
            Profiles = new Dictionary<string, UserProfile>();
            string text = File.ReadAllText(path);
            foreach (Match item in Regex.Matches(text, @"(?<=@"")[A-Za-z0-9]+(?="")"))
            {
                string _Username = item.Value;
                if (!Profiles.ContainsKey(_Username))
                {
                    Profiles.Add(_Username, Get(_Username));
                }
            }
            return Profiles;
        }

        /// <summary>
        /// Метод предназначен для редактирования отдельных параметров из файла Profiles.
        /// </summary>
        /// <param name="Username">Username без собаки.</param>
        /// <param name="type">Изменяемый параметр из файла.</param>
        /// <param name="value">Новое значение параметра из файла.</param>
        public static void Edit(string Username, ProfileValueType type, string value) // Редактирует в файле параметр, указанный в ProfileValueType
        {
            string text = File.ReadAllText(path);
            string NewValue = "";
            switch (type)
            {
                case ProfileValueType.CustomName:
                    NewValue = $"{Username} CustomName: <{value}>";
                    text = text.Replace($"{Username} CustomName: <{Get(Username).CustomName}>", NewValue);
                    break;
                case ProfileValueType.currentRank:
                    NewValue = $"{Username} currentRank: <{value}>";
                    text = text.Replace($"{Username} currentRank: <{Get(Username).currentRank}>", NewValue);
                    break;
                case ProfileValueType.quantityMessage:
                    NewValue = $"{Username} quantityMessage: <{value}>";
                    text = text.Replace($"{Username} quantityMessage: <{Get(Username).quantityMessage}>", NewValue);
                    break;
                case ProfileValueType.LastActivity:
                    NewValue = $"{Username} LastActivity: <{value}>";
                    text = text.Replace($"{Username} LastActivity: <{Get(Username).LastActivity}>", NewValue);
                    break;
                case ProfileValueType.quantityUserWarnings:
                    NewValue = $"{Username} quantityUserWarnings: <{value}>";
                    text = text.Replace($"{Username} quantityUserWarnings: <{Get(Username).quantityUserWarnings}>", NewValue);
                    break;
                case ProfileValueType.quantityUserPoints:
                    NewValue = $"{Username} quantityUserPoints: <{value}>";
                    text = text.Replace($"{Username} quantityUserPoints: <{Get(Username).quantityUserPoints}>", NewValue);
                    break;
            }
            File.WriteAllText(path, text);
        }
        /// <summary>
        /// Проверяет, существует ли профиль в файле.
        /// </summary>
        /// <param name = "Username">Username без собаки.</param>
        /// <returns>Профиль существует: (true/false).</returns>
        public static bool Exists(string Username) => GetAll().ContainsKey(Username);
    }
}