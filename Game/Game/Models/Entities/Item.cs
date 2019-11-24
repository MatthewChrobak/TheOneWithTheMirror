using System;
using System.Collections.Generic;
using System.Text;
using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;

namespace Game.Models.Entities
{
    class Item : Entity
    {
        public TextureContext _sprite;
        public TextContext _text;
        public string _name = "Test";

        public Item()
        {
            this._sprite = new TextureContext("Clawdia_FacingUpUp.png")
            {
                SourceTextureRect = new IntRect(0, 0, 32, 32),
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-48, -90))
            };

            this._text = new TextContext(this._name, "Default.ttf")
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-44, -70)),
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
        }
    }
}
