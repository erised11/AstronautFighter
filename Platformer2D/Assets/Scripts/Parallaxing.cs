using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{

    public Transform[] backgrounds; //Array of all back and foregrounds to be parallaxed
    private float[] parallaxScales; //The proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f;    //How smooth the parallax is going to be (set above 0)

    private Transform cam;       // reference to main camera's transform
    private Vector3 previousCameraPos;  //position of the camera in the previous frame

    // called before start
    void Awake() {
        cam = Camera.main.transform;
    }


    // Start is called before the first frame update
    void Start()
    {
        // previous frame had current frame's camera position
        previousCameraPos = cam.position;

        // assigning parallax scales
        parallaxScales = new float[backgrounds.Length];
        for(int i=0; i<backgrounds.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i< backgrounds.Length; i++) {
            // the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            float parallax = (previousCameraPos.x - cam.position.x) * parallaxScales[i];

            //set a target x position which is the current position + the parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            //create target position which is the background's current position with it's target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            //fade between current position and target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //set previous cam pos to cameras pos
        previousCameraPos = cam.position;
    }
}
