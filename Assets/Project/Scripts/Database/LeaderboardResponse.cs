using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Scripts.Database
{
    internal class LeaderboardResponse
    {
        public string Status { get; set; }
        public LeaderboardEntry[] Leaderboard { get; set; }
    }
}
