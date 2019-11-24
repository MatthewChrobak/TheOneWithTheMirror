﻿using Annex.Data;
using Annex.Data.Shared;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Game.Models.Buffs;
using Game.Models.Chunks;

namespace Game.Models.Entities.Items
{
    public class PoisonArrow : Item
    {
        public SpriteSheetContext _sprite;
        public TextContext _text;
        public String _name = "Sword";

        public PoisonArrow()
        {
            this.EntityType = EntityType.PoisonArrow;
            this._sprite = new SpriteSheetContext("Sword_Sprite_Sheet.png", 1, 23)
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, -16)),
                RenderSize = Vector.Create(32, 32)
            };

            //randomize the sword spanning
            System.Random random = new System.Random();
            this.Position.Set(random.Next(Map.Size_X), random.Next(Map.Size_Y));

            this._text = new TextContext(this._name, "Default.ttf")
            {
                RenderPosition = new OffsetVector(this.Position, Vector.Create(-16, 8)),
                FontColor = RGBA.White,
                BorderColor = RGBA.Black,
                BorderThickness = 2,
                FontSize = 12
            };

            buffType = BuffTypes.Damage;
        }

        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(this._sprite);
            canvas.Draw(this._text);
            base.Draw(canvas);
        }
    }
}
