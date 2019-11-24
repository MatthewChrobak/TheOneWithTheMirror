using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes.Components;
using Game.Models.Buffs;
using Game.Models.Entities;

namespace Game.Scenes.Stage1.Elements
{
    public class PlayerOverlay : UIElement
    {
        private Player _player;
        public static int NumOverlays = 0;

        private static float width => GameWindow.RESOLUTION_WIDTH / 3;
        private static float height => GameWindow.RESOLUTION_HEIGHT / 6;

        private TextureContext _icon;
        private TextureContext _healthbar;
        private TextContext _healthPercentage;

        private TextureContext[] _buffIcons;
        private TextContext[] _buffCount;

        public PlayerOverlay(Player player) : base("") {
            this._player = player;

            float halfHeight = height / 2;
            float quarterHeight = halfHeight / 2;
            float eigthHeight = quarterHeight / 2;
            float sixteenthHeight = eigthHeight / 2;
            this._icon = new TextureContext("shia.png") {
                RenderPosition = GetPosition(),
                RenderSize = Vector.Create(halfHeight, halfHeight),
                UseUIView = true
            };
            this._healthbar = new TextureContext("healthbar.png") {
                RenderPosition = new OffsetVector(GetPosition(), quarterHeight, sixteenthHeight),
                RenderSize = Vector.Create(width - (quarterHeight + halfHeight), eigthHeight),
                UseUIView = true
            };
            this._healthPercentage = new TextContext("100%", "default.ttf") {
                RenderPosition = GetPosition(),
                FontSize = 24,
                BorderColor = RGBA.Black,
                BorderThickness = 2,
                FontColor = RGBA.White,
                Alignment = new TextAlignment() {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Size = Vector.Create(width, halfHeight)
                },
                UseUIView = true
            };

            NumOverlays++;

            this._buffCount = new TextContext[(int)BuffTypes.COUNT];
        }

        public static Vector GetPosition() {
            switch (NumOverlays) {
                case 0:
                    return Vector.Create(0 * width, 0 * height);
                case 1:
                    return Vector.Create(2 * width, 0 * height);
                case 2:
                    return Vector.Create(0 * width, 5 * height);
                case 3:
                    return Vector.Create(2 * width, 5 * height);
                case 4:
                    return Vector.Create(1 * width, 0 * height);
                case 5:
                    return Vector.Create(1 * width, 5 * height);
                default:
                    return null;
            }
        }

        public override void Draw(ICanvas canvas) {
            canvas.Draw(this._healthbar);
            canvas.Draw(this._healthPercentage);
            canvas.Draw(this._icon);
        }
    }
}
