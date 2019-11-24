using Annex.Data;
using Annex.Graphics;
using Annex.Scenes.Components;

namespace Game.Scenes.MainMenu.Elements
{
    public class SubTitle : Label
    {
        public const string ID = "lblSubTitle";

        public SubTitle() : base(ID) {
            this.Position.Set(50, 0);
            this.Caption.Set("press A to start!");
            this.FontSize.Set(24);
            this.Font.Set("default.ttf");
            this.RenderText.FontColor = RGBA.White;
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT - 100);

            this.RenderText.Alignment = new TextAlignment() {
                Size = this.Size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };
        }
    }
}
