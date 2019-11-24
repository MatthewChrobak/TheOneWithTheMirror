using Annex.Data;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Chunks;
using Game.Scenes.Stage1;
using System;

namespace Game.Models.Entities
{
    public class Player : HitboxEntity
    {
        private (int x, int y) LastChunkID = (int.MinValue, int.MinValue);
        public int CurrentXChunkID => (int)Math.Floor(this.Position.X / MapChunk.ChunkWidth);
        public int CurrentYChunkID => (int)Math.Floor(this.Position.Y / MapChunk.ChunkHeight);
        public event Action<int, int> ChunkLoader;
        public event Func<HitboxEntity, (float x, float y)> CollisionHandler;

        public SpriteSheetContext _sprite;

        private int framesBeforeSlowingDown = 5;
        private long slowedFrameIntervals = 50;

        private int jumpFrames = 25;
        private int jumpFrameIntervals = 5;
        private const float delayBetweenJumps = 500;
        private float dx;
        private float dy;
        private int jumpCount = 0;
        private long lastTimeMoved;

        public readonly uint _joystickID;

        public Player(uint joystickID) : base(5, 5, 5, 5) {
            this._joystickID = joystickID;

            this._sprite = new SpriteSheetContext("smushy.png", 1, 8) {
                RenderPosition = this.Position,
                RenderSize = Vector.Create(25, 50)
            };

            EventManager.Singleton.AddEvent(PriorityType.INPUT, HandlePlayerInput, 10, 0, "KeyboardInput");
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

            if (jumpCount == jumpFrames) {
                return ControlEvent.REMOVE;
            } else if (jumpCount >= jumpFrames - framesBeforeSlowingDown) {
                //var events = Events.GetEvent("Jump");
                var e = EventManager.Singleton.GetEvent("Jump");
                e.SetInterval(slowedFrameIntervals);
            }

            float speed = 2;
            float signX = (this.dx / 100) * speed;
            float signY = (this.dy / 100) * speed;
            this.Position.Add(signX * speed, signY * speed);

            var collisions = CollisionHandler?.Invoke(this);


            this.Position.Add(collisions.Value.x, collisions.Value.y);
            if (collisions.Value.x != 0 || collisions.Value.y != 0) {
                return ControlEvent.REMOVE;
            }

            HasMovedToNewChunk();
            return ControlEvent.NONE;
        }

        private ControlEvent UpdateAnimation() {
            this._sprite.StepColumn();

            if (this._sprite.Column == 0) {
                return ControlEvent.REMOVE;
            }
            return ControlEvent.NONE;
        }

        public void HasMovedToNewChunk() {
            if (LastChunkID.x != CurrentXChunkID || LastChunkID.y != CurrentYChunkID) {
                this.LastChunkID = (CurrentXChunkID, CurrentYChunkID);

                int chunkDistance = 0;
                for (int y = -chunkDistance; y <= chunkDistance; y++) {
                    for (int x = -chunkDistance; x <= chunkDistance; x++) {
                        ChunkLoader?.Invoke(CurrentXChunkID + y, CurrentYChunkID + x);
                    }
                }
            }
        }
    }
}