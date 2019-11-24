using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Entities;
using Game.Scenes.Stage1;
using System;

namespace Game.Models
{
    public class Enemy : HitboxEntity
    {
        private readonly SpriteSheetContext _sprite;
        public readonly int enemyMovementSpeed;

        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 5;
        public const int positionXLowerBound = 0;
        public const int positionXHigherBound = 500;
        public const int positionYLowerBound = 0;
        public const int positionYHigherBound = 500;

        public Enemy() : base(5, 5, 5, 5)
        {
            this.EntityType = EntityType.Enemy;
            this.health = 100;
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
                RenderPosition = this.Position,
                RenderSize = Vector.Create(10, 10)
            };
        }
        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
            base.Draw(canvas);
        }

        public override void OnDeath()
        {
            var stage1 = SceneManager.Singleton.CurrentScene as Stage1;
            var map = stage1.map;

            map.RemoveEntity(this);
        }

        public override void OnCollision(HitboxEntity entity)
        {
            //TODO: implement the enemy damage to player.
            //if (entity.EntityType == EntityType.Player)
            //{
            //    entity.Damage(25);
            //}
        }
    }
}