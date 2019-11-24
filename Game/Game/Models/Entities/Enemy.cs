using Annex.Data;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Entities;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using System;

namespace Game.Models
{
    public class Enemy : HitboxEntity
    {
        private readonly SolidRectangleContext _redHealthbar;
        private readonly SolidRectangleContext _greenHealthbar;

        private readonly SpriteSheetContext _sprite;
        public readonly int enemyMovementSpeed;

        public readonly Vector Size;

        private long LastAttacked = 0;

        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 5;
        public const int positionXLowerBound = 0;
        public const int positionXHigherBound = 500;
        public const int positionYLowerBound = 0;
        public const int positionYHigherBound = 500;

        public Enemy() : base(10, 10, 10, 10)
        {
            this.Size = Vector.Create(30, 30);

            var healthbarSize = Vector.Create(50, 5);

            this._greenHealthbar = new SolidRectangleContext(RGBA.Green) {
                RenderSize = new ScalingVector(healthbarSize, Vector.Create(this.Health.GetPureRatio(), 1)),
                RenderBorderColor = RGBA.Black,
                RenderBorderSize = 2,
                //-25 + 0, -30 -10
                RenderPosition = new OffsetVector(Position, new OffsetVector(new ScalingVector(healthbarSize, -0.5f, -1), new ScalingVector(Size, 0, -1)))
            };
            this._redHealthbar = new SolidRectangleContext(RGBA.Red) {
                RenderSize = healthbarSize,
                RenderBorderColor = RGBA.Black,
                RenderBorderSize = 2,
                RenderPosition = new OffsetVector(Position, new OffsetVector(new ScalingVector(healthbarSize, -0.5f, -1), new ScalingVector(Size, 0, -1)))
            };

            this.EntityType = EntityType.Enemy;
            Random random = new Random();
            //Generates a random position for the enemy
            var positionX = random.Next(positionXLowerBound, positionXHigherBound);
            var positionY = random.Next(positionYLowerBound, positionYHigherBound);

            //Generates a random speed for the enemy
            int speed = random.Next(enemyMovementSpeedLowerBound, enemyMovementSpeedHigherBound);

            this.enemyMovementSpeed = speed;

            Position.X = positionX;
            Position.Y = positionY;

            this._sprite = new SpriteSheetContext("Clawdia_FacingUpUp.png", 1, 1)
            {
                RenderPosition = new OffsetVector(this.Position, -15, -15),
                RenderSize = Size
            };
        }
        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
            canvas.Draw(this._redHealthbar);
            canvas.Draw(this._greenHealthbar);
            base.Draw(canvas);
        }

        public override void OnDeath()
        {
            var scene = SceneWithMap.CurrentScene;
            var map = scene.map;

            map.RemoveEntity(this);
        }

        public override void OnCollision(HitboxEntity entity)
        {
            if (entity is PlayerHitbox playerHitbox) {

                if (EventManager.CurrentTime - LastAttacked < 1000) {
                    return;
                }
                this.LastAttacked = EventManager.CurrentTime;

                playerHitbox.Player.Damage(1);

                var scene = SceneWithMap.CurrentScene;
                scene.AddScrollingMessage(new ScrollingTextMessage("-1", entity.Position.X, entity.Position.Y, RGBA.Purple));
            }
        }
    }
}