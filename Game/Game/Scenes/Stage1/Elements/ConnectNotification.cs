using Annex.Data;
using Annex.Graphics;
using Annex.Scenes.Components;

namespace Game.Scenes.Stage1.Elements
{
    public class ConnectNotification : Label
    {
        public const string ID = "lblConnectNotification";

        public ConnectNotification() : base(ID) {
            this.Caption.Set("Press A to connect!");
            this.Font.Set("default.ttf");
            this.Position.Set(0, 0);
            this.FontSize.Set(24);
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT);
            this.RenderText.Alignment.VerticalAlignment = VerticalAlignment.Top;
            this.RenderText.Alignment.HorizontalAlignment = HorizontalAlignment.Left;
            this.RenderText.FontColor = RGBA.White;
            this.RenderText.BorderColor = RGBA.Black;
            this.RenderText.BorderThickness = 2;
        }
    }
}
