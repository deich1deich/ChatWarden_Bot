using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaceDaBoll.Profiles
{
    public class UserProfile(string UserName)
    {
        public string Username { get; set; } = UserName ?? "";
        public string SecondUsername { get; set; } = "";
        public int currentRank { get; set; } = 0;
        public int quantityMessage { get; set; } = 0;
        public DateTime FirstActivity { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = default(DateTime);
        public int quantityUserWarnings { get; set; } = 0;
        public int quantityUserPoints { get; set; } = 0;
    }
}
