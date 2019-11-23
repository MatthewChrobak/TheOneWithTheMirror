using Annex.Data;
using Annex.Graphics;
using Annex.Scenes.Components;

namespace Game.Scenes.MainMenu.Elements
{
    public class SplashTitle : Label
    {
        public const string ID = "lblSplashTitle";

        public SplashTitle() : base(ID) {
            this.Position.Set(0, 0);
            this.Caption.Set("Jumpy Jump Game");
            this.FontSize.Set(48);
            this.Font.Set("default.ttf");
            this.RenderText.FontColor = RGBA.White;
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT / 4);
        }
    }
}
