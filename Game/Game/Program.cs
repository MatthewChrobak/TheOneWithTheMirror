using Annex;
using Annex.Events;
using Annex.Graphics;
using Game.Scenes.MainMenu;

namespace Game
{
    public class Program
    {
        public static void Main(string[] args) {
            var game = new AnnexGame();

            var drawEvent = EventManager.Singleton.GetEvent(GameWindow.DrawGameEventID);
            drawEvent.SetInterval(0);
            var tracker = new EventTracker(1000);
            drawEvent.AttachTracker(tracker);

            Debug.AddDebugInformation(() => $"FPS: {tracker.LastCount}");

            game.Start<MainMenu>();
        }
    }
}
