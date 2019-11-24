using Annex;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Chunks;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using Game.Scenes.Stage1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models.Entities
{
    public class Player : Entity
    {
        private (int x, int y) LastChunkID = (int.MinValue, int.MinValue);
        public int CurrentXChunkID => (int)Math.Floor(this.Position.X / MapChunk.ChunkWidth);
        public int CurrentYChunkID => (int)Math.Floor(this.Position.Y / MapChunk.ChunkHeight);
        public event Func<HitboxEntity, (float x, float y)> CollisionHandler;
        public event Func<HitboxEntity, (float x, float y)> BorderCollisionHandler;

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

        private HitboxEntity hitbox;

        public readonly uint _joystickID;

        public Player(uint joystickID) {
            this._joystickID = joystickID;

            this.hitbox = new PlayerHitbox(this, 10, 10, 10, 10);
            AddHitboxToMap(this.hitbox);


            var scene = SceneWithMap.CurrentScene;
            scene.Events.AddEvent($"KeyboardInput-{joystickID}", PriorityType.INPUT, HandlePlayerInput, 10, 0);
            Debug.AddDebugInformation(() => $"Player {_joystickID} dx: {dx} dy: {dy}");
            Debug.AddDebugInformation(() => $"Angle: {angle}");
        }

        private void AddHitboxToMap(HitboxEntity entity) {
            var scene = SceneWithMap.CurrentScene;
            Debug.Assert(scene != null);
            scene.map.AddEntity(entity);
        }

        private void RemoveHitboxFromMap(HitboxEntity entity) {
            var scene = SceneWithMap.CurrentScene;
            Debug.Assert(scene != null);
            scene.map.RemoveEntity(entity);
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._sprite);
        }

        private ControlEvent HandlePlayerInput() {
            if (!SceneManager.Singleton.IsCurrentScene<Stage1>()) {
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

            // So the player can no longer be damaged.
            RemoveHitboxFromMap(this.hitbox);

            long time = EventManager.CurrentTime;

            if (time > lastTimeMoved + delayBetweenJumps) {
                this.dx = dx;
                this.dy = dy;
                this.jumpCount = 0;
                var scene = SceneWithMap.CurrentScene;
                Debug.Assert(scene != null);
                scene.Events.AddEvent("Jump", PriorityType.ANIMATION, Jump, jumpFrameIntervals, 0);
                lastTimeMoved = time;

                scene.Events.AddEvent("", PriorityType.ANIMATION, UpdateAnimation, (int)delayBetweenJumps / 10);
            }

            return ControlEvent.NONE;
        }

        private ControlEvent Jump() {
            jumpCount++;

            if (jumpCount == (int)(0.9 * jumpFrames)) {
                var attack = new JumpAttack(this);
                AddHitboxToMap(attack);
                CollisionHandler?.Invoke(attack);
                RemoveHitboxFromMap(attack);
            }

            if (jumpCount >= jumpFrames) {
                // So the player can be damaged again.
                AddHitboxToMap(this.hitbox);
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
            var collisions = BorderCollisionHandler?.Invoke(this.hitbox);
            this.Position.Add(collisions.Value.x, collisions.Value.y);
            return ControlEvent.NONE;
        }

        private ControlEvent UpdateAnimation() {
            this._sprite.StepColumn();

            if (this._sprite.Column == 0) {
                return ControlEvent.REMOVE;
            }
            return ControlEvent.NONE;
        }

        public void ChangeDirection() {
            angle = (Math.Atan2(dy, dx) * (180 / Math.PI) + 360) % 360;

            //right
            if (angle >= 337.5 || angle < 22.5) {

            }
            //bottom right
            else if (angle >= 22.5 || angle < 67.5) {

            }
            //down
            else if (angle >= 67.5 || angle < 112.5) {

            }
            //bottom left
            else if (angle >= 112.5 || angle < 157.5) {

            }
            //left
            else if (angle >= 157.5 || angle < 202.5) {

            }
            //top left
            else if (angle >= 202.5 || angle < 247.5) {

            }
            //up
            else if (angle >= 247.5 || angle < 292.5) {

            }
            //top right
            else if (angle >= 292.5 || angle < 337.5) {

            }


            //left
            if (dx < -50) {
                if (this._sprite.Row % 2 == 1) {
                    this._sprite.StepRow();
                }
            }
            //right
            else if (dx > 50) {
                if (this._sprite.Row % 2 == 0) {
                    this._sprite.StepRow();
                }
            }
        }

    }
}