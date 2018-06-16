using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_SmallHealth : MonoBehaviour {

    PlayerController pc;

	void Start () {
        //Get reference to player script
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
    void OnTriggerEnter2D (Collider2D c)
    {
        //If player enters trigger, add health to player and remove object, assuming they are not at full health.
        if (c.gameObject.CompareTag("Player") && pc.health < 6)
        {
            pc.health += 2;
            Destroy(gameObject);
        }
    }
}
