using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public event Action<BulletController, Collision> OnCollide;
    public event Action<BulletController, Collider> OnTrigger;
    public Rigidbody RigidBody;
    public Collider Collider;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollide?.Invoke(this, collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(this, other);
    }
}
