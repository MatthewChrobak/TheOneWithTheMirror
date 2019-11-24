using Annex.Data;
using Game.Scenes;

namespace Game.Models.Entities.Hitboxes
{
    public class JumpAttack : HitboxEntity
    {
        public JumpAttack(Player player) : base(player.Position, 20, 20, 20, 20) {
            _hitboxSprite.RenderFillColor = new RGBA(0, 0, 255, 150);
        }

        public override void OnCollision(HitboxEntity entity) {
            if (entity.EntityType == EntityType.Enemy) {
                entity.Damage(25);

                var scene = SceneWithMap.CurrentScene;
                scene.AddScrollingMessage(new ScrollingTextMessage(
                    "-25", 
                    entity.Position.X, 
                    entity.Position.Y, 
                    RGBA.Red
                ));
            }
        }
    }
}
