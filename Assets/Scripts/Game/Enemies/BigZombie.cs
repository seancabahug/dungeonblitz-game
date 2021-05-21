using System.Collections.Generic;

namespace DungeonBlitz {
    public class BigZombie : Enemy {
        public BigZombie(string name, int level, int health, int strength, int defense) :
            base(name, level, health, strength, defense)
        {}

        public override UnitAction ExecuteAction() {
            return new UnitAction("yeet", UnitActionType.ATTACK, 1, 1f);
        }
    }
}