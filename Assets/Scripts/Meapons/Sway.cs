using System;
using UnityEngine;
using Photon.Pun;

namespace Meapons
{
    public class Sway : MonoBehaviourPunCallbacks
    {
        public float intensity;
        public float smooth;
        public bool isMine;

        private Quaternion origin_rotation;

        private void Start()
        {
            origin_rotation = transform.localRotation;
        }

        private void Update()
        {
            if(!photonView.IsMine) return;
            UpdateSway();
        }

        private void UpdateSway()
        {
            float t_x_mouse = Input.GetAxis("Mouse X");
            float t_y_mouse = Input.GetAxis("Mouse Y");
            if (!isMine)
            {
                t_x_mouse = 0;
                t_y_mouse = 0;
            }

            Quaternion t_x_adj = Quaternion.AngleAxis(-intensity* t_x_mouse, Vector3.up);
            Quaternion t_y_adj = Quaternion.AngleAxis(intensity* t_y_mouse, Vector3.right);
            Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;
            
            transform.localRotation = Quaternion.Lerp(transform.localRotation,target_rotation,Time.deltaTime*smooth);
        }
    }
}