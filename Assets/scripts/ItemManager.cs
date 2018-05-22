using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    // Use this for initialization
    public static int coins;
	void Start () {
         GameFileManager.LoadGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static void SetCoins(int coins)
    {
        ItemManager.coins += coins;
    }
    public static int GetCoins()
    {
        
        return coins;
    }
}
