using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rBodies;
    MeshCollider[]meshColliders;
    CharacterController controller;
    Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        rBodies = GetComponentsInChildren<Rigidbody>();
        meshColliders = GetComponentsInChildren<MeshCollider>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        DeactivatePhysics();
    }

    public void DeactivatePhysics() {
        foreach(var r in rBodies) r.isKinematic = true;
        foreach(var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        controller.enabled = true;
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
