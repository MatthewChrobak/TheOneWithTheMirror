using Annex;
using Annex.Audio;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Game.Models;
using Game.Models.Entities;
using Game.Models.Entities.Items;
using Game.Scenes.CharacterSelect;
using Game.Scenes.Stage1.Elements;
using System;
using System.Linq;

namespace Game.Scenes.Stage1
{
    public class Stage1 : SceneWithMap
    {
        public Player[] players;
        public Enemy[] enemies;

        // MAP EDITING TOOLS
        private uint debugPlayerID = 0;
        public string MapBrush_Texture;
        public int MapBrush_Top;
        public int MapBrush_Left;
        public string MapBrush_Mode = "single";
        public int enemyCount = 0;
        public const int  maximumEnemy = 100;

        //Spawn rate in milliseconds
        public const int enemySpawnRate = 5000;
        public Annex.Audio.Players.IAudioPlayer audio = AudioManager.Singleton;

        public Stage1() : base("stage1") {
            this.Size.Set(500, 500);
            players = new Player[6];

            PlayerOverlay.NumOverlays = 0;

            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);

            //Initializing First Entities
            map.AddEntity(new Enemy());
            map.AddEntity(new Sword());
            map.AddEntity(new PoisonBow());
            map.AddEntity(new Shield());
            map.AddEntity(new RegenPotion());
            map.AddEntity(new SpeedRing());

            audio.PlayBufferedAudio("AwesomeMusic.flac", "AwesomeMusic", true, 60);

            this.Events.AddEvent("add-new-enemy", PriorityType.LOGIC, AddEnemy, enemySpawnRate);
            this.Events.AddEvent("update-enemy-positions", PriorityType.LOGIC, UpdateEnemyPositions, 20);
            this.Events.AddEvent("update-enemy-positions", PriorityType.LOGIC, () => {
                foreach (var entity in map.GetEntities(e => e.EntityType == EntityType.Enemy)) {
                    var enemy = entity as Enemy;
                    enemy.Animate();
                }
                return ControlEvent.NONE;
            }, 1000);
            this.Events.AddEvent("update-fly-positions", PriorityType.LOGIC, UpdateFlyPositions, 20);
            this.Events.AddEvent("rotateSword", PriorityType.LOGIC, UpdateSwordAnimation, 100);
            this.Events.AddEvent("rotatePoisonArrow", PriorityType.LOGIC, UpdatePoisonBowAnimation, 100);
            this.Events.AddEvent("rotateShield", PriorityType.LOGIC, UpdateShieldAnimation, 100);
            this.Events.AddEvent("rotateRegenPotion", PriorityType.LOGIC, UpdateRegenPotionAnimation, 100);
            this.Events.AddEvent("rotateSpeedRing", PriorityType.LOGIC, UpdateSpeedRingAnimation, 100);

            this.Events.AddEvent("ant-fireballs", PriorityType.LOGIC, () => {

                foreach (var entity in this.map.GetEntities(entity => entity as Enemy != null)) {
                    var enemy = entity as Enemy;

                    if (enemy.IsAnt) {
                        enemy.CastFireball();
                    }
                }

                return ControlEvent.NONE;
            }, 5000);


            Debug.AddDebugCommand("enablekeys", (data) => {
                var canvas = GameWindow.Singleton.Canvas;
                this.Events.AddEvent("debug-keys", PriorityType.INPUT, () => {

                    float speed = 1;
                    float boost = 10;
                    float dx = 0;
                    float dy = 0;

                    if (canvas.IsKeyDown(KeyboardKey.LShift) || canvas.IsKeyDown(KeyboardKey.RShift)) {
                        speed = boost;
                    }
                    if (canvas.IsKeyDown(KeyboardKey.Up)) {
                        dy -= speed;
                    }
                    if (canvas.IsKeyDown(KeyboardKey.Down)) {
                        dy += speed;
                    }
                    if (canvas.IsKeyDown(KeyboardKey.Left)) {
                        dx -= speed;
                    }
                    if (canvas.IsKeyDown(KeyboardKey.Right)) {
                        dx += speed;
                    }

                    this.players[0]?.Position.Add(dx, dy);

                    return ControlEvent.NONE;
                }, 10);
            });
            Debug.AddDebugCommand("setplayerposition", (data) => {
                int id = int.Parse(data[0]);
                float x = float.Parse(data[1]);
                float y = float.Parse(data[2]);

                this.players[id]?.Position.Set(x, y);
            });
            Debug.AddDebugCommand("newplayer", (data) => {
                debugPlayerID = uint.Parse(data[0]);
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.A,
                    JoystickID = debugPlayerID
                });
            });

            Debug.AddDebugCommand("additem", (data) => {
                map.AddEntity(new Sword());
            });
        }
        private ControlEvent UpdateSpeedRingAnimation() {
            var entities = map.GetEntities(Entity => Entity.EntityType == EntityType.SpeedRing);

            foreach (var entity in entities) {
                if (entity is SpeedRing item)
                    item._sprite.StepColumn();
            }

            return ControlEvent.NONE;
        }

        private ControlEvent UpdateRegenPotionAnimation() {
            var entities = map.GetEntities(Entity => Entity.EntityType == EntityType.RegenPotion);

            foreach (var entity in entities) {
                if (entity is RegenPotion item)
                    item._sprite.StepColumn();
            }

            return ControlEvent.NONE;
        }

        private ControlEvent UpdateShieldAnimation() {
            var entities = map.GetEntities(Entity => Entity.EntityType == EntityType.Shield);

            foreach (var entity in entities) {
                if (entity is Shield item)
                    item._sprite.StepColumn();
            }

            return ControlEvent.NONE;
        }

        private ControlEvent AddEnemy() {
            if (enemyCount < maximumEnemy) {
                map.AddEntity(new Enemy());
                enemyCount++;
            }
            return ControlEvent.NONE;
        }
        private ControlEvent UpdateSwordAnimation() {
            var entities = map.GetEntities(Entity => Entity.EntityType == EntityType.Sword);

            foreach (var entity in entities) {
                if (entity is Sword item)
                    item._sprite.StepColumn();
            }

            return ControlEvent.NONE;
        }

        private ControlEvent UpdatePoisonBowAnimation() {
            var entities = map.GetEntities(Entity => Entity.EntityType == EntityType.PoisonBow);

            foreach (var entity in entities) {
                if (entity is PoisonBow item)
                    item._sprite.StepColumn();
            }

            return ControlEvent.NONE;
        }

        private ControlEvent UpdateFlyPositions() {
            var flies = map.GetEntities(entity => entity.EntityType == EntityType.Fly);
            foreach (var f in flies) {
                var fly = f as Flies;
                fly.Move();
                var correction = this.map.GetMapBorderCollisions(fly);
                fly.Position.Add(correction.x, correction.y);
            }
            return ControlEvent.NONE;
        }

        private ControlEvent UpdateEnemyPositions() {
            var enemyList = map.GetEntities(entity => entity.EntityType == EntityType.Enemy).ToList();
            foreach (var entity in enemyList) {
                if (entity is Enemy enemy) {
                    var target = players.FirstOrDefault();
                    var distance = double.MaxValue;

                    if (target == null) {
                        continue;
                    }

                    foreach (var player in players) {
                        if (player == null) {
                            continue;
                        }
                        var xDif = Math.Abs(enemy.Position.X - player.Position.X);
                        var yDif = Math.Abs(enemy.Position.Y - player.Position.Y);

                        double curDistance = Math.Sqrt(xDif * xDif + yDif * yDif);

                        if (curDistance <= distance) {
                            target = player;
                            distance = curDistance;
                        }
                    }

                    float dx = -1;
                    float dy = -1;

                    float xDifference = target.Position.X - enemy.Position.X;
                    float yDifference = target.Position.Y - enemy.Position.Y;
                    if (Math.Abs(xDifference) < enemy.enemyMovementSpeed) {
                        dx = 0;
                    }
                    if (Math.Abs(yDifference) < enemy.enemyMovementSpeed) {
                        dy = 0;
                    }

                    if (dx == -1) {
                        dx = 0;
                        if (target.Position.X > enemy.Position.X) {
                            dx += enemy.enemyMovementSpeed;
                        }

                        if (target.Position.X < enemy.Position.X) {
                            dx -= enemy.enemyMovementSpeed;
                        }
                    }

                    if (dy == -1) {
                        dy = 0;
                        if (target.Position.Y > enemy.Position.Y) {
                            dy += enemy.enemyMovementSpeed;
                        }

                        if (target.Position.Y < enemy.Position.Y) {
                            dy -= enemy.enemyMovementSpeed;
                        }
                    }

                    enemy.Position.Add(dx, dy);

                    var correction = this.map.GetMaximumColllisions(enemy);
                    enemy.Position.Add(correction.x, correction.y);
                    enemy.dx = dx;
                    enemy.dy = dy;

                    //if (players[index].Position.Y == enemy.Position.Y && players[index].Position.X == enemy.Position.X)
                    //{
                    //    audio.PlayAudio("Sharp_Punch.flac");
                    //}

                }
            }

            return ControlEvent.NONE;
        }

        private ControlEvent CheckForNewInput() {

            var canvas = GameWindow.Singleton.Canvas;

            for (uint i = 0; i < 4; i++) {
                if (players[i] != null) {
                    continue;
                }

                if (canvas.IsJoystickConnected(i)) {
                    var notification = this.GetElementById(ConnectNotification.ID);
                    if (notification == null) {
                        this.AddChild(new ConnectNotification());
                    }
                }
            }

            return ControlEvent.NONE;
        }

        public override void HandleJoystickButtonPressed(JoystickButtonPressedEvent e) {
            if (e.Button == JoystickButton.Back) {
                SceneManager.Singleton.LoadScene<MainMenu.MainMenu>();
                return;
            }

            if (e.Button == JoystickButton.A) {
                if (players[e.JoystickID] == null) {
                    this.RemoveElementById(ConnectNotification.ID);

                    var newPlayer = new Player(e.JoystickID);
                    this.players[e.JoystickID] = newPlayer;
                    newPlayer.CollisionHandler += this.map.GetMaximumColllisions;
                    newPlayer.BorderCollisionHandler += this.map.GetMapBorderCollisions;
                    this.map.AddEntity(newPlayer);

                    SceneManager.Singleton.LoadScene<CharacterSelection>();
                    var characterSelection = SceneManager.Singleton.CurrentScene as CharacterSelection;
                    characterSelection.EditingPlayer = this.players[e.JoystickID];

                    PlayerOverlay overlay = new PlayerOverlay(newPlayer);
                    this.AddChild(overlay);
                    newPlayer.overlay = overlay;


                    Debug.AddDebugInformation(() => $"Player {e.JoystickID} - X: {(int)newPlayer.Position.X} Y: {(int)newPlayer.Position.Y}");
                    CameraFocus(newPlayer);
                }
            }
            //attack button
            if (e.Button == JoystickButton.B) {
            }
        }

        public void CameraFocus(Player player) {
            var v = player.Position;
            float count = 1;
            for (int i = 0; i < this.players.Length; i++) {
                if (i == player._joystickID) {
                    continue;
                }

                if (this.players[i] == null) {
                    continue;
                }
                v = new OffsetVector(v, this.players[i].Position);
                count++;
            }
            v = new ScalingVector(v, 1 / count, 1 / count);
            GameWindow.Singleton.Canvas.GetCamera().Follow(v);
        }

        public override void HandleKeyboardKeyPressed(KeyboardKeyPressedEvent e) {
            if (e.Key == KeyboardKey.Insert) {
                Debug.ToggleDebugOverlay();
            }

            base.HandleKeyboardKeyPressed(e);

            if (e.Key == KeyboardKey.Escape) {
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.Back
                });
            }

            if (e.Key == KeyboardKey.Tab) {
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.A
                });
            }

            if (e.Handled) {
                return;
            }
        }
    }
}