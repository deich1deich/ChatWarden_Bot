using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaceDaBoll.Profiles
{
    public class UserProfile()
    {
        public string Username = "";
        public string CustomName = "";
        public int currentRank = 0;
        public int quantityMessage = 0;
        public string FirstActivity = DateTime.Now.ToString();
        public string LastActivity = default(DateTime).ToString();
        public int quantityUserWarnings = 0;
        public int quantityUserPoints = 0;
    }
}
