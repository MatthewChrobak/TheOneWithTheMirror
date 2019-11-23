using Annex;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models.Chunks;
using Game.Models.Player;
using Game.Scenes.Stage1.Elements;
using System.Collections.Generic;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public readonly Map map;
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
                    this.players[e.JoystickID].ChunkLoader += map.LoadChunk; // Remove event when changing scenes

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
