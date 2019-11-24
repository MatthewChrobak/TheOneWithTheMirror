using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Chunks;
using System;

namespace Game.Models.Entities
{
    public class Flies : HitboxEntity
    {
        private readonly SpriteSheetContext _sprite;
        public readonly Vector Size;
        public readonly Vector Target;

        public float DX => MathF.Sign(this.Target.X - Position.X);
        public float DY => MathF.Sign(this.Target.Y - Position.Y);

        public Flies() : base(10, 10, 10, 10) {
            this.EntityType = EntityType.Fly;
            this.Target = Vector.Create(0, 0);
            this.Size = Vector.Create(20, 20);
            GetNewTarget();
            this._sprite = new SpriteSheetContext("flies.png", 1, 3) {
                RenderPosition = new OffsetVector(this.Position, new ScalingVector(this.Size, -0.5f, -0.5f))
            };
        }

        public void Move() {
            float dx = this.Position.X - this.Target.X;
            float dy = this.Position.Y - this.Target.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            this.Position.Add(DX, DY);

            this._sprite.StepColumn();

            if (distance < 10) {
                GetNewTarget();
            }
        }

        public void GetNewTarget() {
            this.Target.Set(RNG.Next(Map.Size_X), RNG.Next(Map.Size_Y));
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._sprite);
            base.Draw(canvas);
        }
    }
}
