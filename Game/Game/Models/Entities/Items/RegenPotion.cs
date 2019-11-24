using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Buffs;
using Game.Models.Chunks;
namespace Game.Models.Entities.Items
{
    public class RegenPotion : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public string _name = "Regen Potion";

        public RegenPotion()
        {
            this.EntityType = EntityType.RegenPotion;
            this.buffType = BuffTypes.Regen;
            this._sprite = new SpriteSheetContext("regen_potion.png", 1, 10)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(32, 32)
            };

            //randomize the sword spanning
            System.Random random = new System.Random();
            this.Position.Set(random.Next(Map.Size_X), random.Next(Map.Size_Y));

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
