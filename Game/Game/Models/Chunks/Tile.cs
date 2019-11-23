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

        public String TextureName;
        public IntRect Rect;
        private readonly TextureContext _tileSprite;

        public Tile(Vector renderOffset, int x, int y) {
            this._renderOffset = renderOffset;

            this.TextureName = new String("grasstiles.png");
            this.Rect = new IntRect(0, 64, 32, 32);
            this._tileSprite = new TextureContext(this.TextureName) {
                RenderPosition = new OffsetVector(
                    new ScalingVector(Vector.Create(x, y),
                    TileWidth, TileHeight),
                    _renderOffset),
                SourceTextureRect =  Rect,
                RenderSize = Vector.Create(TileWidth, TileHeight)
            };
        }

        public void Draw(ICanvas canvas) {
            canvas.Draw(_tileSprite);
        }
    }
}
