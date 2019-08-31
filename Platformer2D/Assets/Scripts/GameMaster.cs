using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    [SerializeField]
    private int maxLives = 3;

    private static int _remainingLives;
    public static int RemainingLives {
        get { return _remainingLives; }
    }

    private AudioSource countdownSound;

    private void Awake() {
        if(gm == null) {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
        countdownSound = GetComponent<AudioSource>();
    }

    public Transform spawnPrefab;
    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 4;
    public string respawnCountdownSoundName = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public string gameOverSoundName = "GameOver";

    public CameraShake cameraShake;

    [SerializeField] private int startingMoney = 0;
    public static int Money;

    [SerializeField]
    private GameObject gameOverUI = null;

    [SerializeField]
    private GameObject upgradeMenu = null;

    [SerializeField]
    private WaveSpawner waveSpawner = null;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;


    private AudioManager audioManager;

    private void Start() {
        if(cameraShake == null) {
            Debug.LogError("No camera shake referenced in GameMaster");
        }

        if(gameOverUI == null) {
            Debug.LogError("No gmame over UI referenced in GM!");
        }

        _remainingLives = maxLives;

        Money = startingMoney;

        audioManager = AudioManager.instance;
        if(audioManager == null) {
            Debug.LogError("No audiomanager found in scene");
        }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            ToggleUpgradeMenu();
        }
    }

    private void ToggleUpgradeMenu() {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        waveSpawner.enabled = (!upgradeMenu.activeSelf);
        onToggleUpgradeMenu.Invoke(upgradeMenu.activeSelf);
    }

    IEnumerator RespawnPlayer() {

        audioManager.PlaySound(respawnCountdownSoundName);
        yield return new WaitForSeconds(spawnDelay);

        audioManager.PlaySound(spawnSoundName);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void EndGame() {
        Debug.Log("Game Over!");
        gameOverUI.SetActive(true);
        audioManager.PlaySound(gameOverSoundName);
    }

    public static void KillPlayer(Player player) {
        Destroy(player.gameObject);
        _remainingLives -= 1;
        if(_remainingLives <= 0){
            gm.EndGame();
        }
        else {
            gm.StartCoroutine(gm.RespawnPlayer());
        }
    }

    public static void KillEnemy(Enemy enemy) {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy) {
        Destroy(_enemy.gameObject);
        Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity);
        cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);

        audioManager.PlaySound(_enemy.deathSoundName);

        Money += _enemy.moneyDrop;
        audioManager.PlaySound("Bonus");
    }

}
