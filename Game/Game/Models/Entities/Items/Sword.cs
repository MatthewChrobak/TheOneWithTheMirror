using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Buffs;
using Game.Models.Chunks;

namespace Game.Models.Entities.Items
{
    public class Sword : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public string _name = "Sword";

        public Sword() : base()
        {
            this.EntityType = EntityType.Sword;
            this.buffType = BuffTypes.Damage;
            this._sprite = new SpriteSheetContext("Sword_Sprite_Sheet.png", 1, 23)
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
