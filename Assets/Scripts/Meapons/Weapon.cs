using System;
using ScriptableObjGen;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;

namespace Meapons
{
    public class Weapon : MonoBehaviourPunCallbacks
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
        private float currentCooldown;
        

        private void Update()
        {
            if(photonView.IsMine && Input.GetKeyDown(WeaponUp))
            {
                photonView.RPC("Equip",RpcTarget.All,0);
            };
            
            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {
                    Aim(Input.GetKey(Aimming));
                    if (Input.GetKeyDown(Shooting) && currentCooldown<=0)
                    {
                        photonView.RPC("Shoot", RpcTarget.All);
                    }
                    if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
                }
                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition,Vector3.zero, Time.deltaTime * 4f);
                currentWeapon.transform.localRotation = Quaternion.Lerp(currentWeapon.transform.localRotation,Quaternion.identity, Time.deltaTime * 4f);
            }
        }

        [PunRPC]
        void Equip(int p_ind)
        {
            if(currentWeapon != null) Destroy(currentWeapon);

            currentIndex = p_ind;
            GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab,weaponParent.position,weaponParent.rotation,weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;
            t_newEquipment.GetComponent<Sway>().isMine = photonView.IsMine;
            
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

        [PunRPC]
        void Shoot()
        {
            Transform t_spawn = transform.Find("Cameras/NormalCamera");
            
            //bloom
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();
            
            //raycast
            RaycastHit t_hit = new RaycastHit();
            if (Physics.Raycast(t_spawn.position, t_bloom,out t_hit, 1000f, canBeShot))
            {
                GameObject t_newHole = Instantiate(bulletHolePrefab,t_hit.point+t_hit.normal*0.01f,Quaternion.identity) as GameObject;
                t_newHole.transform.LookAt(t_hit.point+t_hit.normal);
                Destroy(t_newHole,2f);

                if (photonView.IsMine)
                {
                    if (t_hit.collider.gameObject.layer == 12)
                    {
                        //Do damage
                       t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage",RpcTarget.All,loadout[currentIndex].damage);
                    }
                }
            }
            
            //gun fx
            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil,0,0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
            
            //set cooldown
            currentCooldown = loadout[currentIndex].firerate;
        }

        [PunRPC]
         void TakeDamage(int p_damage)
        {
            if (photonView.IsMine)
            {
                GetComponent<Player>().TakeDamage_(p_damage);
            }
        }
    }
}