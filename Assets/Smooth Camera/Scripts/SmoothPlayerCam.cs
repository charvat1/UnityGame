using UnityEngine;

public class SmoothPlayerCam : MonoBehaviour
{
    static public float changeZoomTo;
    static public float changeSpeedTo;
    static public bool lockXAxis;
    static public bool lockYAxis;
    static public float speedOfZoom;
        
    [Header("Main Settings")]
    public Transform player;
    public bool smoothCam = true;
    public bool blockX = false;
    public bool blockY = false;
    [Range(1.0f, 10.0f)]
    public float speed = 5.0f;
    [Range(1.0f, 10.0f)]
    public float distance = 5.0f;

    [Header("Shake Settings")]
    public bool shake = false;
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 1.0f;

    [Header("Knockback Settings")]
    public bool leftKnockback = false;
    public bool rightKnockback = false;
    public float knockbackTime = 0.1f;
    public float knockbackAmount = 1.2f;
    
    private float timer;
    private float shakeTimer;
    private Vector3 goHere;
    private Camera cam;

    void Start()
    {                                               //Set the default values of this variables.
        cam = GetComponent<Camera>();
        changeZoomTo = distance;
        changeSpeedTo = speed;
        lockXAxis = blockX;
        lockYAxis = blockY;
    }

    void FixedUpdate()
    {                                               //Update the new values of this variables.
        CameraShake();                                                            
        cam.orthographicSize = distance;
        speed = changeSpeedTo;
        blockX = lockXAxis;
        blockY = lockYAxis;

        if (distance > changeZoomTo)
        {                                           //Decrease zoom till "distance" is set.
            distance -= (speedOfZoom * Time.deltaTime);

            if (distance <= changeZoomTo)
            {
                distance = changeZoomTo;
            }
        }

        if (distance < changeZoomTo)
        {                                           //Increase zoom till "distance" is set.
            distance += (speedOfZoom * Time.deltaTime);

            if (distance >= changeZoomTo)
            {
                distance = changeZoomTo;
            }
        }

        if (smoothCam)                              //Start smoothCamera.
        {
            if (blockY)                             //Save player Xpos, use own Ypos and add the distance of the camera.
            {                                       //Move the camera to the goHere position.
                goHere = new Vector3(player.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }

            if (blockX)
            {
                goHere = new Vector3(transform.position.x, player.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }

            if (!blockX && !blockY)
            {                                       //Don't block the X or Y axis.
                goHere = new Vector3(player.position.x, player.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }
            else if (blockX && blockY)
            {                                       //Block the X or Y axis.
                goHere = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }
        }
        else
        {                                           //No smooth movement.

            goHere = edgeCheck();

            transform.position = new Vector3(goHere.x, goHere.y, goHere.z);
        }
    }

    void CameraShake()
    {

        if (leftKnockback)
        {                                           //Knock cam left.
            timer += Time.deltaTime;
            goHere = new Vector3((player.position.x) - knockbackAmount, player.position.y, transform.position.z);

            if (timer > knockbackTime)
            {
                goHere = new Vector3(player.position.x, player.position.y, transform.position.z);
                leftKnockback = false;
                timer = 0.0f;                
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }
        }

        if (rightKnockback)
        {                                           //Knock cam right.
            timer += Time.deltaTime;
            goHere = new Vector3((player.position.x) + knockbackAmount, player.position.y, transform.position.z);

            if (timer > knockbackTime)
            {
                goHere = new Vector3(player.position.x, player.position.y, transform.position.z);
                rightKnockback = false;
                timer = 0.0f;                
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
            }
        }
        
        if (shake)
        {                                           //Start shaking cam.
            shakeTimer += Time.deltaTime;

            if (shakeTimer < shakeDuration)         //Shakes the cam for a set time.
            {
                timer += Time.deltaTime;

                if (timer < shakeIntensity * knockbackTime)
                {                                   //Shake right.
                    goHere = new Vector3((transform.position.x) + knockbackAmount, transform.position.y, transform.position.z);
                }
                else if (timer < shakeIntensity * knockbackTime)
                {                                   //Shake left.
                    goHere = new Vector3((transform.position.x) - knockbackAmount, transform.position.y, transform.position.z);
                }

                if (timer > knockbackTime)
                {                                   //Stop shaking cam.
                    goHere = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    timer = 0.0f;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, goHere, Time.deltaTime * speed);
                }
            }
            else
            {                                       //Stop shaking cam.
                shake = false;
                shakeTimer = 0.0f;
                timer = 0.0f;
            }
        }
    }
    public Transform farLeft;  // End of screen Left
    public Transform farRight;  //End of Screen Right
    private Vector3 edgeCheck()
    {
       /* if (transform.position.x > farRight.position.x)
        {
            GloabalVars.canMove = false;
            return new Vector3(farRight.position.x, transform.position.y, transform.position.z);
        }

        else if (transform.position.x < farLeft.position.x)
        {
            GloabalVars.canMove = false;
            return new Vector3(farLeft.position.x, transform.position.y, transform.position.z);
        }

        else
        {
            GloabalVars.canMove = true;*/
            return new Vector3(player.position.x, player.position.y, transform.position.z);
       // }
       // return new Vector3();

      //  Debug.Log("Screen edge");
    
    }
}



/*
_________________________________________________________________________
#################################
######### By SchrippleA #########
#################################
*/