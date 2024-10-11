using System.Text.RegularExpressions;
using File = System.IO.File;

namespace PeaceDaBoll.Profiles.ProfileLogicXYI
{
    internal class CustomLogicProfiles
    {
        private static readonly string path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\Profiles.xyi";

        public static Dictionary<string, UserProfile> Profiles;
        public enum ProfileValueType
        {
            CustomName,
            currentRank,
            quantityMessage,
            FirstActivity,
            LastActivity,
            quantityUserWarnings,
            quantityUserPoints
        }

        public static void AddNewProfile(UserProfile user, string name)
        {
            if (!ProfileExists(name))
            {
                File.AppendAllText(path,
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
        }
        public static UserProfile GetProfile(string Username)
        {
            string text = File.ReadAllText(path);
            UserProfile profile = new UserProfile()
            {
                Username = Username,
                CustomName = Regex.Match(text, @$"(?<={Username} CustomName: <)[A-Za-z0-9]+(?=>)").Value,
                currentRank = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} currentRank: <)[0-9]+(?=>)").Value),
                quantityMessage = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityMessage: <)[0-9]+(?=>)").Value), // 10.10.2024 16:33:33
                FirstActivity = Regex.Match(text, @$"(?<={Username} FirstActivity: <)[0-9]+.[0-9]+.[0-9]{4,4} [0-9]+:[0-9]+:[0-9]+(?=>)").Value,
                LastActivity = Regex.Match(text, @$"(?<={Username} LastActivity: <)[0-9]+.[0-9]+.[0-9]{4,4} [0-9]+:[0-9]+:[0-9]+(?=>)").Value,
                quantityUserWarnings = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityUserWarnings: <)[0-9]+(?=>)").Value),
                quantityUserPoints = Convert.ToInt32(Regex.Match(text, @$"(?<={Username} quantityUserPoints: <)[0-9]+(?=>)").Value)
            };
            return new UserProfile();
        }

        public static Dictionary<string, UserProfile> GetAllProfiles()
        {
            Profiles = new Dictionary<string, UserProfile>();
            string text = File.ReadAllText(path);
            foreach (string _Username in Regex.Matches(text, @"(?<=@"")[A-Za-z0-9]+(?="")"))
            {
                if (!Profiles.ContainsKey(_Username))
                {
                    Profiles.Add(_Username, GetProfile(_Username));
                }
            }
            return Profiles;
        }

        public static void EditProfile(string Username, ProfileValueType type, string value)
        {
            string text = File.ReadAllText(path);
            string NewValue = "";
            switch (type)
            {
                case ProfileValueType.CustomName:
                    NewValue = $"{Username} CustomName: <{value}>";
                    text = text.Replace($"{Username} CustomName: <{GetProfile(Username).CustomName}>", NewValue);
                    break;
                case ProfileValueType.currentRank:
                    NewValue = $"{Username} currentRank: <{value}>";
                    text = text.Replace($"{Username} currentRank: <{GetProfile(Username).currentRank}>", NewValue);
                    break;
                case ProfileValueType.quantityMessage:
                    NewValue = $"{Username} quantityMessage: <{value}>";
                    text = text.Replace($"{Username} quantityMessage: <{GetProfile(Username).quantityMessage}>", NewValue);
                    break;
                case ProfileValueType.FirstActivity:
                    NewValue = $"{Username} FirstActivity: <{value}>";
                    text = text.Replace($"{Username} FirstActivity: <{GetProfile(Username).FirstActivity}>", NewValue);
                    break;
                case ProfileValueType.LastActivity:
                    NewValue = $"{Username} LastActivity: <{value}>";
                    text = text.Replace($"{Username} LastActivity: <{GetProfile(Username).LastActivity}>", NewValue);
                    break;
                case ProfileValueType.quantityUserWarnings:
                    NewValue = $"{Username} quantityUserWarnings: <{value}>";
                    text = text.Replace($"{Username} quantityUserWarnings: <{GetProfile(Username).quantityUserWarnings}>", NewValue);
                    break;
                case ProfileValueType.quantityUserPoints:
                    NewValue = $"{Username} quantityUserPoints: <{value}>";
                    text = text.Replace($"{Username} quantityUserPoints: <{GetProfile(Username).quantityUserPoints}>", NewValue);
                    break;
            }
            File.WriteAllText(path, text);
        }

        public static bool ProfileExists(string Username) => GetAllProfiles().ContainsKey(Username);
    }
}