using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed;

    public float lifeTime;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);

        if (lifeTime <= 0) {
            lifeTime = 2.0f;//set default lifeTime value if value not set properly.
            Debug.LogWarning("lifeTime not set on " + name + "; Defaulting to " + lifeTime);
        }

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D c) {
        if (!(c.gameObject.CompareTag("Player"))) 
            Destroy(gameObject);
    }
}
