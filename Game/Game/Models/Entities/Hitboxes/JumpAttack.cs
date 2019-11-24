using Annex.Data;
using Game.Models.Buffs;
using Game.Models.Entities.Items;
using Game.Scenes;

namespace Game.Models.Entities.Hitboxes
{
    public class JumpAttack : HitboxEntity
    {
        public Player Player;
        public int damageConstant = 25;

        public JumpAttack(Player player) : base(player.Position, 20, 20, 20, 20) {
            this.Player = player;
            _hitboxSprite.RenderFillColor = new RGBA(0, 0, 255, 150);
        }

        public override void OnCollision(HitboxEntity entity) {
            if (entity.EntityType == EntityType.Enemy) {

                int playerDamage = this.Player.GetBuff(BuffTypes.Damage) * 10;
                entity.Damage(damageConstant, playerDamage);

                if(entity is Enemy enemy)
                {
                    var damageOverTime = this.Player.GetBuff(BuffTypes.DOT) * 1;
                    enemy.Health.Regen.Value = -damageOverTime * 5f;
                    if(damageOverTime > 0)
                    {
                        enemy.DamageOverTime();
                    }
                }
                                
                var lifestealAmount = this.Player.GetBuff(BuffTypes.Lifesteal) * 0.05f;
                this.Player.Health.Current.Value += (damageConstant + playerDamage) * lifestealAmount;

                var scene = SceneWithMap.CurrentScene;
                scene.AddScrollingMessage(new ScrollingTextMessage(
                    (damageConstant + playerDamage).ToString(), 
                    entity.Position.X, 
                    entity.Position.Y, 
                    RGBA.Red
                ));
            }

            if (entity is Item item) {
                this.Player.SetBuff(item.buffType);

                //remove item after taking it
                var scene = SceneWithMap.CurrentScene;
                var map = scene.map;
                map.RemoveEntity(entity);
            }

            if (entity is Flies fly) {
                var scene = SceneWithMap.CurrentScene;
                scene.map.RemoveEntity(fly);

                this.Player.Health.Current.Value += 10;
            }
        }
    }
}
