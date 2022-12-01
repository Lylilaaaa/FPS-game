using System;
using ScriptableObjGen;
using UnityEditor;
using UnityEngine;

namespace Meapons
{
    public class Weapon : MonoBehaviour
    {
        public Gun[] loadout;
        public Transform weaponParent;

        private GameObject currentWeapon;
        

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
        }

        void Equip(int p_ind)
        {
            if(currentWeapon != null) Destroy(currentWeapon);
            GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab,weaponParent.position,weaponParent.rotation,weaponParent) as GameObject;
            t_newEquipment.transform.localPosition = Vector3.zero;
            t_newEquipment.transform.localEulerAngles = Vector3.zero;
            currentWeapon = t_newEquipment;
        }
    }
}