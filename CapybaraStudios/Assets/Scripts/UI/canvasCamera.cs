using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _camera = null;

    void Start()
    {
        StartCoroutine(LateStart(2f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
        {
            _camera = Camera.main.gameObject;
        } else {
            transform.LookAt(_camera.transform);
        }
    }
}