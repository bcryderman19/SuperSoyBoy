using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))] //validates that any game object attached to this script has the min necessary components
public class SoyBoyController : MonoBehaviour
{
    public float speed = 14.0f;
    public float accel = 6.0f;

    private Vector2 input; //stores the controllers current input values for x and y at any point in time. negatives are left and down. positives right or up
    private SpriteRenderer sr; 
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1 gets x and y position from built in unity control axis horizontal and jump
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");

        //2
        if (input.x > 0f) 
        {
            sr.flipX = false; //player facing right sprite gets flipped on the x axis
        }
        else if (input.x < 0f)
        {
            sr.flipX = true; //player facing left so sprite not flipped
        }
    }

    void FixedUpdate()
    {
        //1
        var acceleration = accel;
        var xVelocity = 0f;

        //2
        if (input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }

        //3
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));

        //4
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }
}
