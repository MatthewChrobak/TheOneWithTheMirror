using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Buffs;
using Game.Models.Chunks;

namespace Game.Models.Entities.Items
{
    public class Shield : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public string _name = "Shield";

        public Shield() : base()
        {
            this.EntityType = EntityType.Shield;
            this.buffType = BuffTypes.Shield;
            this._sprite = new SpriteSheetContext("shield.png", 1, 10)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(32, 32)
            };

            this._text = new TextContext(this._name, "Default.ttf")
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, 12)),
                FontColor = RGBA.White,
                BorderColor = RGBA.Black,
                BorderThickness = 2,
                FontSize = 12
            };
        }

        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
            canvas.Draw(this._text);
            base.Draw(canvas);
        }
    }
}
