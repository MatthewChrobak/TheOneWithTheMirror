using Annex.Data;
using Annex.Graphics;
using Annex.Scenes.Components;

namespace Game.Scenes.CharacterSelect.Elements
{
    class CharacterSelect : Label
    {
        public const string ID = "lblCharacterSelect";

        public CharacterSelect() : base(ID)
        {
            this.Caption.Set("SELECT YOUR CHARACTER");
            this.Font.Set("default.ttf");
            this.Position.Set(0, 0);
            this.FontSize.Set(24);
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT);
            this.RenderText.Alignment.VerticalAlignment = VerticalAlignment.Top;
            this.RenderText.Alignment.HorizontalAlignment = HorizontalAlignment.Center;
            this.RenderText.FontColor = RGBA.White;
            this.RenderText.BorderColor = RGBA.Black;
            this.RenderText.BorderThickness = 2;
        }
    }
}