using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models.Chunks
{
    public class Tile : IDrawableObject
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;
        private Vector _renderOffset;

        private readonly TextureContext _tileSprite;

        public Tile(Vector renderOffset, int x, int y) {
            this._renderOffset = renderOffset;

            this._tileSprite = new TextureContext("grasstiles.png") {
                RenderPosition = new OffsetVector(
                    new ScalingVector(Vector.Create(x, y),
                    TileWidth, TileHeight),
                    _renderOffset),
                SourceTextureRect =  new IntRect(0 , 64, 32, 32),
                RenderSize = Vector.Create(TileWidth, TileHeight)
            };
        }

        public void Draw(ICanvas canvas) {
            canvas.Draw(_tileSprite);
        }
    }
}
