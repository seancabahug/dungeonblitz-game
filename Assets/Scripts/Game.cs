using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using TMPro;

using DungeonBlitz;

public enum GameState {
    LOBBY,
    STARTING,
    ENCOUNTERING,
    BATTLE_EXECUTION,
    BATTLE_CONCLUSION,
    STARTING_MENU
}

public struct Character {
    public Character(string name, PlayerClass playerClass) {
        this.Name = name;
        this.Class = playerClass;
    }

    public string Name { get; }
    public PlayerClass Class { get; set; }
}

public class Game : MonoBehaviour
{
    private GameState gameState = GameState.STARTING_MENU;

    [Header("Objects")]
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI mainMenuButtonText;
    public LobbySystem lobbySystemComponent;
    private BattleSystem battleSystem = null;

    [Header("Socket")]
    private WebSocketConnection webSocket;

    private List<Character> characters = new List<Character>();

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Application.runInBackground = true;
        webSocket = GetComponent<WebSocketConnection>();
        webSocket.Connect("ws://localhost:3000/");
    }

    public void HandleEvent(string message) {
        Debug.Log(message);

        JObject data = JObject.Parse(message);
        switch ((string) data["event"]) {
            case "roomCreated":
                if (gameState == GameState.LOBBY)
                    subtitleText.text = "Room Code: " + (string) data["roomCode"];
                break;

            case "newPlayer":
                if (gameState == GameState.LOBBY && characters.Count < 4) {
                    int playerNumber = (int)data["playerNumber"];
                    string playerName = (string)data["playerName"];
                    characters.Add(new Character(playerName, (PlayerClass) 0));
                    lobbySystemComponent.NewPlayer(playerName, playerNumber);
                    if (characters.Count == 4) mainMenuButtonText.text = "Start!";
                    else {
                        int playersNeeded = 4 - characters.Count;
                        mainMenuButtonText.text = "Need " + playersNeeded + " more player" + ((playersNeeded > 1) ? "s" : "");
                    }
                }
                break;

            case "playerAction":
                if (gameState == GameState.BATTLE_EXECUTION) {
                    StartCoroutine(battleSystem.HandlePlayerAction((int)data["playerNumber"] - 1, (int)data["actionId"]));
                }
                break;
        }
    }

    public void UpdateGameState(GameState state) {
        gameState = state;
        webSocket.SendData(new {
            action = "updateGameState",
            gameState = (int) state
        });
    }

    public void CreateRoom() {
        webSocket.SendData(new {
            action = "createRoom"
        });
        gameState = GameState.LOBBY;
    }

    public int GetNumberOfPlayers() {
        return characters.Count;
    }

    public GameState GetGameState() {
        return gameState;
    }

    public IEnumerator StartGame() {
        if (gameState == GameState.LOBBY)
        {
            webSocket.SendData(new {
                action = "startGame",
                playerInfo = new []{1, 2, 3, 4}
            });
            for (int i = 3; i > 0; i--)
            {
                subtitleText.text = "Starting in " + i + "...";
                yield return new WaitForSeconds(1f);
            }
            SceneManager.LoadScene("BattleScene");
            // TO-DO: Gracefully transition from lobby to battle scene,
            // with character data from lobby successfully used to
            // instantiate their respective characters in the battle.
        }
    }

    // Theoretically should be called once; should be used
    // to initialize the battle system.
    public List<Character> GetPlayerCharacters() {
        return characters;
    }

    public void SetBattleSystem(BattleSystem battleSystem) {
        this.battleSystem = battleSystem;
    }

    public void GameOver() {
        webSocket.SendData(new {
            action = "endGame"
        });
    }
}
