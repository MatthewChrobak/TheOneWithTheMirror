using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models.Entities
{
    public class HitboxEntity : Entity
    {
        public SolidRectangleContext _hitboxSprite;

        public static bool RenderHitboxes = false;

        private float _left;
        private float _top;
        private float _right;
        private float _bottom;

        public float RealLeft => this.Position.X - this._left;
        public float RealTop => this.Position.Y - this._top;
        public float RealRight => this.Position.X + this._right;
        public float RealBottom => this.Position.Y + this._bottom;

        public HitboxEntity(Vector position, uint left, uint top, uint right, uint bottom) : base(position) {
            this._left = left;
            this._top = top;
            this._right = right;
            this._bottom = bottom;

            this._hitboxSprite = new SolidRectangleContext(new RGBA(255, 0, 0, 150)) {
                RenderPosition = new OffsetVector(this.Position, -left, -top),
                RenderSize = Vector.Create(left + right, top + bottom)
            };
        }

        public HitboxEntity(uint left, uint top, uint right, uint bottom) : this(null, left, top, right, bottom) {
        }

        public override void Draw(ICanvas canvas) {
            if (RenderHitboxes) {
                canvas.Draw(this._hitboxSprite);
            }
        }

        public virtual void OnCollision(HitboxEntity entity) {

        }
    }
}
