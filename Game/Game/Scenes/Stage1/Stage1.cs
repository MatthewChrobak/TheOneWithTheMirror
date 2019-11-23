using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models.Chunks;
using Game.Scenes.Stage1.Elements;
using System.Collections.Generic;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public Map map;
        public Player[] players;

        public Stage1() {
            players = new Player[4];

            this.map = new Map();
            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);
        }

        public override void Draw(ICanvas canvas)
        {
            map.Draw(canvas);
            for (int i = 0; i < this.players.Length; i++) {
                this.players[i]?.Draw(canvas);
            }
            base.Draw(canvas);
        }

        private HashSet<uint> _playerLoaded = new HashSet<uint>();
        private ControlEvent CheckForNewInput() {
            var canvas = GameWindow.Singleton.Canvas;

            for (uint i = 0; i < 4; i++) {
                if (_playerLoaded.Contains(i)) {
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
                if (!_playerLoaded.Contains(e.JoystickID)) {
                    this.RemoveElementById(ConnectNotification.ID);

                    _playerLoaded.Add(e.JoystickID);
                    this.players[e.JoystickID] = new Player(e.JoystickID);
                    this.players[e.JoystickID].OnPlayerMovedToNewChunk += LoadNearChunks; // Remove event when changing scenes

                    var v = this.players[e.JoystickID].Position;
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

        public void LoadNearChunks(Player player, int x, int y)
        {            
            for (int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (x + i == x && y + j == y)
                    {
                        player.SetCurrentChunk(map.GetChunk(x, y));
                        continue;
                    }
                    map.GetChunk(x + i, y + j);
                }
            }
        }
    }
}
