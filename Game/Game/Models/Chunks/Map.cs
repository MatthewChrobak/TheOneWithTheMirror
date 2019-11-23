using Annex.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private Dictionary<(int, int), MapChunk> _chunks;

        public Map() {
            this._chunks = new Dictionary<(int, int), MapChunk>();
        }

        public MapChunk GetChunk(int x, int y) {
            LoadChunk(x, y);
            return this._chunks[(x, y)];
        }

        public void Draw(ICanvas canvas) {
            foreach (var value in this._chunks.Values) {
                value.Draw(canvas);
            }
        }

        public void LoadChunk(int x, int y) {
            if (!this._chunks.ContainsKey((x, y))) {
                this._chunks[(x, y)] = new MapChunk(x, y);
            }
        }

        public void UnloadChunk()
        {
            var leastRecentlyusedChunk = this._chunks.Min(a => a.Value);
            if(this._chunks.ContainsKey((leastRecentlyusedChunk.X, leastRecentlyusedChunk.Y)))
            {
                int x = leastRecentlyusedChunk.X;
                int y = leastRecentlyusedChunk.Y;
                this._chunks.Remove((x, y));
            }
            
        }
    }
}
