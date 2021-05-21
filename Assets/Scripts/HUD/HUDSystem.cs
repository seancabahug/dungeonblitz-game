using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DungeonBlitz;
using Cinemachine;

public class HUDSystem : MonoBehaviour {
    public new Transform camera;
    public Transform hudObject;
    public Transform canvas;
    public GameObject playerHUDPrefab;
    public TextMeshProUGUI middleText;
    public GameObject damageIndicatorPrefab;
    public Transform[] playerSpriteMarkers;
    public GameObject enemySpritePrefab;
    public GameObject playerSpritePrefab;
    public Animator[] loadedPlayerAnimators = new Animator[4];
    public RuntimeAnimatorController[] classAnimatorControllers;
    public Slider enemyHealthBar;
    public TextMeshProUGUI enemyNameText;
    public CinemachineVirtualCamera cinemachineVirtualCamera;

    private float cameraTransitionSeconds = 3;
    private float cameraTransitionFinalDelta = 17.79f;
    private bool movingCamera = false;
    private GameObject enemySprite;
    private HUD[] playerHUDs = new HUD[4];

    void Update() {
        if (movingCamera) {
            camera.position += new Vector3(cameraTransitionFinalDelta / cameraTransitionSeconds * Time.deltaTime, 0);
        }
    }

    public IEnumerator MoveCamera(float seconds, float units) {
        cameraTransitionSeconds = seconds;
        cameraTransitionFinalDelta = units;
        movingCamera = true;
        yield return new WaitForSeconds(seconds);
        movingCamera = false;
    }

    public void DeleteEnemy() {
        Destroy(enemySprite);
        enemySprite = null;
    }

    public void DrawNewEnemyBeforeEncounter(Enemy enemy) {
        enemySprite = Instantiate(enemySpritePrefab, new Vector3(
            camera.position.x + 17.79f + 4.49f,
            0.804f,
            3f
        ), Quaternion.identity);
    }

    public void DrawPlayer(Player player, int playerID) {

        // Drawing sprite
        GameObject playerObject = Instantiate(playerSpritePrefab, playerSpriteMarkers[playerID]);
        loadedPlayerAnimators[playerID] = playerObject.GetComponent<Animator>();
        loadedPlayerAnimators[playerID].runtimeAnimatorController = classAnimatorControllers[(int)player.Class];

        // Adding the HUD
        GameObject hudGameObject = Instantiate(playerHUDPrefab, hudObject);
        hudGameObject.GetComponent<RectTransform>().localPosition = new Vector3(-300 + 200 * playerID, 0);
        HUD hud = hudGameObject.GetComponent<HUD>();
        hud.SetName(player.unitName);
        hud.SetHealth(player.currentHealth, player.maxHealth);
        playerHUDs[playerID] = hud;
    }

    public void SetPlayerAnimationParameter(int playerID, string parameter, bool value) {
        loadedPlayerAnimators[playerID].SetBool(parameter, value);
    }

    public void SetMiddleText(string text) {
        middleText.text = text;
    }

    public void SetPlayerHealth(int playerID, int currentHealth, int maxHealth) {
        playerHUDs[playerID].SetHealth(currentHealth, maxHealth);
    }

    public IEnumerator SpawnPlayerDamageIndicator(int playerID, int damage) {
        GameObject damageIndicator = Instantiate(damageIndicatorPrefab, canvas);
        damageIndicator.transform.position = playerSpriteMarkers[playerID].position;
        damageIndicator.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        yield return new WaitForSeconds(damageIndicator.GetComponent<Animation>().clip.length);
        Destroy(damageIndicator);
    }

    public IEnumerator SpawnEnemyDamageIndicator(int damage) {
        GameObject damageIndicator = Instantiate(damageIndicatorPrefab, canvas);
        damageIndicator.transform.position = enemySprite.transform.position;
        damageIndicator.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        yield return new WaitForSeconds(damageIndicator.GetComponent<Animation>().clip.length);
        Destroy(damageIndicator);
    }

    public void SetupEnemyHUD(string name, int level) {
        enemyNameText.text = "LV" + level + " " + name;
        enemyHealthBar.value = 1f;
    }

    public void SetEnemyHealth(int currentHealth, int maxHealth) {
        enemyHealthBar.value = currentHealth >= 0 ? currentHealth / (float)maxHealth : 0f;
    }

    public IEnumerator ShakeCamera(float intensity, float duration) {
        CinemachineBasicMultiChannelPerlin noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0;
    }
}