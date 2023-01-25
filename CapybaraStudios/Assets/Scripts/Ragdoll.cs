using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rBodies;
    MeshCollider[] meshColliders;
    Animator animator;
    ClientNetworkAnimator canimator;
    // Start is called before the first frame update
    void Awake()
    {
        rBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        meshColliders = GetComponentsInChildren<MeshCollider>();
        DeactivatePhysics();
    }

    public void DeactivatePhysics() {
        foreach(var r in rBodies) r.isKinematic = true;
        foreach(var collider in GetComponentsInChildren<Collider>()) if(collider.GetType() != typeof(CharacterController)) collider.enabled = false;
        foreach(var collider in meshColliders) collider.enabled = true;
        animator.enabled = true;
    }

    public void EnablePhysics() {
        foreach(var r in rBodies) r.isKinematic = false;
        foreach(var collider in GetComponentsInChildren<Collider>()) collider.enabled = true;
        foreach(var collider in meshColliders) collider.enabled = false;
        animator.enabled = false;
    }
}
