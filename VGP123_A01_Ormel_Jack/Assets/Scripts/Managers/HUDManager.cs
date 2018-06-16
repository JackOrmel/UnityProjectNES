using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    PlayerController player;

    [SerializeField] Image[] healthBars;
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("PlayerCharacter").GetComponent<PlayerController>();
        if (!player)
            Debug.LogError("Could not find PlayerController script for " + name);

        if (healthBars == null)
            Debug.LogError("GameObjects not properly added to " + name);
    }
	
	public void SetShowingHP(int hp)
    {
        for (int i = 0; i < 6; i ++)
        {
            if (i < hp)
                healthBars[i].sprite = full;
            else           
                healthBars[i].sprite = empty;            
        }
    }
}
