using Annex.Graphics;
using Annex.Scenes.Components;
using Game.Models.Chunks;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public Map map;
        public Player player;

        public Stage1() {
            player = new Player();

            this.map = new Map();
            map.GetChunk(0, 0);
            map.GetChunk(1, 1);
        }

        public override void Draw(ICanvas canvas) {
            map.Draw(canvas);
            player.Draw(canvas);
            base.Draw(canvas);
        }
    }
}
