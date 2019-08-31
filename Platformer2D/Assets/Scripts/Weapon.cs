using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public float effectSpawnRate = 10;

    public float fireRate = 0;
    public int damage;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitPrefab;

    public string weaponShootSound = "DefaultShot";

    // handle camera shaking
    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.2f;
    CameraShake camShake;

    float timeToSpawnEffect = 0;
    float timeToFire = 0;
    Transform firePoint;

    //Caching
    AudioManager audioManager;

    private WeaponStats stats;

    private void Awake() {
        firePoint = transform.Find("FirePoint");
        if(firePoint == null) {
            Debug.LogError("Didn't find firepoint");
        }
    }

    private void Start() {
        stats = WeaponStats.instance;
        print(damage);
        damage = stats.curDamage;
        print(damage);

        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null) {
            Debug.LogError("No camera shake found on GM object");
        }
        audioManager = AudioManager.instance;

        if(audioManager == null) {
            Debug.LogError("No audiomanager found in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fireRate == 0) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        }
        else {
            if (Input.GetButton("Fire1") && Time.time > timeToFire) {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }   
        }
    }

    void Shoot() {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition-firePointPosition, 100, whatToHit);
        
        if(hit.collider != null) {           
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) {
                
                enemy.DamageEnemy((int)(damage*PlayerStats.instance.damageMultiplier));
                Debug.Log("We hit " + hit.collider.name + " and did " + damage * PlayerStats.instance.damageMultiplier + " damage.");
            }
        }

        if (Time.time >= timeToSpawnEffect) {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null) {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else {
                hitNormal = hit.normal;
                hitPos = hit.point;
            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }

    }

    void Effect(Vector3 hitPos, Vector3 hitNormal) {
        Transform trail = Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>(); 

        if(lr != null) {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }
        Destroy(trail.gameObject, 0.04f);

        if(hitNormal != new Vector3(9999, 9999, 9999)) {
            Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.forward, hitNormal));
        }

        Transform clone = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);  

        camShake.Shake(camShakeAmt, camShakeLength);

        //play shoot sound
        audioManager.PlaySound(weaponShootSound);
    }

}
