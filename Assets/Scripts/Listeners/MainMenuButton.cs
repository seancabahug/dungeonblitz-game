using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using DungeonBlitz;
public class MainMenuButton : MonoBehaviour
{
    public Game game;
    private int timesClicked = 0;

    public void OnClick() {
        GameState gameState = game.GetGameState();
        if (gameState == GameState.STARTING_MENU) {
            game.CreateRoom();
            GetComponentInChildren<TextMeshProUGUI>().text = "Need 4 more players";
        } else if (gameState == GameState.LOBBY) {
            if (game.GetNumberOfPlayers() == 4 || timesClicked > 3)
                StartCoroutine(game.StartGame());
        }
        timesClicked++;
        // TO-DO: make it a start button too when all players have joined
    }
}
