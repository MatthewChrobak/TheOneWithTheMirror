using Annex.Graphics;
using System.Collections.Generic;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private Dictionary<(int, int), MapChunk> _chunks;

        public Map()
        {
            this._chunks = new Dictionary<(int, int), MapChunk>();
        }

        public MapChunk GetChunk(int x, int y)
        {
            LoadChunk(x, y);
            return this._chunks[(x, y)];
        }

        public void Draw(ICanvas canvas)
        {
            foreach (var value in this._chunks.Values)
            {
                value.Draw(canvas);
            }
        }

        public void LoadChunk(int x, int y)
        {
            if (!this._chunks.ContainsKey((x, y)))
            {
                this._chunks[(x, y)] = new MapChunk(x, y);
            }
        }

        public void UnloadChunk(int currentPlayerX, int currentPlayerY, bool isChunkEdited = false)
        {
            const int offSetConstant = 1;
            HashSet<(int, int)> chunkAroundPlayer = new HashSet<(int, int)>();

            //TODO: to refactor the unload chunks
            //foreach player:
            //    for x= -1 to 1
            //        for y = -1 to 1:
            //            chunkAroundPlayer.Add(Player.x + x )

            //add current player position
            chunkAroundPlayer.Add((currentPlayerX, currentPlayerY));
            //check north
            if (this._chunks.ContainsKey((currentPlayerX, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX, currentPlayerY + offSetConstant));
            //check west
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY));
            //check south
            if (this._chunks.ContainsKey((currentPlayerX, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX, currentPlayerY - offSetConstant));
            //check east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY));
            //check west-north
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY + offSetConstant));
            //check west-south
            if (this._chunks.ContainsKey((currentPlayerX - offSetConstant, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX - offSetConstant, currentPlayerY - offSetConstant));
            //check south-east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY - offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY - offSetConstant));
            //check north-east
            if (this._chunks.ContainsKey((currentPlayerX + offSetConstant, currentPlayerY + offSetConstant)))
                chunkAroundPlayer.Add((currentPlayerX + offSetConstant, currentPlayerY + offSetConstant));

            //remove all chunk that are not in the hashset
            foreach (var entry in this._chunks)
            {
                if (chunkAroundPlayer.Contains(entry.Key))
                {
                    continue;
                }
                this._chunks.Remove((entry.Value.X, entry.Value.Y));
            }
        }
    }
}
