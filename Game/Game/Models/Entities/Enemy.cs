using Annex.Data;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Entities;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using Game.Scenes.Stage1;
using System;
using System.Linq;

namespace Game.Models
{
    public class Enemy : HitboxEntity
    {
        private static SpriteSheetContext GetEnemySprite(Vector position, Vector size) {
            switch (RNG.Next(3)) {
                case 0:
                    return new SpriteSheetContext("beedle.png", 2, 2) {
                        RenderPosition = new OffsetVector(position, new ScalingVector(size, -0.5f, -0.5f))
                    };
                case 1:
                    return new SpriteSheetContext("evil spider.png", 2, 2) {
                        RenderPosition = new OffsetVector(position, new ScalingVector(size, -0.5f, -0.5f))
                    };
                case 2:
                    return new SpriteSheetContext("spider.png", 2, 12) {
                        RenderPosition = new OffsetVector(position, new ScalingVector(size, -0.5f, -0.5f))
                    };
                default:
                    return new SpriteSheetContext("", 0, 0) {
                        RenderPosition = new OffsetVector(position, new ScalingVector(size, -0.5f, -0.5f))

                    };
            }
        }
        private static SpriteSheetContext GetAntSprite(Vector position, Vector size) {
            return new SpriteSheetContext("Aunt.png", 2, 4) {
                RenderPosition = new OffsetVector(position, new ScalingVector(size, -0.5f, -0.5f))
            };
        }

        private readonly SolidRectangleContext _redHealthbar;
        private readonly SolidRectangleContext _greenHealthbar;

        private readonly SpriteSheetContext _sprite;
        public readonly int enemyMovementSpeed;

        public readonly Vector Size;

        private long LastAttacked = 0;
        internal float dy;
        internal float dx;
        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 4;
        //Spawn the enemy inside of the arena =/
        public const int positionXLowerBound = 150;
        public const int positionXHigherBound = 800;
        public const int positionYLowerBound = 150;
        public const int positionYHigherBound = 600;
        public readonly bool IsAnt = false;

        public Enemy() : base(10, 10, 10, 10) {
            this.Size = Vector.Create(32, 32);

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


            if (RNG.Next(100) < 10) {
                IsAnt = true;
                this._sprite = GetAntSprite(this.Position, this.Size);
                this.enemyMovementSpeed = enemyMovementSpeedLowerBound;
            } else {
                IsAnt = false;
                this._sprite = GetEnemySprite(this.Position, this.Size);
            }
        }

        public void CastFireball() {
            var scene = SceneWithMap.CurrentScene;
            if (scene is Stage1 s1) {

                var availablePlayers = s1.players.Where(player => player?.isPlayerDead == false).ToArray();

                if (availablePlayers.Length == 0) {
                    return;
                }

                var target = availablePlayers[RNG.Next(availablePlayers.Length)];

                var fireball = new Fireball(target, this.Position);
                scene.map.AddEntity(fireball);
            }
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._sprite);
            canvas.Draw(this._redHealthbar);
            canvas.Draw(this._greenHealthbar);
            base.Draw(canvas);
        }

        public override void OnDeath() {
            var scene = SceneWithMap.CurrentScene;
            var map = scene.map;

            map.RemoveEntity(this);

            if (RNG.Next(100) <= 10) {
                var fly = new Flies();
                fly.Position.Set(this.Position);
                map.AddEntity(fly);
            }
        }

        public override void OnCollision(HitboxEntity entity) {
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

        public void Animate() {
            if (dx < 0) {
                this._sprite.SetRow(1);
            } else {
                this._sprite.SetRow(0);
            }

            if (dx != 0 && dy != 0) {
                this._sprite.StepColumn();
            }
        }

        public override bool ConsiderCollision(HitboxEntity entity) {
            if (entity is Fireball) {
                return false;
            }
            return base.ConsiderCollision(entity);
        }
    }
}