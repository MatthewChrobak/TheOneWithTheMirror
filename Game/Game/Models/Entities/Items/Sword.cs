using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;

namespace Game.Models.Entities.Items
{
    public class Sword : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public string _name = "Sword";

        public Sword()
        {
            this.EntityType = EntityType.Sword;
            this._sprite = new SpriteSheetContext("Sword_Sprite_Sheet.png", 1, 23)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(32, 32)
            };

            //randomize the sword spanning
            System.Random random = new System.Random();
            var map = SceneManager.Singleton.CurrentScene;
            this.Position.Set(random.Next((int)map.Size.X), random.Next((int)map.Size.Y));


            this._text = new TextContext(this._name, "Default.ttf")
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, 8)),
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
