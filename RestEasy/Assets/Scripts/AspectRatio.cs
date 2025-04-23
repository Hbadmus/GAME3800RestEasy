using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatio : MonoBehaviour
{
    public RenderTexture renderTexture;
    
    private float oldHeight;
    private float oldWidth;
    private float heightScale = 1f;
    private float widthScale = 1f;

    // Start is called before the first frame update
    void Start()
    {        
        // Find height and width of gameObject to render
        RectTransform rt = (RectTransform)transform;

        oldHeight = rt.sizeDelta.y;
        oldWidth = rt.sizeDelta.x;

        float hwRatio = oldHeight / oldWidth;
        float whRatio = oldWidth / oldHeight;

        // Release the current RenderTexture
        renderTexture.Release();

        if (Screen.width > Screen.height) {
            SetRenderTexture((int)(Screen.height * whRatio), Screen.height);
        }
        else {
            SetRenderTexture(Screen.width, (int)(Screen.width * hwRatio));
        }

        GetComponent<RawImage>().SetNativeSize();

        ScaleChildren();
    }

    void SetRenderTexture(int width, int height) {
        // Assign the new RenderTexture
        renderTexture.height = height;
        renderTexture.width = width;

        heightScale = height / oldHeight;
        widthScale = width / oldWidth;
    }

    void ScaleChildren() {
        foreach (Transform child in transform) {
            RectTransform childRT = (RectTransform)child;
            childRT.anchoredPosition = new Vector2(childRT.anchoredPosition.x * widthScale, childRT.anchoredPosition.y * heightScale);
            childRT.localScale = new Vector3(childRT.localScale.x * widthScale, childRT.localScale.y * heightScale, childRT.localScale.z);
        }
    }
}
