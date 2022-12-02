using Photon.Pun;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    public class Manager_ : MonoBehaviour
    {
        public static Manager_ instance;
        public string player_prefab;
        public Transform[] spawn_point;

        private void Awake()
        {
            instance = this;
        }   

        private void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            Transform t_spawn = spawn_point[Random.Range(0, spawn_point.Length)];
            PhotonNetwork.Instantiate(player_prefab, t_spawn.position, t_spawn.rotation);
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}