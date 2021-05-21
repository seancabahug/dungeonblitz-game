namespace DungeonBlitz {
    public enum UnitActionType {
        ATTACK,
        HEAL,
        SHIELD
    }

    public class UnitAction {
        private string name;
        private UnitActionType type;
        private int healthPointsInvolved;
        private float cooldown;
        private bool coolingDown;
        public UnitAction(string name, UnitActionType type, int healthPointsInvolved, float cooldown) {
            this.name = name;
            this.type = type;
            this.healthPointsInvolved = healthPointsInvolved;
            this.cooldown = cooldown;
            this.coolingDown = false;
        }
        public UnitActionType Type {
            get => type;
        }

        public int HealthPointsInvolved {
            get => healthPointsInvolved;
        }

        public string Name {
            get => name;
        }

        public float Cooldown {
            get => cooldown;
        }

        public bool CoolingDown {
            get => coolingDown;
            set => coolingDown = value;
        }
    }
}