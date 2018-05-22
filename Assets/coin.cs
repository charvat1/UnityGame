using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour {

    // Use this for initialization
    public AudioClip coinCollect;
    void Start () {
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = coinCollect;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        Debug.Log("coin collected");
        // this.gameObject.SetActive(false);
        GetComponent<AudioSource>().PlayOneShot(coinCollect);
        //audio.PlayOneShot(aClip);
        GetComponent<SpriteRenderer>().enabled = false;
        ItemManager.SetCoins(1);
        Destroy(gameObject, coinCollect.length);
        // Destroy(this.gameObject,3f);
    }
}
