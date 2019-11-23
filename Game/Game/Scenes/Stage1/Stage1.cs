using Annex;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models.Chunks;
using Game.Models.Entities;
using Game.Scenes.Stage1.Elements;
using System;

namespace Game.Scenes.Stage1
{
    public class Stage1 : Scene
    {
        public readonly Map map;
        public Player[] players;

        // MAP EDITING TOOLS
        public string MapBrush_Texture;
        public int MapBrush_Top;
        public int MapBrush_Left;

        public Stage1() {
            players = new Player[4];

            this.map = new Map("stage1");
            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);

            Debug.AddDebugCommand("savemap", (data) => {
                map.Save();
            });

            Debug.AddDebugCommand("setbrush", (data) => {
                this.MapBrush_Texture = data[0];
                this.MapBrush_Top = int.Parse(data[1]);
                this.MapBrush_Left = int.Parse(data[2]);
            });
        }

        public override void Draw(ICanvas canvas) {
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

                    var newPlayer = new Player(e.JoystickID);
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

        private bool RemoveBrushEvent = false;
        public override void HandleMouseButtonPressed(MouseButtonPressedEvent e) {
            base.HandleMouseButtonPressed(e);

            if (e.Handled) {
                return;
            }

            if (e.Button == MouseButton.Left) {
                if (MapBrush_Texture != null) {
                    this.RemoveBrushEvent = false;
                    var canvas = GameWindow.Singleton.Canvas;
                    this.Events.AddEvent("map-brush", PriorityType.GRAPHICS, () => {
                        var pos = canvas.GetGameWorldMousePos();

                        int chunkXID = (int)Math.Floor(pos.X / MapChunk.ChunkWidth);
                        int chunkYID = (int)Math.Floor(pos.Y / MapChunk.ChunkHeight);

                        int relativeX = (int)pos.X % MapChunk.ChunkWidth;
                        int relativeY = (int)pos.Y % MapChunk.ChunkHeight;

                        if (relativeX < 0) {
                            relativeX += MapChunk.ChunkWidth;
                        }

                        if (relativeY < 0) {
                            relativeY += MapChunk.ChunkHeight;
                        }

                        int tileX = relativeX / Tile.TileWidth;
                        int tileY = relativeY / Tile.TileHeight;

                        var chunk = this.map.GetChunk(chunkXID, chunkYID);
                        var tile = chunk.GetTile(tileX, tileY);

                        tile.TextureName.Set(this.MapBrush_Texture);
                        tile.Rect.Top.Set(this.MapBrush_Top);
                        tile.Rect.Left.Set(this.MapBrush_Left);

                        if (this.RemoveBrushEvent) {
                            return ControlEvent.REMOVE;
                        }
                        return ControlEvent.NONE;
                    }, 50);
                }
            }
        }

        public override void HandleMouseButtonReleased(MouseButtonReleasedEvent e) {
            base.HandleMouseButtonReleased(e);

            if (e.Handled) {
                return;
            }

            if (e.Button == MouseButton.Left) {
                this.RemoveBrushEvent = true; 
            }
        }
    }
}
