using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Scenes
{
    public class ScrollingTextMessage : IDrawableObject
    {
        public TextContext _message;
        private int _updates = 0;
        private const int _numUpdates = 50;

        public ScrollingTextMessage(string message, float x, float y, RGBA color) {
            this._message = new TextContext(message, "default.ttf") {
                RenderPosition = Vector.Create(x, y),
                FontColor = color,
                Alignment = new TextAlignment() {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle,
                    Size = Vector.Create(0, 0)
                }
            };
        }

        public void Draw(ICanvas canvas) {
            canvas.Draw(_message);
        }

        public bool Update() {
            this._message.RenderPosition.Y -= 1;
            this._updates++;

            if (this._updates >= _numUpdates) {
                return true;
            }
            return false;
        }
    }
}
