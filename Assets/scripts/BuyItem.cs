using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BuyItem : MonoBehaviour {

    public ItemManager itemManager;
	public void Buy100Coins()
    {
        ItemManager.SetCoins(100);
        // 1
        Save save = GameFileManager.SaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        // 3
      //  coins = 0;
        
        Debug.Log("Game Saved");
    }
  
}
