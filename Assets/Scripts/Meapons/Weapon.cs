using System;
using ScriptableObjGen;
using UnityEditor;
using UnityEngine;

namespace Meapons
{
    public class Weapon : MonoBehaviour
    {
        [Header("===== Default Setting =====")]
        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject bulletHolePrefab;
        public LayerMask canBeShot;

        [Header("===== Input Setting =====")]
        public string WeaponUp = "Alpha1";
        public string Aimming = "mouse 1";
        public string Shooting = "mouse 0";

        private int currentIndex;
        private GameObject currentWeapon;
        

        private void Update()
        {
            if(Input.GetKeyDown(WeaponUp)) Equip(0);
            if (currentWeapon != null)
            {
                Aim(Input.GetKey(Aimming));
                if (Input.GetKeyDown(Shooting))
                {
                    Shoot();
                }
            }
        }

        void Equip(int p_ind)
        {
            if(currentWeapon != null) Destroy(currentWeapon);

            currentIndex = p_ind;
            GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab,weaponParent.position,weaponParent.rotation,weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;
            
            currentWeapon = t_newEquipment;
        }

        void Aim(bool p_isAiming)
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_state_ads = currentWeapon.transform.Find("State/ADS");
            Transform t_state_hip = currentWeapon.transform.Find("State/Hip");
            if(p_isAiming)
            {
                t_anchor.position = Vector3.Lerp(t_anchor.position,t_state_ads.position,Time.deltaTime*loadout[currentIndex].aimSpeed);
            }
            else
            {
                t_anchor.position = Vector3.Lerp(t_anchor.position,t_state_hip.position,Time.deltaTime*loadout[currentIndex].aimSpeed);
            }
        }

        void Shoot()
        {
            Transform t_spawn = transform.Find("Cameras/NormalCamera");
            
            //bloom
            
            
            RaycastHit t_hit = new RaycastHit();
            if (Physics.Raycast(t_spawn.position, t_spawn.forward,out t_hit, 1000f, canBeShot))
            {
                GameObject t_newHole = Instantiate(bulletHolePrefab,t_hit.point+t_hit.normal*0.01f,Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt(t_hit.point+t_hit.normal);
                Destroy(t_newHole,2f);
            }
        }
    }
}