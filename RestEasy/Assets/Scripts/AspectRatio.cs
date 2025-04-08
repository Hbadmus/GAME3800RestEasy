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
        // Find height and width of gameObject to render
        RectTransform rt = (RectTransform)transform;

        float height = rt.sizeDelta.y;
        float width = rt.sizeDelta.x;

        float hwRatio = height / width;
        float whRatio = width / height;

        // R elease the current RenderTexture
        renderTexture.Release();

        if (Screen.width > Screen.height) {
            SetRenderTexture((int)(Screen.height * whRatio), Screen.height);
        }
        else {
            SetRenderTexture(Screen.width, (int)(Screen.width * hwRatio));
        }

        GetComponent<RawImage>().SetNativeSize();
    }

    void SetRenderTexture(int width, int height) {
        // Assign the new RenderTexture
        renderTexture.height = height;
        renderTexture.width = width;
    }
}
