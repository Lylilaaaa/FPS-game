using System;
using UnityEngine;

namespace CameraS
{
    public class Look: MonoBehaviour
    {
        public Transform player;
        public Transform cams;

        public float xSensitivity;
        public float ySensitivity;
        public float maxAngle;

        private Quaternion camCenter;

        private void Start()
        {
            camCenter = cams.localRotation;
        }

        private void FixedUpdate()
        {
            SetY();
            SetX();
        }

        void SetX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            print("mouse X"+Input.GetAxis("Mouse X"));
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;
        }

        void SetY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            print("mouse Y"+Input.GetAxis("Mouse Y"));
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cams.localRotation * t_adj;
            if (Quaternion.Angle(camCenter,t_delta) < maxAngle)
                cams.localRotation = t_delta;
        }
    }
}