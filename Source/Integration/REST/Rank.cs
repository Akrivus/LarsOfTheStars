using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LarsOfTheStars.Source.Integration.REST
{
    public class Rank
    {
        public Entry[] Leaderboard { get; set; }
        public int YourRank { get; set; }
        public int Total { get; set; }
    }
}
