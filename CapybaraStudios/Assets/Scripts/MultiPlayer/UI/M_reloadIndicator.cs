using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class M_reloadIndicator : MonoBehaviour
{
    private M_GunScript _gunScript;

    [CanBeNull] private Image img;

    // Start is called before the first frame update
    void Start()
    {
        _gunScript = GetComponentInParent<M_GunScript>();
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var status = _gunScript.getReloadStatus();
        if (status == 1)
        {
            img.enabled = false;
        }
        else
        {
            img.enabled = true;
            img.fillAmount = status;
        }
    }
}