using Annex;
using Annex.Audio;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models;
using Game.Models.Chunks;
using Game.Models.Entities;
using Game.Scenes.Stage1.Elements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {


        public List<Enemy> EnemyList;
        public const int playerDisplacement = 10;
        public const int enemyDisplacement = 1;
        public const int timerInterval = 100;
        public const int maximumEnemy = 100;
        public int enemyCounter = 0;


        public readonly Map map;
        public Game.Models.Entities.Player[] players;
        public Annex.Audio.Players.IAudioPlayer audio = AudioManager.Singleton;


        public Stage1()
        {
            players = new Game.Models.Entities.Player[4];
            audio.PlayBufferedAudio("AwesomeMusic.flac", "test", true, 100);

            this.map = new Map();
            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);


            map.AddEntity(new Enemy());

            this.Events.AddEvent("add-new-enemy", PriorityType.LOGIC, AddEnemy, 1000);
            this.Events.AddEvent("update-enemy-positions", PriorityType.LOGIC, UpdateEnemyPositions, 20);

            //events.AddEvent(PriorityType.LOGIC, () =
            //{
            //    //var enemyList = new List<Enemy>();

            //    ////Add the first enemy
            //    //var t = new Enemy();
            //    //enemyList.Add(t);


            //    if (players != null)
            //    {
            //        if (enemyCounter < maximumEnemy)
            //        {
            //            var t = new Enemy();
            //            map.AddEntity(new Enemy());
            //            enemyCounter++;
            //        }


            //        foreach (var enemy in map.)
            //        {
            //            var index = 0;

            //            for (var i = 0; i < (players.Length - 1); i++)
            //            {
            //                var currentPlayerXDifference = Math.Abs(enemy.Position.X - players[i].Position.X);
            //                var currentPlayerYDifference = Math.Abs(enemy.Position.Y - players[i].Position.Y);
            //                var nextPlayerXDifference = Math.Abs(enemy.Position.X - players[i].Position.X);
            //                var nextPlayerYDifference = Math.Abs(enemy.Position.Y - players[i].Position.Y);

            //                if (Math.Sqrt(currentPlayerXDifference * currentPlayerXDifference + currentPlayerYDifference * currentPlayerYDifference) > Math.Sqrt(nextPlayerXDifference * nextPlayerXDifference + nextPlayerYDifference * nextPlayerYDifference))
            //                {
            //                    index = i;
            //                }
            //                else
            //                {
            //                    index = i + 1;
            //                }
            //            }

            //            if (players[index].Position.X > enemy.Position.X)
            //            {
            //                enemy.Position.X += enemy.enemyMovementSpeed;
            //            }

            //            if (players[index].Position.X < enemy.Position.X)
            //            {
            //                enemy.Position.X -= enemy.enemyMovementSpeed;
            //            }

            //            if (players[index].Position.Y > enemy.Position.Y)
            //            {
            //                enemy.Position.Y += enemy.enemyMovementSpeed;
            //            }

            //            if (players[index].Position.Y < enemy.Position.Y)
            //            {
            //                enemy.Position.Y -= enemy.enemyMovementSpeed;
            //            }

            //            if (players[index].Position.Y == enemy.Position.Y && players[index].Position.X == enemy.Position.X)
            //            {
            //                audio.PlayAudio("Sharp_Punch.flac");
            //            }

            //        }
            //    }

            //    return ControlEvent.NONE;
            //}, 1000);

        }

        private ControlEvent AddEnemy()
        {
            map.AddEntity(new Enemy());
            return ControlEvent.NONE;
        }

        private ControlEvent UpdateEnemyPositions()
        {

            foreach (var entity in map.GetEntities(entity => entity.EntityType == EntityType.Enemy))
            {
                var enemy = entity as Enemy;

                var index = 0;

                var playerList = map.GetEntities(entity => entity.EntityType == EntityType.Player);


                for (var i = 0; i < (players.Length - 1); i++)
                {
                    if (players[i] != null)
                    {
                        var currentPlayerXDifference = Math.Abs(enemy.Position.X - players[i].Position.X);
                        var currentPlayerYDifference = Math.Abs(enemy.Position.Y - players[i].Position.Y);

                        var nextPlayerXDifference = currentPlayerXDifference;
                        var nextPlayerYDifference = currentPlayerYDifference;

                        if (players[i + 1] != null)
                        {
                            nextPlayerXDifference = Math.Abs(enemy.Position.X - players[i + 1].Position.X);
                            nextPlayerYDifference = Math.Abs(enemy.Position.Y - players[i + 1].Position.Y);
                        }

                        if (players[i + 1] == null || (Math.Sqrt(currentPlayerXDifference * currentPlayerXDifference + currentPlayerYDifference * currentPlayerYDifference) < Math.Sqrt(nextPlayerXDifference * nextPlayerXDifference + nextPlayerYDifference * nextPlayerYDifference)))
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



                        if (players[index].Position.Y == enemy.Position.Y && players[index].Position.X == enemy.Position.X)
                        {
                            audio.PlayAudio("Sharp_Punch.flac");
                        }
                    }
                }
                  
                
            }

            return ControlEvent.NONE;
        }

        public override void Draw(ICanvas canvas)
        {
            map.Draw(canvas);
            base.Draw(canvas);
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

                    var newPlayer = new Game.Models.Entities.Player(e.JoystickID);
                    this.players[e.JoystickID] = newPlayer;
                    this.players[e.JoystickID].ChunkLoader += map.LoadChunk; // Remove event when changing scenes
                    this.map.AddEntity(newPlayer);

                    Debug.AddDebugInformation(() => $"Player {e.JoystickID} - X: {(int)newPlayer.Position.X} Y: {(int)newPlayer.Position.Y}");

                    // TODO: Move this to its own function.
                    var v = newPlayer.Position;
                    float count = 1;
                    for (int i = 0; i < this.players.Length; i++) {
                        if (i == e.JoystickID) {
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
            }
        }

        public override void HandleKeyboardKeyPressed(KeyboardKeyPressedEvent e) {
            if (e.Key == KeyboardKey.Insert) {
                Debug.ToggleDebugOverlay();
            }
       

            base.HandleKeyboardKeyPressed(e);

            if (e.Handled) {
                return;
            }

        }
    }
}
