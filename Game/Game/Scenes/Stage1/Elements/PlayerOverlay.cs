using Annex.Graphics.Contexts;
using Game.Models.Buffs;
using Game.Models.Entities;

namespace Game.Scenes.Stage1.Elements
{
    public class PlayerOverlay
    {
        private Player _player;
        public static int NumOverlays = 0;

        private TextureContext _icon;
        private TextureContext _healthbar;
        private TextContext _healthPercentage;

        private TextureContext[] _buffIcons;
        private TextContext[] _buffCount;

        public PlayerOverlay(Player player) {
            this._player = player;

            this._buffCount = new TextContext[(int)BuffTypes.COUNT];
        }
    }
}
