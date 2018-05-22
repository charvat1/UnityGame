using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Animator animator;
    private SpriteRenderer r;
    private Rigidbody2D rb2d;
    private int maxSpeed = 5;
    public bool isJump = false;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        r = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    Vector2 pos;
   [SerializeField] Vector2 runSpeed = new Vector2(5,0);
    [SerializeField] Vector2 jumpSpeed = new Vector2(0, 5);
    // Update is called once per frame
    void Update()
    {
 //   autoMovement();
      //  jump();
        
    }
    private void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;
            animator.SetBool("isRunning", true);
            rb2d.AddForce(jumpSpeed);
            // rb2d.velocity = Vector3.ClampMagnitude(rb2d.velocity, maxSpeed);
          
        }
    }
    private void autoMovement()
    {
        animator.SetBool("isRunning", true);
        rb2d.AddForce(runSpeed);
        rb2d.velocity = Vector3.ClampMagnitude(rb2d.velocity, maxSpeed);
    }
    private void movement()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                r.flipX = false;
            }
            animator.SetBool("isRunning", true);
            rb2d.AddForce(-runSpeed);
            rb2d.velocity = Vector3.ClampMagnitude(rb2d.velocity, maxSpeed);

            Debug.Log("running left");
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                r.flipX = true;
            }
            animator.SetBool("isRunning", true);
            rb2d.AddForce(runSpeed);
            rb2d.velocity = Vector3.ClampMagnitude(rb2d.velocity, maxSpeed);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
}
