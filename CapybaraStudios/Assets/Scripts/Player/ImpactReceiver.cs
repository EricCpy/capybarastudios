using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    
    Vector3 impact = Vector3.zero;
    CharacterController character;
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        // apply impact force:
        if (impact.sqrMagnitude > 0.2) {
            character.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
            if(impact.sqrMagnitude <= 0.2) impact = Vector3.zero;
        }
        
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        impact += dir.normalized * force / 3f;
    }
}
