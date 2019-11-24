using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Buffs;
using Game.Models.Chunks;

namespace Game.Models.Entities.Items
{
    public class PoisonBow : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public String _name = "Poison Bow";

        public PoisonBow() : base()
        {
            this.EntityType = EntityType.PoisonBow;
            this.buffType = BuffTypes.DOT;
            this._sprite = new SpriteSheetContext("poison_bow.png", 1, 13)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(40, 40)
            };

            this._text = new TextContext(this._name, "Default.ttf")
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-30, 17)),
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
