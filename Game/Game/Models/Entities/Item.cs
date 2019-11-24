using System;
using System.Collections.Generic;
using System.Text;
using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models.Entities
{
    class Item : HitboxEntity
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public string _name = "Test";

        public Item() : base(5, 5, 5, 5)
        {
            this._sprite = new SpriteSheetContext("Clawdia_Direction_Anim.png", 4, 8)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(32, 32)
            };


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
