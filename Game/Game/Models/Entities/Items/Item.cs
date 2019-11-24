using Game.Models.Buffs;

namespace Game.Models.Entities.Items
{
    public class Item : HitboxEntity
    {
        public BuffTypes buffType;

        public Item() : base(10, 10, 10, 10)
        {
            //randomize item spawning
            System.Random random = new System.Random();
            var xPosition = random.Next(200, 800);
            var yPosition = random.Next(200, 600);
            this.Position.Set(xPosition, yPosition);
        }
    }
}
