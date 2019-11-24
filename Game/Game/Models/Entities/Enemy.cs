using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Entities;
using System;

namespace Game.Models
{
    public class Enemy : Entity
    {
        private readonly SpriteSheetContext _sprite;
        public readonly int enemyMovementSpeed;

        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 5;
        public const int positionXLowerBound = -5000;
        public const int positionXHigherBound = -5000;
        public const int positionYLowerBound = -5000;
        public const int positionYHigherBound = -5000;

        public Enemy()
        {
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

            this._sprite = new SpriteSheetContext("Pokemon.png", 25, 20)
            {
                RenderPosition = this.Position,
                RenderSize = Vector.Create(10, 10)
            };
        }
        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
        }
    }
}