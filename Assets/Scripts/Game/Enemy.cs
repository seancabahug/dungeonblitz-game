using System.Collections.Generic;
using UnityEngine;

namespace DungeonBlitz {
    public abstract class Enemy : Unit {
        public Enemy(string name, int level, int health, int strength, int defense) :
            base(name, level, health, strength, defense)
        {}

        public abstract UnitAction ExecuteAction();
    }
}