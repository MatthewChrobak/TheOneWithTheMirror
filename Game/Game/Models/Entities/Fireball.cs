using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using System;

namespace Game.Models.Entities
{
    public class Fireball : HitboxEntity
    {
        private Vector Size;
        private SpriteSheetContext _fireballSprite;
        private Player target;
        public float DX;
        public float DY;

        public Fireball(Player target, Vector position) : base(5, 5, 5, 5) {
            this.target = target;
            this.Size = Vector.Create(15, 15);

            this.Position.Set(position);

            float tx = target.Position.X - this.Position.X;
            float ty = target.Position.Y - this.Position.Y;

            double angle = Math.Atan2(ty, tx) + 3.14f;

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            DX = (float)(2 * cos);
            DY = (float)(2 * sin);


            this._fireballSprite = new SpriteSheetContext("fireball.png", 1, 3) {
                RenderPosition = new OffsetVector(this.Position, new ScalingVector(this.Size, -0.5f, -0.5f))
            };
        }

        public void Move() {

            this.Position.Add(-DX, -DY);

            this._fireballSprite.StepColumn();

            var scene = SceneWithMap.CurrentScene;
            var col = scene.map.GetMaximumColllisions(this);
            if (col.x != 0 || col.y != 0) {
                scene.map.RemoveEntity(this);
                return;
            }
        }

        public override bool ConsiderCollision(HitboxEntity entity) {
            if (entity is Enemy) {
                return false;
            }
            return base.ConsiderCollision(entity);
        }

        public override void Draw(ICanvas canvas) {

            canvas.Draw(this._fireballSprite);
            base.Draw(canvas);
        }

        public override void OnCollision(HitboxEntity entity) {
            if (entity is PlayerHitbox hitbox) {
                hitbox.Player.Damage(10);

                SceneWithMap.CurrentScene.AddScrollingMessage(new ScrollingTextMessage("-10", hitbox.Position.X, hitbox.Position.Y, RGBA.Red));
            }
        }
    }
}
