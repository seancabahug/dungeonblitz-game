using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonBlitz
{
    public class BattleSystem : MonoBehaviour
    {
        public HUDSystem hudSystem;

        private Game game;

        private Player[] players = new Player[4];
        private Enemy enemy;
        private int enemyLevel = 1;
        private string[] possibleEnemyNames = new string[] {
            "Big Zombie",
            "Large Creature",
            "Giant Undead",
            "Colossal Apparition"
        };

        void Start()
        {
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
            game.SetBattleSystem(this);
            List<Character> characters = game.GetPlayerCharacters();
            for (int i = 0; i < characters.Count; i++) {
                players[i] = new Player(characters[i].Name, characters[i].Class, 1, 10, 1, 1);
                hudSystem.DrawPlayer(players[i], i);
            }
            // TO-DO: Finish initialization phase
            StartCoroutine(this.Encounter());
        }

        public IEnumerator Encounter() {
            // TO-DO: run encounter animations, etc.
            game.UpdateGameState(GameState.ENCOUNTERING);
            enemy = new BigZombie(
                possibleEnemyNames[Mathf.FloorToInt(Random.Range(0, 4))],
                enemyLevel,
                enemyLevel * 50,
                Mathf.FloorToInt(Random.Range(enemyLevel - 2 >= 1 ? enemyLevel - 2 : 1, enemyLevel + 2)),
                Mathf.FloorToInt(Random.Range(enemyLevel - 2 >= 1 ? enemyLevel - 2 : 1, enemyLevel + 2))
            );
            enemyLevel++; // Increasing for future enemies
            hudSystem.DrawNewEnemyBeforeEncounter(enemy);
            hudSystem.SetupEnemyHUD(enemy.unitName, enemy.level);
            StartCoroutine(hudSystem.MoveCamera(3, 17.79f));
            yield return new WaitForSeconds(3);
            for (int i = 3; i >= 0; i--) {
                hudSystem.SetMiddleText(i > 0 ? i + "..." : "FIGHT!");
                yield return new WaitForSeconds(0.7f);
            }
            hudSystem.SetMiddleText("");
            game.UpdateGameState(GameState.BATTLE_EXECUTION);
            StartCoroutine(EnemyFightLoop());
        }

        public IEnumerator EnemyFightLoop() {
            while (enemy.currentHealth > 0 && !AllPlayersDead()) {
                UnitAction enemyAction = enemy.ExecuteAction();
                switch (enemyAction.Type) {
                    case UnitActionType.ATTACK:
                        int randomPlayerId = Mathf.FloorToInt(Random.Range(0, 4));
                        while (players[randomPlayerId].currentHealth <= 0) {
                            randomPlayerId = Mathf.FloorToInt(Random.Range(0, 4));
                        }
                        Player victim = players[randomPlayerId];
                        victim.TakeDamage(enemyAction.HealthPointsInvolved + enemy.strength);
                        hudSystem.SetPlayerHealth(randomPlayerId, victim.currentHealth, victim.maxHealth);
                        StartCoroutine(hudSystem.ShakeCamera(5, 0.3f));
                        StartCoroutine(hudSystem.SpawnPlayerDamageIndicator(randomPlayerId, enemyAction.HealthPointsInvolved));
                        if (AllPlayersDead()) {
                            hudSystem.SetMiddleText("Everyone is dead!");
                            game.GameOver();
                            yield return new WaitForSeconds(2);
                            Destroy(game.gameObject);
                            SceneManager.LoadScene("MainMenu");
                        }
                        break;
                }
                yield return new WaitForSeconds(enemyAction.Cooldown);
            }
        }

        public IEnumerator HandlePlayerAction(int playerID, int actionID) {
            Player player = players[playerID];
            if (player.currentHealth > 0)
            {
                Debug.Log("handling player action");
                UnitAction action = players[playerID].MoveList[actionID];
                if (!action.CoolingDown)
                {
                    switch (action.Type)
                    {
                        case UnitActionType.ATTACK:
                            int damage = action.HealthPointsInvolved + player.strength;
                            enemy.TakeDamage(damage);
                            StartCoroutine(hudSystem.SpawnEnemyDamageIndicator(damage));
                            hudSystem.SetEnemyHealth(enemy.currentHealth, enemy.maxHealth);
                            StartCoroutine(hudSystem.ShakeCamera(3, 0.3f));
                            if (enemy.currentHealth <= 0)
                            {
                                // TO-DO: stop enemy game loop, clean extra
                                // stuff up
                                StartCoroutine(ConcludeBattle());
                            }
                            break;
                        case UnitActionType.HEAL:
                            break;
                    }
                    action.CoolingDown = true;
                    yield return new WaitForSeconds(action.Cooldown);
                    action.CoolingDown = false;
                }
            }
        }

        public IEnumerator ConcludeBattle() {
            game.UpdateGameState(GameState.BATTLE_CONCLUSION);
            Debug.Log("Battle concluded");
            hudSystem.DeleteEnemy();
            hudSystem.SetMiddleText("Victory!");
            yield return new WaitForSeconds(2);
            hudSystem.SetMiddleText("");
            // TO-DO: level ups, stat buffs, clean everything up and
            // prep for the next encounter
            for (int i = 0; i < players.Length; i++) {
                Player player = players[i];
                player.level++;
                player.maxHealth += 2;
                player.currentHealth += 2;
                player.strength += 2;
                player.defense += 2;
                hudSystem.SetPlayerHealth(i, player.currentHealth, player.maxHealth);
            }
            enemy = null;
            StartCoroutine(Encounter());
        }

        public bool AllPlayersDead() {
            foreach (Player player in players) {
                if (player.currentHealth > 0) return false;
            }
            return true;
        }
    }
}