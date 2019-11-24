using Annex;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models.Chunks;
using Game.Models.Entities;
using Game.Scenes.CharacterSelect;
using Game.Scenes.Stage1.Elements;
using System;
using System.Collections.Generic;

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
        public string MapBrush_Mode = "single";

        public Stage1() {
            players = new Player[4];

            this.map = new Map("stage1");
            this.Events.AddEvent("HandleNewConnections", PriorityType.INPUT, CheckForNewInput, 5000, 500);
            this.Events.AddEvent("IsChunkRemovable", PriorityType.LOGIC, IsChunkRemovable, 5000);

            Debug.AddDebugCommand("savemap", (data) => {
                map.Save();
            });
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
                    this.players[0]?.HasMovedToNewChunk();

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
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.A,
                    JoystickID = 0
                });
            });

            Debug.AddDebugCommand("setbrush", (data) => {
                this.MapBrush_Texture = data[0];
                this.MapBrush_Top = int.Parse(data[1]);
                this.MapBrush_Left = int.Parse(data[2]);
            });
            Debug.AddDebugCommand("setbrushmode", (data) => {
                this.MapBrush_Mode = data[0].ToLower();

                if (this.MapBrush_Mode == "random") {
                    for (int i = 1; i < data.Length; i++) {
                        // TODO:
                    }
                }
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
                    this.players[e.JoystickID].ChunkLoader += map.LoadChunk; // Remove event when changing scenes
                    this.map.AddEntity(newPlayer);

                    SceneManager.Singleton.LoadScene<CharacterSelection>();
                    var characterSelection = SceneManager.Singleton.CurrentScene as CharacterSelection;
                    characterSelection.EditingPlayer = this.players[e.JoystickID];  
                    
                    Debug.AddDebugInformation(() => $"Player {e.JoystickID} - X: {(int)newPlayer.Position.X} Y: {(int)newPlayer.Position.Y}");
                    CameraFocus(newPlayer);
                }
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
            if (e.Key == KeyboardKey.Insert) {
                Debug.ToggleDebugOverlay();
            }

            base.HandleKeyboardKeyPressed(e);

            if (e.Key == KeyboardKey.Escape) {
                this.HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.Back
                });
            }

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

            if (e.Button == MouseButton.Left)
            {
                this.RemoveBrushEvent = true;
            }

        }

        private ControlEvent IsChunkRemovable()
        {
            foreach(var player in this.players)
            {
                if(player != null)
                    this.map.UnloadChunk(player.CurrentXChunkID, player.CurrentYChunkID);
            }

            return ControlEvent.NONE;
        }
    }
}