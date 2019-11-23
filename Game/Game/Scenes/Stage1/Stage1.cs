using Annex.Events;
using Annex.Graphics;
using Annex.Scenes.Components;
using Game.Models.Chunks;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public Map map;
        public Player player;

        public Stage1()
        {
            player = new Player();

            this.map = new Map();
            map.GetChunk(0, 0);
            map.GetChunk(1, 1);

            LoadNearChunks(0, 0);          
            player.OnPlayerMovedToNewChunk += LoadNearChunks; //Remove event when changing scenes

            var camera = GameWindow.Singleton.Canvas.GetCamera();
            camera.Follow(this.player.Position); 
        }

        public override void Draw(ICanvas canvas)
        {
            map.Draw(canvas);
            player.Draw(canvas);
            base.Draw(canvas);
        }

        public void LoadNearChunks(int x, int y)
        {            
            for (int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (x + i == x && y + j == y)
                    {
                        player.SetCurrentChunk(map.GetChunk(x, y));
                        continue;
                    }
                    map.GetChunk(x + i, y + j);
                }
            }
        }
    }
}
