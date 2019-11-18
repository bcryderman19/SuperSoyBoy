using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))] //validates that any game object attached to this script has the min necessary components
public class SoyBoyController : MonoBehaviour
{
    public float speed = 14.0f;
    public float accel = 6.0f;
    public float jumpSpeed = 8f;
    public float jumpDurationThreshold = 0.25f;
    public float airAccel = 3f;
    public float jump = 14f;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip slideClip;
    public bool isJumping;

    private Vector2 input; //stores the controllers current input values for x and y at any point in time. negatives are left and down. positives right or up
    private SpriteRenderer sr; 
    private Rigidbody2D rb;
    private Animator animator;
    private float rayCastLengthCheck = 0.005f;
    private float width;
    private float height;
    private float jumpDuration;
    private AudioSource audioSource;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool PlayerIsOnGround()
    {
        //1 performs raycast directly below center of character
        bool groundCheck1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - height),
                                -Vector2.up, rayCastLengthCheck);
        bool groundCheck2 = Physics2D.Raycast(new Vector2(transform.position.x + (width - 0.2f), transform.position.y - height),
                                -Vector2.up, rayCastLengthCheck);
        bool groundCheck3 = Physics2D.Raycast(new Vector2(transform.position.x - (width - 0.2f), transform.position.y - height),
                                -Vector2.up, rayCastLengthCheck);

        //2
        if (groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWallToLeftOrRight()
    {
        //1 raycasts to the left or to the right
        bool wallOnLeft = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y),
                                -Vector2.right, rayCastLengthCheck);
        bool wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y),
                                Vector2.right, rayCastLengthCheck);

        //2
        if (wallOnLeft || wallOnRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerIsTouchingGroundOrWall()
    {
        if (PlayerIsOnGround() || IsWallToLeftOrRight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWallDirection()
    {
        bool isWallLeft = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y),
                                -Vector2.right, rayCastLengthCheck);

        bool isWallRight = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y),
                                Vector2.right, rayCastLengthCheck);

        if (isWallLeft)
        {
            return -1;
        }
        else if (isWallRight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //1 gets x and y position from built in unity control axis horizontal and jump
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");

        animator.SetFloat("Speed", Mathf.Abs(input.x));

        //2
        if (input.x > 0f)
        {
            sr.flipX = false; //player facing right sprite gets flipped on the x axis
        }
        else if (input.x < 0f)
        {
            sr.flipX = true; //player facing left so sprite not flipped
        }

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;
            animator.SetBool("IsJumping", true);
        }
        else
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
            jumpDuration = 0f;
        }
        if (PlayerIsOnGround() && !isJumping)
        {
            if (input.y > 0f)
            {
                isJumping = true;
                PlayAudioClip(jumpClip);
            }
            animator.SetBool("IsOnWall", false);
            if (input.x < 0f || input.x > 0f)
            {
                PlayAudioClip(runClip);
            }
        }

        if (jumpDuration > jumpDurationThreshold)
        {
            input.y = 0f;
            animator.SetBool("IsOnWall", false);
        }
    }

    void FixedUpdate()
    {
        //1
        var acceleration = 0f;
        if (PlayerIsOnGround())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }

        var xVelocity = 0f;

        //2
        if (PlayerIsOnGround() && input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }

        var yVelocity = 0f;
        if (PlayerIsTouchingGroundOrWall() && input.y == 1)
        {
            yVelocity = jump;
        }
        else
        {
            yVelocity = rb.velocity.y;
        }

        //3
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));

        //4
        rb.velocity = new Vector2(xVelocity, yVelocity);

        if (IsWallToLeftOrRight() && !PlayerIsOnGround() && input.y == 1)
        {
            rb.velocity = new Vector2(-GetWallDirection() * speed * 0.75f, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        else if (!IsWallToLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        if (IsWallToLeftOrRight() && !PlayerIsOnGround() && input.y == 1)
        {
            rb.velocity = new Vector2(-GetWallDirection() * speed * 0.75f, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
            PlayAudioClip(jumpClip);
        }
        else if (!IsWallToLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        if (IsWallToLeftOrRight() && !PlayerIsOnGround())
        {
            animator.SetBool("IsOnWall", true);
            PlayAudioClip(slideClip);
        }

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(clip);
        }
    }
}
