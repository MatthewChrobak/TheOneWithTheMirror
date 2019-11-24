using Annex.Data.Shared;
using Annex.Graphics;
using Game.Models.Vitals;

namespace Game.Models.Entities
{
    public abstract class Entity : IDrawableObject
    {
        public readonly Vector Position;
        public EntityType EntityType;
        public readonly Vital Health;

        public Entity(Vector position = null) {
            this.Health = new Vital();
            if (position != null) {
                this.Position = position;
            } else {
                this.Position = Vector.Create();
            }
        }

        public abstract void Draw(ICanvas canvas);

        public void Damage(int damage) {
            this.Health.Current.Value -= damage;

            if (this.Health.Current.Value <= 0) {
                this.OnDeath();
            }
        }

        public virtual void OnDeath() {

        }
    }
}

