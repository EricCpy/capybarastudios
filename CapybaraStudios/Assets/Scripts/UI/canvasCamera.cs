using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject camera;

    void Start()
    {
        StartCoroutine(LateStart(0.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (!camera)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        transform.LookAt(camera.transform);
    }
}