using UnityEngine;

public class Unit {
    public string unitName;
    public int level;

    public int strength;
    public int defense;

    public int currentHealth;
    public int maxHealth;

    public Unit(string name, int level, int health, int strength, int defense) {
        this.unitName = name;
        this.level = level;
        this.currentHealth = health;
        this.maxHealth = health;
        this.strength = strength;
        this.defense = defense;
    }

    public int TakeDamage(int damage) {
        currentHealth -= damage - defense >= 1 ? damage - defense : 1;
        Debug.Log(unitName + ": " + this.currentHealth + "HP");
        return currentHealth;
    }

    public int Heal(int health) {
        if (currentHealth + health > maxHealth)
            currentHealth = maxHealth;
        else
            currentHealth += health;
        return currentHealth;
    }

}