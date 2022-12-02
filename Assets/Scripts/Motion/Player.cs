using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Systems;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("===== Settings =====")]
    public Camera normalCamera;
    public LayerMask ground;
    public GameObject groundSenser;
    public Transform weaponParent;
    public GameObject cameraParent;
    //public Manager_ manager_;

    [Header("===== Digital Settings =====")]
    public float originSpeed;
    public float runningSpeedAdd;
    public float jumpForce;
    public float runFOVModifier;
    public float maxHealth;
    
    [Header("===== Input Settings =====")]
    public string Jump_;
    public string Run_1;
    public string Run_2;

    
    private float nowSpeed;
    private Rigidbody rigi;

    private Vector3 weaponParentOrigin;
    private float idleCounter;
    private float movementCounter;
    private Vector3 targetWeaponBodPos;

    private float baseFOV;

    private float currHealth;

    private Transform ui_healthbar;
    private Manager_ _manager;

    private void Start()
    {
        _manager = GameObject.Find("Manager").GetComponent<Manager_>();
        currHealth = maxHealth;
        cameraParent.SetActive(photonView.IsMine);
        if (!photonView.IsMine)
        {
            gameObject.layer = 12;
        }
        
        if(Camera.main) Camera.main.enabled = false;
        rigi = GetComponent<Rigidbody>();
        nowSpeed = originSpeed;
        baseFOV = normalCamera.fieldOfView;
        weaponParentOrigin = weaponParent.localPosition;

        if (photonView.IsMine)
        {
            ui_healthbar = GameObject.Find("HUD/Health/Bar").transform;
            RefreshHealthBar();
        }
    }
    
    private void FixedUpdate()
    {
        if(!photonView.IsMine) return;
        
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
        
        if(Input.GetKeyDown(KeyCode.U)) TakeDamage_(100);
        
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

        if (Dright == 0 && Dup == 0)
        {
            HeadBob(idleCounter,0.025f,0.025f);
            idleCounter += Time.deltaTime*2f;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition,targetWeaponBodPos,Time.deltaTime*2f);
        }
        else if(!isRunning)
        {
            HeadBob(movementCounter,0.05f,0.05f);
            movementCounter += Time.deltaTime *3f;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition,targetWeaponBodPos,Time.deltaTime*8f);
        }
        else
        {
            HeadBob(movementCounter,0.3f,0.05f);
            movementCounter += Time.deltaTime *5f;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition,targetWeaponBodPos,Time.deltaTime*10f);
        }

        RefreshHealthBar();
    }

    void RefreshHealthBar()
    {
        float t_health_ratio = (float)currHealth / (float)maxHealth;
        ui_healthbar.localScale = Vector3.Lerp( ui_healthbar.localScale, new Vector3(t_health_ratio,1,1), Time.deltaTime*8f);
    }

    void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        targetWeaponBodPos = weaponParentOrigin + new Vector3( (float)Math.Cos(p_z) * p_x_intensity, (float)Math.Cos(p_z) * p_y_intensity*2,
            0f);
    }
    
    [PunRPC]
    public void TakeDamage_(int p_damage)
    {
        if (photonView.IsMine)
        {
            currHealth -= p_damage;
            RefreshHealthBar();
            Debug.Log("take damage!,now: " + currHealth);
            if (currHealth <= 0)
            {
                Debug.Log("you die");
                _manager.Spawn();
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
