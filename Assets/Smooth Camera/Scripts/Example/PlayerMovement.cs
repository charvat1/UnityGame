using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    static public bool dead = false;

	public float speed = 6.0f;

	private Rigidbody2D rb2d;

	void Start ()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	void Update () 
	{
        if (!dead)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
                transform.localScale = new Vector2(-1.0f, 1.0f);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
                transform.localScale = new Vector2(1.0f, 1.0f);
            }
        }
	}
}



/*
_________________________________________________________________________
#################################
######### By SchrippleA #########
#################################
*/