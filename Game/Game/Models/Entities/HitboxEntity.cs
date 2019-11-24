using System;
using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models.Entities
{
    public class HitboxEntity : Entity
    {
        protected SolidRectangleContext _hitboxSprite;

        private float _left;
        private float _top;
        private float _right;
        private float _bottom;

        public float RealLeft => this.Position.X - this._left;
        public float RealTop => this.Position.Y - this._top;
        public float RealRight => this.Position.X + this._right;
        public float RealBottom => this.Position.Y + this._bottom;

        public HitboxEntity(uint left, uint top, uint right, uint bottom) {

            this._left = left;
            this._top = top;
            this._right = right;
            this._bottom = bottom;

            this._hitboxSprite = new SolidRectangleContext(new RGBA(255, 0, 0, 150)) {
                RenderPosition = new OffsetVector(this.Position, -left, -top),
                RenderSize = Vector.Create(left + right, top + bottom)
            };
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._hitboxSprite);
        }

        public virtual void OnCollision(HitboxEntity entity) {

        }

        internal void Damage(int damage)
        {
            this.health -= damage;

            if (this.health <= 0)
            {
                this.OnDeath();
            }
        }

        public virtual void OnDeath()
        {

        }
    }
}
