using Annex.Data.Shared;
using Annex.Graphics;

namespace Game.Models.Entities
{
    public abstract class Entity : IDrawableObject
    {
        public readonly Vector Position;
        public EntityType EntityType;
        public int health;


        public Entity() {
            this.Position = Vector.Create();
        }

        public abstract void Draw(ICanvas canvas);
    }
}

