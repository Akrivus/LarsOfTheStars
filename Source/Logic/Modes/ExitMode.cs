using LarsOfTheStars.Source.Client;
using LarsOfTheStars.Source.Integration;

namespace LarsOfTheStars.Source.Logic.Modes
{
    public class ExitMode : Mode
    {
        public override void Start()
        {
            Game.Stopped = true;
        }
    }
}
