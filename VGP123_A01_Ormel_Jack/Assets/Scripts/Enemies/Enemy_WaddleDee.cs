using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Enemy_WaddleDee : MonoBehaviour {

    Rigidbody2D rb;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] Transform jumpCheck;
    Transform player;

    float speed = 50f;

    bool isFacingLeft = true;
    bool canJump = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
            Debug.LogError("Rigidbody2D missing on " + name + "; This should never print.");

        if (!jumpCheck)
            Debug.LogError("JumpCheck GameObject missing on " + name);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (!player)
            Debug.LogError("player GameObject missing on " + name);
        else if (player.position.x > transform.position.x)
            Flip();
    }
	
	void FixedUpdate () {
        Move();
    }

    void Flip()
    {
        //Changes facing direction via x-scale and boolean.
        isFacingLeft = !isFacingLeft;
        Vector3 scaleFactor = transform.localScale;
        scaleFactor.x = -scaleFactor.x;
        transform.localScale = scaleFactor;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        if (c.gameObject.tag == "PlayerProjectile")
        {
            //Increment score here.
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        
    }

    void Move()
    {
        //Move enemy depending on current facing direction
        if (isFacingLeft)
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(speed, rb.velocity.y);

        //Make enemy jump if they bump into a platform.
        canJump = Physics2D.IsTouchingLayers(jumpCheck.GetComponent<Collider2D>(), GroundLayer);
        if (canJump && rb.velocity.y == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * 90f, ForceMode2D.Impulse);
        }
    }
}
