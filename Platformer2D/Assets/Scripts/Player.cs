using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

[RequireComponent(typeof(Platformer2DUserControl))]
public class Player : MonoBehaviour {



    public int fallBoundary = -20;
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "Grunt";

    private AudioManager audioManager; 

    private PlayerStats stats;

    [SerializeField]
    private StatusIndicator statusIndicator = null;

    private void Start() {

        stats = PlayerStats.instance;
        stats.curHealth = stats.maxHealth;

        if(statusIndicator == null) {
            Debug.LogError("STATUS INDICATOR: No status indicator referenced on player");
        }
        else {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

        audioManager = AudioManager.instance;
        if (audioManager == null) {
            Debug.LogError("No audiomanager found");
        }

        InvokeRepeating("RegenHealth", 1f/stats.healthRegenRate, 1f/stats.healthRegenRate) ;

    }

    void Update() {
        if (transform.position.y <= fallBoundary)
            DamagePlayer(99999);
    }

    public void DamagePlayer(int damage) {
        stats.curHealth -= damage;
        if(stats.curHealth <= 0) {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else {
            audioManager.PlaySound(damageSoundName  );
        }

        if (statusIndicator != null) {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    void OnUpgradeMenuToggle(bool activeState) {
        // handle what happens when upgrade menu is toggled
        GetComponent<Platformer2DUserControl>().enabled = !activeState;
        Weapon _weapon = GetComponentInChildren<Weapon>();
        if (_weapon != null)
            _weapon.enabled = !activeState;
    }

    void RegenHealth() {
        stats.curHealth += 1;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
    }

    private void OnDestroy() {
        GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
    }

}
