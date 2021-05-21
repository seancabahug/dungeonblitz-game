using UnityEngine;
using TMPro;

namespace DungeonBlitz {
    public class HUD : MonoBehaviour {
        // TO-DO: add variables for all HUD elements per player
        public TextMeshProUGUI nameDisplay;
        public TextMeshProUGUI hpDisplay;

        public void SetHealth(int currentHealth, int maxHealth){
            hpDisplay.text = currentHealth + "HP";
        }

        public void SetName(string name) {
            nameDisplay.text = name;
        }
    }
}