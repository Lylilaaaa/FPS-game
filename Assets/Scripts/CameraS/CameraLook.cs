using System;
using UnityEngine;
using Photon.Pun;


namespace CameraS
{
    public class CameraLook: MonoBehaviourPunCallbacks
    {
        public Transform player;
        public Transform cams;
        public Transform weapon;

        public float xSensitivity;
        public float ySensitivity;
        public float maxAngle;

        private Quaternion camCenter;
        public bool cursorLocked = true;

        private void Start()
        {
            camCenter = cams.localRotation;
        }

        private void FixedUpdate()
        {
            if(!photonView.IsMine) return;
            SetY();
            SetX();
            CursorLock();
        }

        void SetX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            //print("mouse X"+Input.GetAxis("Mouse X"));
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;
            weapon.rotation = cams.rotation;
        }

        void SetY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            //print("mouse Y"+Input.GetAxis("Mouse Y"));
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cams.localRotation * t_adj;
            if (Quaternion.Angle(camCenter,t_delta) < maxAngle) cams.localRotation = t_delta;
            weapon.rotation = cams.rotation;
        }

        private void CursorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = true;
                }
            }
        }
    }
}