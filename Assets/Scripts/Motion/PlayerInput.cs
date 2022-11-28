using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float originSpeed;
    public float runningSpeedAdd;
    public Camera normalCamera;
    public float jumpForce;
    public GameObject groundSenser;
    public LayerMask ground;

    private float nowSpeed;
    private Rigidbody rigi;

    private float baseFOV;
    private float runFOVModifier = 1.5f;

    private void Start()
    {
        Camera.main.enabled = false;
        rigi = GetComponent<Rigidbody>();
        nowSpeed = originSpeed;
        baseFOV = normalCamera.fieldOfView;
    }

    private void FixedUpdate()
    {
        float Dright = Input.GetAxisRaw("Horizontal");
        float Dup = Input.GetAxisRaw("Vertical");

        bool run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool jump = Input.GetKeyDown(KeyCode.F);

        bool isJumping = jump;
        bool isRunning = run && Dup> 0;
        bool isGround = Physics.Raycast(groundSenser.transform.position, Vector3.down, 0.1f, ground);
        
        
        Vector3 Ddir = new Vector3(Dright, 0, Dup);
        Ddir.Normalize();

        if (isJumping && isGround)
        {
            rigi.AddForce(Vector3.up*jumpForce*10);
            print("jumping!");
            isRunning = false;
        }
        
        
        if (isRunning) {
            print("running!");
            nowSpeed = originSpeed * runningSpeedAdd;
            normalCamera.fieldOfView = Mathf.Lerp(normalCamera.fieldOfView, baseFOV * runFOVModifier, Time.deltaTime * 8f);
        }
        else
        {
            normalCamera.fieldOfView = Mathf.Lerp( normalCamera.fieldOfView, baseFOV,Time.deltaTime * 8f);
            nowSpeed = originSpeed;
        }
        Vector3 temp = transform.TransformDirection(Ddir) * nowSpeed * Time.deltaTime;
        temp.y = rigi.velocity.y;
        rigi.velocity = temp;
    }
}
