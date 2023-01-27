using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAdjuster : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        // Set this camera to render after the main camera
        cam.depth = +100;
    }
}
