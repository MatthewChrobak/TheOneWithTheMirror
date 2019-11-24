using Annex;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Buffs;
using Game.Models.Chunks;
using Game.Models.Entities.Hitboxes;
using Game.Scenes;
using Game.Scenes.Stage1;
using System;
using System.Collections.Generic;

namespace Game.Models.Entities
{
    public class Player : Entity
    {
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
        internal bool isPlayerDead = false;

        private double angle;

        private HitboxEntity hitbox;

        public readonly uint _joystickID;
        private Dictionary<Buffs.BuffTypes, int> buffs;

        public Player(uint joystickID) {
            this._joystickID = joystickID;

            this.hitbox = new PlayerHitbox(this, 10, 10, 10, 10);
            AddHitboxToMap(this.hitbox);

            var scene = SceneWithMap.CurrentScene;
            scene.Events.AddEvent($"KeyboardInput-{joystickID}", PriorityType.INPUT, HandlePlayerInput, 10, 0);
            Debug.AddDebugInformation(() => $"Player {_joystickID} dx: {dx} dy: {dy}");
            Debug.AddDebugInformation(() => $"Angle: {angle}");

            buffs = new Dictionary<Buffs.BuffTypes, int>();
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

            //check if the player is dead
            if (this.isPlayerDead)
                return ControlEvent.NONE;

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
            
            int buffStack = 0;
            if(!buffs.TryGetValue(Buffs.BuffTypes.Speed, out buffStack))
            {
                buffs.Add(Buffs.BuffTypes.Speed, 0);
            }            

            if (time > lastTimeMoved + (delayBetweenJumps * (1 - 0.05 * buffStack ))) {
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
            if(angle >= 337.5 || angle < 22.5)
            {
                this._sprite.SetRow(2);
            }
            //bottom right
            else if (angle >= 22.5 && angle < 67.5)
            {
                this._sprite.SetRow(3);
            }
            //down
            else if(angle >= 67.5 && angle < 112.5)
            {
                this._sprite.SetRow(4);
            }
            //bottom left
            else if(angle >= 112.5 && angle < 157.5)
            {
                this._sprite.SetRow(5);
            }
            //left
            else if(angle >= 157.5 && angle < 202.5)
            {
                this._sprite.SetRow(6);
            }
            //top left
            else if(angle >= 202.5 && angle < 247.5)
            {
                this._sprite.SetRow(7);
            }
            //up
            else if(angle >= 247.5 && angle < 292.5)
            {
                this._sprite.SetRow(0);
            }
            //top right
            else if(angle >= 292.5 && angle < 337.5)
            {
                this._sprite.SetRow(1);
            }
        }

        public bool GetBuff(BuffTypes types) {
            return this.buffs.TryGetValue(types, out _) ? true : false;
        }

        public void SetBuff(BuffTypes type)
        {
            switch (type)
            {
                case Buffs.BuffTypes.Damage:
                    this.buffs.Add(BuffTypes.Damage, (int)BuffTypes.Damage);
                    break;
                case Buffs.BuffTypes.Defense:
                    break;
                case Buffs.BuffTypes.Speed:
                    break;
                case Buffs.BuffTypes.Chill:
                    break;
                case Buffs.BuffTypes.MeleeResistance:
                    break;
                case Buffs.BuffTypes.RangeResistance:
                    break;
                case Buffs.BuffTypes.PoisonResistance:
                    break;
                case Buffs.BuffTypes.Big:
                    break;
                case Buffs.BuffTypes.Splash:
                    break;
                case Buffs.BuffTypes.DOT:
                    break;
                case Buffs.BuffTypes.Regen:
                    this.buffs.Add(BuffTypes.Regen, (int)BuffTypes.Regen);
                    break;
                case Buffs.BuffTypes.Lifesteal:
                    this.buffs.Add(BuffTypes.Lifesteal, (int)BuffTypes.Lifesteal);
                    break;
                case Buffs.BuffTypes.Shield:
                    this.buffs.Add(BuffTypes.Shield, (int)BuffTypes.Shield);
                    break;
                case Buffs.BuffTypes.COUNT:
                    break;
            }
        }

        public override void OnDeath()
        {
            if(this.Health.Current <= 0)
            {
                var scene = SceneWithMap.CurrentScene;
                var map = scene.map;
                this.isPlayerDead = true;
                map.RemoveEntity(this);
            }

        }

    }
}