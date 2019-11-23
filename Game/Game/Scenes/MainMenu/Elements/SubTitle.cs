using Annex.Data;
using Annex.Graphics;
using Annex.Scenes.Components;

namespace Game.Scenes.MainMenu.Elements
{
    public class SubTitle : Label
    {
        public const string ID = "lblSubTitle";

        public SubTitle() : base(ID) {
            this.Position.Set(0, 0);
            this.Caption.Set("press A to start!");
            this.FontSize.Set(12);
            this.Font.Set("default.ttf");
            this.RenderText.FontColor = RGBA.White;
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT);
        }
    }
}
