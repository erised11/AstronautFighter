using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    
    // TODO: Implement cooldown

    public float maxDashTime = 1.0f;
    public float dashSpeed = 50.0f;
    public float upwardForce = 0f;
    public float dashCooldown = 2f;

    // handle camera shaking
    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.2f;
    CameraShake camShake;

    public Transform dashParticles;

    private Vector2 moveDirection;
    private float currentDashTime;
    private float currentCooldown;
    private bool canDash;
    private Rigidbody2D rb;

    [SerializeField] private string dashSoundName = "DashSound";

    AudioManager audioManager;
    void Start() {

        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null) {
            Debug.LogError("No camera shake found on GM object");
        }

        currentDashTime = maxDashTime;
        rb = GetComponent<Rigidbody2D>();

        audioManager = AudioManager.instance;
        if (audioManager == null) {
            Debug.LogError("No audiomanager found");
        
    }
}

    void Update() {
        doDash();
    }

    void DashEffect(float direction) {
        audioManager.PlaySound(dashSoundName);
        camShake.Shake(camShakeAmt, camShakeLength);
        Instantiate(dashParticles, rb.position, Quaternion.Euler(0, direction*90, 0));
    }

    void doDash() {
        float direction = Input.GetAxisRaw("Horizontal");

        if (Input.GetMouseButtonDown(1) && direction != 0 && canDash) {
            DashEffect(direction);
            currentDashTime = maxDashTime;
            currentCooldown = dashCooldown;
            canDash = false;
        }

        if(currentCooldown > 0) {
            currentCooldown -= Time.deltaTime;
        }
        else {
            canDash = true;
        }

        if (currentDashTime > 0) {
            moveDirection = new Vector2(direction * dashSpeed, upwardForce);
            currentDashTime -= Time.deltaTime;
        }
        else {
            moveDirection = Vector2.zero;
        }

        rb.AddForce(moveDirection);
    }

}
