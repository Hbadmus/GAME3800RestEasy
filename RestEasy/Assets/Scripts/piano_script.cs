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
        if (canKeyBePressed && key == keySequence[currentKeyIndex])
        {
            canKeyBePressed = false;
            key.transform.localPosition = new Vector3(key.transform.localPosition.x, -0.17f, key.transform.localPosition.z);

            if (currentKeyIndex == keySequence.Length - 1)
            {
                Debug.Log("Puzzle completed!");
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
