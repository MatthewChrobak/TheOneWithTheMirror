using Annex;
using Game.Scenes.MainMenu;

namespace Game
{
    public class Program
    {
        public static void Main(string[] args) {
            var game = new AnnexGame();
            game.Start<MainMenu>();
        }
    }
}
