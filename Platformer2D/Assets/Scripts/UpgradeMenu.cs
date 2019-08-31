using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeMenu : MonoBehaviour {
    public GameObject player;

    [SerializeField] private Text healthText = null;

    [SerializeField] private Text damageText = null;

    [SerializeField] private Text healthCostText = null;

    [SerializeField] private Text damageCostText = null;

    [SerializeField] private Text dashCostText = null;

    [SerializeField] private float healthMultiplier = 1.2f;

    [SerializeField] private Button dashButton = null;

    [SerializeField]
    private int healthUpgradeCost;

    [SerializeField]
    private int damageUpgradeCost;

    [SerializeField]
    private int dashUpgradeCost = 0;

    private PlayerStats stats;
    private WeaponStats weapStats;

    private bool searchingForPlayer = false;

    private void OnEnable() {
        stats = PlayerStats.instance;
        UpdateValues();
    }

    private void Update() {
        if (player == null) {
            if (!searchingForPlayer) {
                searchingForPlayer = true;
                Debug.Log("LOOKING FOR PLAYER");
                StartCoroutine(SearchForPlayer());
            }
            return;
        }
    }

    void UpdateValues() {
        healthText.text = "health: " + stats.maxHealth.ToString();
        damageText.text = "damage multiplier: " + stats.damageMultiplier.ToString() + "x";
        healthCostText.text = "UPGRADE (" + healthUpgradeCost.ToString() + ")";
        damageCostText.text = "UPGRADE (" + damageUpgradeCost.ToString() + ")";
        dashCostText.text = "UPGRADE (" + dashUpgradeCost.ToString() + ")";
    }

    public void UpgradeHealth() {

        if (GameMaster.Money < healthUpgradeCost) {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }

        AudioManager.instance.PlaySound("Bonus");
        stats.maxHealth = (int)(stats.maxHealth * healthMultiplier);
        stats.curHealth = stats.maxHealth;
        GameMaster.Money -= healthUpgradeCost;
        healthUpgradeCost = (int)(healthUpgradeCost * 1.3f);
        UpdateValues();
    }

    public void UpgradeDamage() {
        if (GameMaster.Money < damageUpgradeCost) {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }
        stats.damageMultiplier += .1f;
        GameMaster.Money -= damageUpgradeCost;
        damageUpgradeCost = (int)(damageUpgradeCost * 1.2f);
        UpdateValues();
        AudioManager.instance.PlaySound("Bonus");
    }

    public void UpgradeDash() {
        if (GameMaster.Money < dashUpgradeCost) {
            AudioManager.instance.PlaySound("NoMoney");
            return;
        }
        GameMaster.Money -= dashUpgradeCost;
        UpdateValues();
        AudioManager.instance.PlaySound("Bonus");
        dashButton.interactable = false;

        player.gameObject.GetComponent<DashAbility>().enabled = true;

        if (player == null) {
            if (!searchingForPlayer) {
                searchingForPlayer = true;
                Debug.Log("LOOKING FOR PLAYER");
                StartCoroutine(SearchForPlayer());
            }
            return;
        } 
    }

    IEnumerator SearchForPlayer() {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null) {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else {
            player = sResult;
            Debug.Log("PLAYER FOUND");
            searchingForPlayer = false;
            player.gameObject.GetComponent<DashAbility>().enabled = true;
            yield return false;
        }
    }
}
