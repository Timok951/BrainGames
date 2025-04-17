using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Project.Scripts.Database
{
    internal class LoginResponse
    {
        public string Status { get; set; }
        public string Nick { get; set; }
        public string Email { get; set; }
        public int InfinityScore { get; set; }
        public int ColorsortLevels { get; set; }
        public int LevelScore { get; set; }
        public int ConnectLevels { get; set; }
        public int PipesLevels { get; set; }
    }
}
