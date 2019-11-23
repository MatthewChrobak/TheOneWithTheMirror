using Annex.Data.Shared;
using Game.Models.Chunks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Game
{
    public class Json
    {
        public static void SaveChunk(string chunkFile, MapChunk chunk) {
            var jsonObj = new JObject();

            var x = new JValue(chunk.X);
            var y = new JValue(chunk.Y);

            jsonObj.Add(nameof(MapChunk.X), x);
            jsonObj.Add(nameof(MapChunk.Y), y);

            var tiles = new JArray();
            foreach (var tile in chunk.Tiles) {
                var textureName = new JValue(tile.TextureName.Value);
                var top = new JValue(tile.Rect.Top.Value);
                var left = new JValue(tile.Rect.Left.Value);

                var tileData = new JObject();
                tileData.Add(nameof(Tile.TextureName), textureName);
                tileData.Add(nameof(IntRect.Top), top);
                tileData.Add(nameof(IntRect.Left), left);

                tiles.Add(tileData);
            }
            jsonObj.Add(nameof(MapChunk.Tiles), tiles);

            File.WriteAllText(chunkFile, jsonObj.ToString());
        }

        public static MapChunk LoadChunk(string chunkFile) {

            if (!File.Exists(chunkFile)) {
                return null;
            }

            var jsonObj = JObject.Parse(File.ReadAllText(chunkFile));

            int x = jsonObj[nameof(MapChunk.X)].Value<int>();
            int y = jsonObj[nameof(MapChunk.Y)].Value<int>();

            var chunk = new MapChunk(x, y);

            int i = 0;
            foreach (var entry in jsonObj[nameof(MapChunk.Tiles)]) {
                var tile = chunk.GetTile(i++);

                tile.TextureName.Set(entry[nameof(Tile.TextureName)].Value<string>());
                tile.Rect.Top.Set(entry[nameof(IntRect.Top)].Value<int>());
                tile.Rect.Left.Set(entry[nameof(IntRect.Left)].Value<int>());
            }

            return chunk;
        }
    }
}
