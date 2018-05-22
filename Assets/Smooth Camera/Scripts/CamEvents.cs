using UnityEngine;

public class CamEvents : MonoBehaviour
{
    [Header("Entering")]
    public bool die = false;
    public bool lockXAxis = false;
    public bool lockYAxis = false;
    [Range(1.0f, 10.0f)]
    public float newZoom = 0.0f;
    [Range(1.0f, 10.0f)]
    public float zoomSpeed = 0.0f;
    [Range(0.0f, 10.0f)]
    public float newSpeed = 0.0f;
    
    [Header("Exiting")]
    public bool exitLockXAxis = false;
    public bool exitLockYAxis = false;
    [Range(1.0f, 10.0f)]
    public float exitZoom = 0.0f;
    [Range(0.0f, 10.0f)]
    public float exitSpeed = 0.0f;
        
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")                                        //If object with tag "Player" stays.
        {
            SmoothPlayerCam.speedOfZoom = zoomSpeed;                    //Change cam zoomspeed.
            SmoothPlayerCam.changeZoomTo = newZoom;                     //Change cam zoom.
            SmoothPlayerCam.changeSpeedTo = newSpeed;                   //Change cam speed.
            SmoothPlayerCam.lockXAxis = lockXAxis;                      //Lock/Unlock cam axis.
            SmoothPlayerCam.lockYAxis = lockYAxis;                      //Lock/Unlock cam axis.

            if (die)
            {
                PlayerMovement.dead = true;                             //Player controls disabled.
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")                                        //If object with tag "Player" leaves.
        {
            SmoothPlayerCam.speedOfZoom = zoomSpeed;                    //Change cam zoomspeed.
            SmoothPlayerCam.changeZoomTo = exitZoom;                    //Change cam zoom.
            SmoothPlayerCam.changeSpeedTo = exitSpeed;                  //Change cam speed.
            SmoothPlayerCam.lockXAxis = exitLockXAxis;                  //Lock/Unlock cam axis.
            SmoothPlayerCam.lockYAxis = exitLockYAxis;                  //Lock/Unlock cam axis.
        }
    }
}



/*
_________________________________________________________________________
#################################
######### By SchrippleA #########
#################################
*/
