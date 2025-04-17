using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyScript : MonoBehaviour
{

    /*
    README
    you can look at piano_script to see the exact implementation

    in the main puzzle object script (or at least the one that has the win state)
    in the references add the key script like this:

    private keyScript ks;

    then in the start() add this:

    ks = FindObjectOfType<keyScript>();

    this will allow the other script to communicate with the key script.
    now find wherever the win state is determined and add this code:

    ks.summonKey(new Vector3(5.5f, -1f, 7f));

    these coordinates are examples, it is where the key is instatiated in the pianos case
    I suggest grabbing the prefab and playing with the position to see where best it would be for each puzzle
    */

    public GameObject key;

    public void summonKey(Vector3 pos) {
        // instantiate prefab bookshelf key at that position 
        // Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        Instantiate(key, pos, Quaternion.identity);
    }

}
