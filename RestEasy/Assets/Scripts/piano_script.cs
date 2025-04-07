using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piano_script : MonoBehaviour
{
    [SerializeField]
    private GameObject key1;

    [SerializeField]
    private GameObject key2;

    [SerializeField]
    private GameObject key3;

    [SerializeField]
    private GameObject key4;

    [SerializeField]
    private AudioClip[] keyPressedSFX; // NOTE: Unused now that there's AudioManager but will keep for now

    bool canKeyBePressed = true;
    private int currentKeyIndex = 0;  

    // Define the correct sequence of keys
    private GameObject[] keySequence;

    void Start()
    {
        keySequence = new GameObject[] { key2, key3, key4, key1 };

        // Color the keys for testing purposes (you can remove this later)
        key1.GetComponent<Renderer>().material.color = Color.yellow;
        key2.GetComponent<Renderer>().material.color = Color.yellow;
        key3.GetComponent<Renderer>().material.color = Color.yellow;
        key4.GetComponent<Renderer>().material.color = Color.yellow;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("pianokey"))
                {
                    piano_key_pressed(hit.collider.gameObject);
                }
            }
        }
    }

    void piano_key_pressed(GameObject key)
    {
        Debug.Log(key.name);
        if (key.name == "Cube.021") {
            AudioManager.instance.PlaySFX("c6");
            //AudioSource.PlayClipAtPoint(keyPressedSFX[0], Camera.main.transform.position);
        }
        else if (key.name == "Cube.026") {
            AudioManager.instance.PlaySFX("e6");
            //AudioSource.PlayClipAtPoint(keyPressedSFX[1], Camera.main.transform.position);
        }
        else if (key.name == "Cube.027") {
            AudioManager.instance.PlaySFX("f6");
            //AudioSource.PlayClipAtPoint(keyPressedSFX[2], Camera.main.transform.position);
        }
        else if (key.name == "Cube.028") {
            AudioManager.instance.PlaySFX("g6");
            //AudioSource.PlayClipAtPoint(keyPressedSFX[3], Camera.main.transform.position);
        }

        if (canKeyBePressed && key == keySequence[currentKeyIndex])
        {
            canKeyBePressed = false;
            key.transform.localPosition = new Vector3(key.transform.localPosition.x, -0.17f, key.transform.localPosition.z);

            if (currentKeyIndex == keySequence.Length - 1)
            {
                Debug.Log("Puzzle completed!");
                
                // Play success sound
                AudioManager.instance.PlaySFX("puzzle-success");

                key1.GetComponent<Renderer>().material.color = Color.green;
                key2.GetComponent<Renderer>().material.color = Color.green;
                key3.GetComponent<Renderer>().material.color = Color.green;
                key4.GetComponent<Renderer>().material.color = Color.green;
                
            }
            else
            {
                currentKeyIndex++;
            }

            StartCoroutine(WaitForOneSecond(key));
        }
        else if (canKeyBePressed)
        {
            Debug.Log("Wrong key! Try again.");
        }
    }

    IEnumerator WaitForOneSecond(GameObject key)
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Raise the key back up after the press
        key.transform.localPosition = new Vector3(key.transform.localPosition.x, -0.155f, key.transform.localPosition.z);

        // Reset the ability to press keys
        canKeyBePressed = true;
    }
}
