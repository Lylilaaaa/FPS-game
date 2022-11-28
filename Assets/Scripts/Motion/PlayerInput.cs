using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float speed;
    private Rigidbody rigi;

    private void Start()
    {
        UnityEngine.Camera.main.enabled = false;
        rigi = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float Dright = Input.GetAxis("Horizontal");
        float Dup = Input.GetAxis("Vertical");
        print(Dright);

        Vector3 Ddir = new Vector3(Dright, 0,Dup);
        Ddir.Normalize();

        rigi.velocity = transform.TransformDirection(Ddir) * speed * Time.deltaTime;
    }
}
