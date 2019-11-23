using Annex;
using Game.Scenes.Stage1;

namespace Game
{
    public class Program
    {
        public static void Main(string[] args) {
            var game = new AnnexGame();
            game.Start<Stage1>();
        }
    }
}
