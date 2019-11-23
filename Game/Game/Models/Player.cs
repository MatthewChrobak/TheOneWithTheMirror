using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models
{
    public class Player : IDrawableObject
    {
        private readonly SpriteSheetContext _sprite;
        public readonly Vector Position;

        public Player()
        {

            this.Position = Vector.Create();
            this._sprite = new SpriteSheetContext("Pokemon.png", 25, 20)
            {
                RenderPosition = Position
            };
        }

        public void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
        }
    }
}