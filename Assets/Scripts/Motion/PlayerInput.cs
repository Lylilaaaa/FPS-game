using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("===== Settings =====")]
    public Camera normalCamera;
    public LayerMask ground;
    public GameObject groundSenser;
    
    [Header("===== Digital Settings =====")]
    public float originSpeed;
    public float runningSpeedAdd;
    public float jumpForce;
    
    [Header("===== Input Settings =====")]
    public string Jump_;
    public string Run_1;
    public string Run_2;
    
    
    
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
        //获取输入
        float Dright = Input.GetAxisRaw("Horizontal");
        float Dup = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(Run_1) || Input.GetKey(Run_2);
        bool jump = Input.GetKeyDown(Jump_);
        
        //条件检测
        bool isJumping = jump;
        bool isRunning = run && Dup> 0;
        bool isGround = Physics.Raycast(groundSenser.transform.position, Vector3.down, 0.1f, ground); //地面检测
        
        //输出
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
