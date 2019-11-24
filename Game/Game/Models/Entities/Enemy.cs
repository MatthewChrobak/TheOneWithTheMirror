using Annex.Data;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Entities;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using System;

namespace Game.Models
{
    public class Enemy : HitboxEntity
    {
        private readonly SpriteSheetContext _sprite;
        public readonly int enemyMovementSpeed;

        private long LastAttacked = 0;

        public const int enemyMovementSpeedLowerBound = 1;
        public const int enemyMovementSpeedHigherBound = 4;
        public const int positionXLowerBound = 0;
        public const int positionXHigherBound = 100;
        public const int positionYLowerBound = 0;
        public const int positionYHigherBound = 100;


        private int totalDamageSeconds = 5;
        private int damageSeconds = 0;

        public Enemy() : base(10, 10, 10, 10)
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

            this._sprite = new SpriteSheetContext("Clawdia_FacingUpUp.png", 1, 1)
            {
                RenderPosition = new OffsetVector(this.Position, -15, -15),
                RenderSize = Vector.Create(30, 30)
            };     
        }
        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
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
                
                int defenseMultiplier = playerHitbox.Player.GetBuff(Buffs.BuffTypes.Defense);                
                playerHitbox.Player.Damage(2, -defenseMultiplier);

                var scene = SceneWithMap.CurrentScene;
                scene.AddScrollingMessage(new ScrollingTextMessage("-1", entity.Position.X, entity.Position.Y, RGBA.Purple));
            }
        }

        public void DamageOverTime()
        {
            EventManager.Singleton.AddEvent(PriorityType.LOGIC, () =>
            {
                damageSeconds++;

                if(damageSeconds >= totalDamageSeconds)
                {
                    damageSeconds = 0;
                    return ControlEvent.REMOVE;
                }
                //Self damage
                this.Damage(-(int)this.Health.Regen.Value);
               
                return ControlEvent.NONE;
            }, 1000, 0, "buff-DOT");
        }
    }
}