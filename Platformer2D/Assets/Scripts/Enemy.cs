using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats {
        
        public int damage = 40;
        public int maxHealth = 100;
        private int _curHealth;
        public int curHealth {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init() {
            curHealth = maxHealth;
        }
           
    }

    public EnemyStats stats = new EnemyStats();

    public float shakeAmt = 0.2f;
    public float shakeLength = 0.1f;
    public Transform deathParticles;
    public string deathSoundName = "Explosion";

    public int moneyDrop = 10;

    [Header("Optional:")]
    [SerializeField]
    private StatusIndicator statusIndicator = null;

    private void Start() {
        stats.Init();
        
        if(statusIndicator != null) {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        if(deathParticles == null) {
            Debug.LogError("No death particles referenced on enemy");
        }

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;
    }

    void OnUpgradeMenuToggle(bool activeState) {
        // handle what happens when upgrade menu is toggled
        GetComponent<EnemyAI>().enabled = !activeState;
    }

    private void OnDestroy() {
        GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
    }


    public void DamageEnemy(int damage) {
        stats.curHealth -= damage;
        if (stats.curHealth <= 0) {
            GameMaster.KillEnemy(this);
        }

        if (statusIndicator != null) {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _collision) {
        Player _player = _collision.collider.GetComponent<Player>();
        if(_player != null) {
            _player.DamagePlayer(stats.damage);
            DamageEnemy(9999);
        }
    }

}
