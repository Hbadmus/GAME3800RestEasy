using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatio : MonoBehaviour
{
    public RenderTexture renderTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Find height and width of Tutorial gameObject
        RectTransform rt = (RectTransform)transform;

        float height = rt.sizeDelta.y;
        float width = rt.sizeDelta.x;

        float hwRatio = height / width;
        float whRatio = width / height;

        if (Screen.width < Screen.height) {
            renderTexture.width = Screen.width;
            renderTexture.height = (int)(renderTexture.width * hwRatio);
        }
        else {
            renderTexture.height = Screen.height;
            renderTexture.width = (int)(renderTexture.height * whRatio);
        }

        GetComponent<RawImage>().SetNativeSize();
    }
}
