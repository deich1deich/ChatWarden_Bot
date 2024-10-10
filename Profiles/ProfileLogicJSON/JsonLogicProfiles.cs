using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PeaceDaBoll.Profiles;
using File = System.IO.File;
using Newtonsoft.Json;
using Telegram.Bot;

namespace PeaceDaBoll.Profiles.ProfileLogicJSON
{
    class ProfileJSON
    {
        public string name { get; set; }
        public UserProfile profile { get; set; }
    }
    internal class JsonLogicProfiles
    {
        public static void CreateProfile(UserProfile user)
        {
            string json = File.ReadAllText("Profiles.JSON");
            List<UserProfile> Profiles = JsonConvert.DeserializeObject<List<UserProfile>>(json);

            json = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText("Profiles.JSON",json);
        }
        //public static string[] GetProfile(string name)
        //{
            
        //}
    }
}
    }