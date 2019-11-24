using System;
using Annex;
using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;

namespace Game.Models.Chunks
{
    public class MapChunk : IDrawableObject
    {
        public int X, Y;

        public long lastTimeUsed = EventManager.CurrentTime;

        public const int X_Tiles = 10;
        public const int Y_Tiles = 10;

        public const int ChunkHeight = Y_Tiles * Tile.TileHeight;
        public const int ChunkWidth = X_Tiles * Tile.TileWidth;

        public readonly Vector RenderOffset;

        public readonly Tile[] Tiles;

        public MapChunk(int x, int y) {
            this.X = x;
            this.Y = y;
            this.Tiles = new Tile[X_Tiles * Y_Tiles];
            this.RenderOffset = new ScalingVector(Vector.Create(x, y), ChunkWidth, ChunkHeight);

            for (int i = 0; i < Tiles.Length; i++) {
                Tiles[i] = new Tile(RenderOffset, i % X_Tiles, i / X_Tiles);
            }
        }

        public void Draw(ICanvas canvas) {
            for (int y = 0; y < Y_Tiles; y++) { 
                for (int x = 0; x < X_Tiles; x++) {
                    this.GetTile(x, y).Draw(canvas);
                }
            }
        }

        public Tile GetTile(int x, int y) {
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);
            Debug.Assert(x < X_Tiles);
            Debug.Assert(y < Y_Tiles);

            return this.Tiles[X_Tiles * y + x];
        }

        public Tile GetTile(int i) {
            return GetTile(i % X_Tiles, i / X_Tiles);
        }
    }
}
