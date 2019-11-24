using Annex;
using Annex.Audio;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models;
using Game.Models.Chunks;
using Game.Models.Entities;
using Game.Scenes.CharacterSelect;
using Game.Scenes.Stage1.Elements;
using System;
using System.Linq;


namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public readonly Map map;
        public Player[] players;
        public Enemy[] enemies;

        // MAP EDITING TOOLS
        private uint debugPlayerID = 99;

        public Stage1() {
            this.Size.Set(500, 500);
            players = new Player[6];

            this.map = new Map("stage1");
            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);

            map.AddEntity(new Enemy());

            this.Events.AddEvent("add-new-enemy", PriorityType.LOGIC, AddEnemy, 1000);
            this.Events.AddEvent("update-enemy-positions", PriorityType.LOGIC, UpdateEnemyPositions, 20);

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

            Debug.AddDebugCommand("additem", (data) =>
            {
                map.AddEntity(new Item());
            });
        }

        public override void Draw(ICanvas canvas) {
            map.Draw(canvas);
            base.Draw(canvas);
        }

        private ControlEvent AddEnemy()
        {
            map.AddEntity(new Enemy());
            return ControlEvent.NONE;
        }

        private ControlEvent UpdateEnemyPositions()
        {
            var enemyList = map.GetEntities(entity => entity.EntityType == EntityType.Enemy).ToList();
            foreach (var entity in enemyList)
            {
                var enemy = entity as Enemy;

                var index = 0;

                var playerList = map.GetEntities(entity => entity.EntityType == EntityType.Player);


                for (var i = 0; i < (players.Length - 2); i++)
                {
                    if (players[i] != null)
                    {
                        //Get the current player's X-Y position
                        var currentPlayerXDifference = Math.Abs(enemy.Position.X - players[i].Position.X);
                        var currentPlayerYDifference = Math.Abs(enemy.Position.Y - players[i].Position.Y);

                        //Get the next player's X-Y position
                        var nextPlayerXDifference = currentPlayerXDifference;
                        var nextPlayerYDifference = currentPlayerYDifference;

                        if (players[i + 1] != null)
                        {
                            nextPlayerXDifference = Math.Abs(enemy.Position.X - players[i + 1].Position.X);
                            nextPlayerYDifference = Math.Abs(enemy.Position.Y - players[i + 1].Position.Y);
                        }


                        //Find the shortest distance between an enemy and the players
                        if (players[i + 1] == null || (Math.Sqrt(currentPlayerXDifference * currentPlayerXDifference + currentPlayerYDifference * currentPlayerYDifference) <= Math.Sqrt(nextPlayerXDifference * nextPlayerXDifference + nextPlayerYDifference * nextPlayerYDifference)))
                        {
                            index = i;
                        }
                        else
                        {
                            index = i + 1;
                        }


                        if (players[index].Position.X > enemy.Position.X)
                        {
                            enemy.Position.X += enemy.enemyMovementSpeed;
                        }

                        if (players[index].Position.X < enemy.Position.X)
                        {
                            enemy.Position.X -= enemy.enemyMovementSpeed;
                        }

                        if (players[index].Position.Y > enemy.Position.Y)
                        {
                            enemy.Position.Y += enemy.enemyMovementSpeed;
                        }

                        if (players[index].Position.Y < enemy.Position.Y)
                        {
                            enemy.Position.Y -= enemy.enemyMovementSpeed;
                        }

                        var correction = this.map.GetMaximumColllisions(enemy);
                        enemy.Position.Add(correction.x, correction.y);


                        if (players[index].Position.Y == enemy.Position.Y && players[index].Position.X == enemy.Position.X)
                        {
                            //audio.PlayAudio("Sharp_Punch.flac");
                        }
                    }
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

                if (canvas.IsJoystickConnected(i))
                {
                    var notification = this.GetElementById(ConnectNotification.ID);
                    if (notification == null)
                    {
                        this.AddChild(new ConnectNotification());
                    }
                }
            }

            return ControlEvent.NONE;
        }

        public override void HandleJoystickButtonPressed(JoystickButtonPressedEvent e)
        {
            if (e.Button == JoystickButton.Back)
            {
                SceneManager.Singleton.LoadScene<MainMenu.MainMenu>();
                return;
            }

            if (e.Button == JoystickButton.A) {
                if (players[e.JoystickID] == null) {
                    this.RemoveElementById(ConnectNotification.ID);

                    var newPlayer = new Player(e.JoystickID);
                    this.players[e.JoystickID] = newPlayer;
                    newPlayer.CollisionHandler += this.map.GetMaximumColllisions;
                    this.map.AddEntity(newPlayer);

                    SceneManager.Singleton.LoadScene<CharacterSelection>();
                    var characterSelection = SceneManager.Singleton.CurrentScene as CharacterSelection;
                    characterSelection.EditingPlayer = this.players[e.JoystickID];

                    this.AddChild(new PlayerOverlay(newPlayer));
                    
                    Debug.AddDebugInformation(() => $"Player {e.JoystickID} - X: {(int)newPlayer.Position.X} Y: {(int)newPlayer.Position.Y}");
                    CameraFocus(newPlayer);
                }
            }
            //attack button
            if (e.Button == JoystickButton.B)
            {
                this.players[e.JoystickID].Attack(this.players[e.JoystickID].Position.X, this.players[e.JoystickID].Position.Y, this.map.GetEntities(enemy => enemy.EntityType == EntityType.Enemy));
            }
        }

        public void CameraFocus(Player player)
        {
            var v = player.Position;
            float count = 1;
            for (int i = 0; i < this.players.Length; i++)
            {
                if (i == player._joystickID)
                {
                    continue;
                }

                if (this.players[i] == null)
                {
                    continue;
                }
                v = new OffsetVector(v, this.players[i].Position);
                count++;
            }
            v = new ScalingVector(v, 1 / count, 1 / count);
            GameWindow.Singleton.Canvas.GetCamera().Follow(v);            
        }

        public override void HandleKeyboardKeyPressed(KeyboardKeyPressedEvent e) {
            if (e.Key == KeyboardKey.Tilde) {
                Debug.ToggleDebugOverlay();
            }

            base.HandleKeyboardKeyPressed(e);

            if (e.Key == KeyboardKey.Escape) {
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.Back
                });
            }

            if(e.Key == KeyboardKey.Tab)
            {
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent()
                {
                    Button = JoystickButton.A
                });
            }

            if (e.Handled) {
                return;
            }
        }
    }
}