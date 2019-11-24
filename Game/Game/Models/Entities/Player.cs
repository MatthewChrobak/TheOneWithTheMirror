
using Annex;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Chunks;
using Game.Scenes.Stage1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models.Entities
{
    public class Player : HitboxEntity, ICombatSystem
    {
        private (int x, int y) LastChunkID = (int.MinValue, int.MinValue);
        public int CurrentXChunkID => (int)Math.Floor(this.Position.X / MapChunk.ChunkWidth);
        public int CurrentYChunkID => (int)Math.Floor(this.Position.Y / MapChunk.ChunkHeight);
        public event Func<HitboxEntity, (float x, float y)> CollisionHandler;

        public SpriteSheetContext _sprite;

        private int framesBeforeSlowingDown = 7;
        private long slowedFrameIntervals = 50;

        private int jumpFrames = 40;
        private int jumpFrameIntervals = 5;
        private const float delayBetweenJumps = 600;
        private float dx;
        private float dy;
        private int jumpCount = 0;
        private long lastTimeMoved;

        private double angle;

        public readonly uint _joystickID;

        public Player(uint joystickID) : base(5, 5, 5, 5) {
            this._joystickID = joystickID;
            this.health = 100;
            this._sprite = new SpriteSheetContext("smushy.png", 1, 8) {
                RenderPosition = this.Position,
                RenderSize = Vector.Create(25, 50)
            };

            EventManager.Singleton.AddEvent(PriorityType.INPUT, HandlePlayerInput, 10, 0, "KeyboardInput");
            Debug.AddDebugInformation(() => $"Player {_joystickID} dx: {dx} dy: {dy}");
            Debug.AddDebugInformation(() => $"Angle: {angle}");
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._sprite);
            base.Draw(canvas);
        }

        private ControlEvent HandlePlayerInput() {
            if (!SceneManager.Singleton.IsCurrentScene<Stage1>())
            {
                return ControlEvent.NONE;
            }

            var canvas = GameWindow.Singleton.Canvas;

            var dx = canvas.GetJoystickAxis(this._joystickID, JoystickAxis.X);
            var dy = canvas.GetJoystickAxis(this._joystickID, JoystickAxis.Y);
            float magX = Math.Abs(dx);
            float magY = Math.Abs(dy);

            if (magX < 50 && magY < 50) {
                return ControlEvent.NONE;
            }

            long time = EventManager.CurrentTime;

            if (time > lastTimeMoved + delayBetweenJumps) {
                this.dx = dx;
                this.dy = dy;
                this.jumpCount = 0;
                var events = EventManager.Singleton;
                events.AddEvent(PriorityType.ANIMATION, Jump, jumpFrameIntervals, 0, "Jump");
                lastTimeMoved = time;

                events.AddEvent(PriorityType.ANIMATION, UpdateAnimation, (int)delayBetweenJumps / 10);
            }

            return ControlEvent.NONE;
        }

        private ControlEvent Jump() {
            jumpCount++;

            if (jumpCount >= jumpFrames) {
                return ControlEvent.REMOVE;
            } else if (jumpCount >= jumpFrames - framesBeforeSlowingDown) {
                //var events = Events.GetEvent("Jump");
                var e = EventManager.Singleton.GetEvent("Jump");
                e.SetInterval(slowedFrameIntervals);
            }

            ChangeDirection();
            float speed = 2;
            float signX = (this.dx / 100) * speed;
            float signY = (this.dy / 100) * speed;
            this.Position.Add(signX * speed, signY * speed);
            var collisions = CollisionHandler?.Invoke(this);
            this.Position.Add(collisions.Value.x, collisions.Value.y);
            if (collisions.Value.x != 0 || collisions.Value.y != 0) {
                return ControlEvent.REMOVE;
            }

            return ControlEvent.NONE;
        }

        private ControlEvent UpdateAnimation() {
            this._sprite.StepColumn();

            if (this._sprite.Column == 0) {
                return ControlEvent.REMOVE;
            }
            return ControlEvent.NONE;           
        }

        public void ChangeDirection()
        {
            angle = (Math.Atan2(dy, dx) * (180 / Math.PI) + 360) % 360;
            
            //right
            if(angle >= 337.5 || angle < 22.5)
            {

            }
            //bottom right
            else if (angle >= 22.5 || angle < 67.5)
            {

            }
            //down
            else if(angle >= 67.5 || angle < 112.5)
            {

            }
            //bottom left
            else if(angle >= 112.5 || angle < 157.5)
            {

            }
            //left
            else if(angle >= 157.5 || angle < 202.5)
            {

            }
            //top left
            else if(angle >= 202.5 || angle < 247.5)
            {

            }
            //up
            else if(angle >= 247.5 || angle < 292.5)
            {

            }
            //top right
            else if(angle >= 292.5 || angle < 337.5)
            {

            }


            //left
            if(dx < - 50)
            {
                if (this._sprite.Row % 2 == 1)
                {
                    this._sprite.StepRow();
                }                
            }
            //right
            else if(dx > 50)
            {                
                if(this._sprite.Row % 2 == 0)
                {
                    this._sprite.StepRow();
                }                
            }            
        }

        public void Attack(float x, float y, IEnumerable<Entity> entity)
        {
            var radius = 50;
            var enemyWithinPlayerRange = entity.Where(a => radius >= Math.Pow(a.Position.X, 2) + Math.Pow(a.Position.Y, 2)).ToList();
        }

        public override void OnCollision(HitboxEntity entity)
        {
            if (entity.EntityType == EntityType.Enemy) 
            {
                entity.health = 0;
            }
        }
    }
}