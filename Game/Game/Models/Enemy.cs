using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using System;

namespace Game.Models
{
    public class Enemy : IDrawableObject
    {
        private readonly SpriteSheetContext _sprite;
        public readonly Vector Position;
        public readonly int enemyMovementSpeed;

        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 10;
        public const int positionXLowerBound = 0;
        public const int positionXHigherBound = 1000;
        public const int positionYLowerBound = 0;
        public const int positionYHigherBound = 1000;

        public Enemy()
        {
            
            Random random = new Random();
            //Generates a random position for the enemy
            var positionX = random.Next(positionXLowerBound, positionXHigherBound);
            var positionY = random.Next(positionYLowerBound, positionYHigherBound);

            //Generates a random speed for the enemy
            int speed = random.Next(enemyMovementSpeedLowerBound, enemyMovementSpeedHigherBound);

            this.enemyMovementSpeed = speed;

            this.Position = Vector.Create();
            Position.X = positionX;
            Position.Y = positionY;

            this._sprite = new SpriteSheetContext("Pokemon.png", 25, 20)
            {
                RenderPosition = Position
            };
        }

        public void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
        }
    }
}