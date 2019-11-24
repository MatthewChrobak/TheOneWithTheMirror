using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Models.Entities
{
    public interface ICombatSystem
    {
        public void Attack(float x, float y, IEnumerable<Entity> entity) { }
    }
}
