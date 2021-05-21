using UnityEngine;
using TMPro;

public class LobbySystem : MonoBehaviour {
    public Transform characterSelectionDisplayPrefab;
    public Transform[] characterMarkers;

    private Transform[] characterSelectionDisplays = new Transform[4];

    public void NewPlayer(string name, int player) {
        Debug.Log("creating new player " + name);
        Transform display = Instantiate(characterSelectionDisplayPrefab, characterMarkers[player - 1]);
        display.GetComponentInChildren<TextMeshProUGUI>().text = name;
        characterSelectionDisplays[player - 1] = display;
    }

}