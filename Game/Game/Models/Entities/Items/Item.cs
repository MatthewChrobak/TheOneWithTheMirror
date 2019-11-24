using Game.Models.Buffs;

namespace Game.Models.Entities.Items
{
    public class Item : HitboxEntity
    {
        public BuffTypes buffType;

        public Item() : base(10, 10, 10, 10)
        {

        }
    }
}
