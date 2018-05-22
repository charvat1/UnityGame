using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    [SerializeField] GameObject player;
    Rigidbody2D rb;
    Vector3 offset,jumpoffset;
	// Use this for initialization
	void Start () {
        rb = player.GetComponent<Rigidbody2D>();
        x = player.transform.position.x;
        y = player.transform.position.y;
        playerY = player.transform.position.y;
        offset = transform.position - player.transform.position;
        jumpoffset = new Vector3(transform.position.x - player.transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    float x, y, playerY = 0,curPY;
	void Update () {
       
        
	}

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
   
            transform.position = new Vector3((player.transform.position.x + offset.x), transform.position.y, transform.position.z);

    }
}
