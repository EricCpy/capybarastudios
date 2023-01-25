using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class retroMode : MonoBehaviour
{
    public RenderTexture retroTexture;

    // Start is called before the first frame update
    private float scale;
    private int width = 360;
    private int height = 180;

    void Start()
    {
        RefreshMode();
    }

    public void RefreshMode()
    {
        var width = this.width;
        var height = this.height;
        scale = PlayerPrefs.GetFloat("retroScale", 0f);
        if (scale == 0)
        {
            width = Screen.width;
            height = Screen.height;
            scale = 1;
        }

        retroTexture.Release();
        retroTexture.width = (int)(width * scale);
        retroTexture.height = (int)(height * scale);
        retroTexture.Create();
    }
}