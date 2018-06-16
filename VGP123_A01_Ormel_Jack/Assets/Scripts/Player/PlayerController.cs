using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Object References
    Rigidbody2D rb;
    Animator anim;
    HUDManager HUD;

    [SerializeField] LayerMask GroundLayer;
    [SerializeField] Transform GroundCheck;
    [SerializeField] Transform projectileSpawnPos;
    
    [SerializeField] Projectile spitProjectile;
    [SerializeField] Projectile exhaleProjectile;

    //Fields (PRIVATIZE THESE UPON FINALIZING PLAYER SCRIPT)
    enum Projectiles {EXHALE, SPIT};

    public float speed = 75f;
    public float jumpForce = 100f;

    public float moveValue;

    public bool isFloating = false;
    public bool canFloatShoot = false;
    public bool isShooting = false;
    public bool isGrounded = true;  
    public bool isFacingRight = true;
    public bool isHurting = false;

    public float hurtingTimer = 0.0f;
    public float floatingTimer = 0.0f;

    static int _health = 6;

    //TODO:
    //Set up animation for shooting projectile :: DONE
    //Set delay for shooting projectile :: DONE, with the animation itself.
    //Set delay for being able to float again :: DONE

    //Get enemies set up
    //Get damage set up properly, including healing from items and invulnerability after getting hurt
    //UI
    //Level Transitions
    //Menu
    //GameOver

    private void Awake() {
        isGrounded = true;
        isFacingRight = true;
    }

    void Start() {

        //Get compononets
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        HUD = GameObject.Find("HUD").GetComponent<HUDManager>();

        //Validate components have been gotten
        if (!rb)
            Debug.LogError("Rigidbody2D component not found on " + name);
        if (!rb.sharedMaterial)
            Debug.LogError("PhysicsMaterial component not added to " + name);
        if (!anim) 
            Debug.LogError("Animator component not found on " + name);
        

        if (!GroundCheck)
            Debug.LogError("GroundCheck object not found on " + name);
        if (!projectileSpawnPos)
             Debug.LogError("projectileSpawnPos object not found on " + name);

        if (!spitProjectile)
            Debug.LogError("spitProjectile object not found on " + name);
        if (!exhaleProjectile)
            Debug.LogError("exhaleProjectile object not found on " + name);

        if (!HUD)
            Debug.LogError("HUD object not found on " + name);

        //Set fields to default if not properly set

        if (speed <= 0) {
            speed = 75f;
            Debug.LogWarning("Speed not set on " + name + "; Defaulting to " + speed);
        }

        if (jumpForce <= 0) {
            jumpForce = 100.0f;
            Debug.LogWarning("Jump force not set on " + name + "; Defaulting to " + jumpForce);
        }

        if (floatingTimer != 0.0f)
        {
            floatingTimer = 0.0f;
            Debug.LogWarning("floatingTimer not set properly on " + name + "; Resetting to " + floatingTimer);
        }

        if (!isGrounded) {
            isGrounded = true;//set default groundCheckRadius if value not set properly.
            Debug.LogWarning("isGrounded not set on " + name + "; Defaulting to " + isGrounded);
        }                

    }

    // Update is called once per frame
    void FixedUpdate() {
        
        //If Z is pressed, call current ability.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Ability();
        }

        //Move player.
        if (!isHurting)
            Move();

        //Set animation variables
        SetAnimate();
       
    }

    void OnTriggerEnter2D(Collider2D c) {
        if (c.gameObject.tag == "Killzone") {
            Debug.Log("You dead");
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Enemy")
        {
            if (c.transform.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(-50, 200);
            }
            else
            {
                rb.velocity = new Vector2(50, 200);
            }
            Hurt();
        }
    }

    void Hurt()
    {
        //Damage player, make sure to interuprt certain variables.
        health--;
        isHurting = true;
        canFloatShoot = false;
        isFloating = false;
        EndShoot();
    }

    //=== Move Kirby. ===//
    void Move()
    {
        //Get horizontal input
        moveValue = Input.GetAxis("Horizontal");
        //Get Vertical input for floating around
        float floatValue = Input.GetAxis("Vertical") * 0.75f;

        //When not floating, increment cooldown towards threshold
        if (!isFloating && floatingTimer < 5.0f)
            floatingTimer += 0.1f;

        //If kirby isn't already floating and threshold for timer is met, start floating and reset timer.
        if (Input.GetKey(KeyCode.UpArrow) && !isFloating && floatingTimer >= 5.0f)
        {
            isFloating = true;
            floatingTimer = 0.0f;
        }

        //Stop kirby from moving when he is shooting
        if (isShooting)
        {
            if (rb.velocity.x != 0)
            {
                rb.AddForce(new Vector2(-rb.velocity.x, 0), ForceMode2D.Force);
            }        
        }
        //Otherwise move kirby based on if he is floating.
        else if (isFloating)
        {
            rb.velocity = new Vector2(moveValue * speed * 0.75f, floatValue * speed);
        }
        //Otherwise let kirby move normally and jump if on the ground.
        else
        {
            //Jump if Kirby is on the ground
            if (Input.GetKeyDown(KeyCode.X) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            //Walk with horizontal input.
            rb.velocity = new Vector2(moveValue * speed, rb.velocity.y);
        }
        
    }

    //=== What Ability Kirby should be using when Z is pressed. ===//
    void Ability()
    {
        //If fully floating, shoot a thing.
        if (isFloating && canFloatShoot)
        {
            ShootProjectile(Projectiles.EXHALE);
            canFloatShoot = false;
            isFloating = false;
        }
        //If not floating and on the ground, begin shooting a different thing.
        else if (isGrounded && !isFloating)
        {
            //Begins shooting animation
            isShooting = true;
        }
    }

    //Used by animator to end shooting animation
    void EndShoot()
    {
        isShooting = false;
    }
    //Also used by the animator.
    void CanShoot()
    {
        canFloatShoot = true;
    }

    //=== Set variables relevent to animation. ===//
    void SetAnimate()
    {
        anim.SetFloat("speed", Mathf.Abs(moveValue));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isFloating", isFloating);
        anim.SetBool("isShooting", isShooting);
        anim.SetBool("isFalling", (rb.velocity.y < 0 && !isGrounded));
        anim.SetBool("isHurting", isHurting);

        if (isHurting)
            hurtingTimer += 0.1f;
        if (hurtingTimer >= 7.5f && isGrounded)
        {
            isHurting = false;
            hurtingTimer = 0.0f;
        }        

        //Flip sprite depending on direction facing
        if ((moveValue > 0 && !isFacingRight) || (moveValue < 0 && isFacingRight))
        {
            Flip();
        }
    }
 
    //Turn Kirby around.
    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scaleFactor = transform.localScale;
        scaleFactor.x = -scaleFactor.x;
        transform.localScale = scaleFactor;
    }

    //Choose which projectile to spawn.
    void ShootProjectile(Projectiles p)
    {
        Projectile temp;
        switch(p)
        {
            //Shoot when grounded
            case Projectiles.SPIT:
                temp = Instantiate(spitProjectile, projectileSpawnPos.position, projectileSpawnPos.rotation);
                break;
            //Shoot when floating
            case Projectiles.EXHALE:
                temp = Instantiate(exhaleProjectile, projectileSpawnPos.position, projectileSpawnPos.rotation);
                break;
            //Shoot when shit goes broke
            default:
                temp = Instantiate(spitProjectile, projectileSpawnPos.position, projectileSpawnPos.rotation);
                Debug.LogError("Warning: Projectile error in ShootProjectile on " + name);
                break;
        }

        //.Vaildate projectile has a speed and lifetime set properly.
        if (temp.speed <= 0)
            Debug.LogError("Projectile " + temp.name + "does not have a speed set properly.");
        if (temp.lifeTime <= 0)
            Debug.LogError("Projectile " + temp.name + "does not have a lifetime set properly.");

        //set speed; if facing the other direction, flip speed and sprite's facing direction.
        if (!isFacingRight)
        {
            temp.speed *= -1;
            Vector3 scaleFactor = temp.transform.localScale;
            scaleFactor.x = -scaleFactor.x;
            temp.transform.localScale = scaleFactor;
        }

    }



    //After kirby moves, check to see if he is grounded.
    private void LateUpdate()
    {
        isGrounded = Physics2D.IsTouchingLayers(GroundCheck.GetComponent<Collider2D>(), GroundLayer);
    }

    //Setter and getter for Kirby's HP.
    public int health
    {
        set
        {
            if (!isHurting && value < _health)
            {
                _health = value;
                if (_health < 0)
                    _health = 0;
            }
            else if (value >= _health)
            {
                _health = value;
                if (_health > 6)
                    _health = 6;
            }
            HUD.SetShowingHP(_health);
        }
        get { return _health; }
    }

}