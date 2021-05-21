using System.Collections.Generic;
using UnityEngine;

namespace DungeonBlitz
{
    public static class PlayerClassExt {
        public static List<UnitAction> GetClassActions(this PlayerClass playerClass) {
            switch (playerClass) {
                case PlayerClass.NECROMANCER:
                    return new List<UnitAction>() {
                        new UnitAction("Hit up Enemy's great great grandfather", UnitActionType.ATTACK, 2, 0.2f)
                    };
                default:
                    return new List<UnitAction>(){};
            }
        }
    }

    public enum PlayerClass {
        NECROMANCER
    }

    public class Player : Unit {

        private List<UnitAction> moveList;
        private PlayerClass playerClass;
        public Player(string name, PlayerClass playerClass, int level, int health, int strength, int defense):
            base(name, level, health, strength, defense)
        {
            this.moveList = playerClass.GetClassActions();
            this.playerClass = playerClass;
        }

        public ref List<UnitAction> MoveList {
            get => ref moveList;
        }

        public PlayerClass Class {
            get => playerClass;
        }
    }
}