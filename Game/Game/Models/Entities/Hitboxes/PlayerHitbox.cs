using Annex.Data;

namespace Game.Models.Entities.Hitboxes
{
    public class PlayerHitbox : HitboxEntity
    {
        public readonly Player Player;

        public PlayerHitbox(Player player, uint left, uint top, uint right, uint bottom) : base(player.Position, left, top, right, bottom) {
            this._hitboxSprite.RenderFillColor = new RGBA(0, 0, 255, 100);
            this.Player = player;
        }
    }
}
