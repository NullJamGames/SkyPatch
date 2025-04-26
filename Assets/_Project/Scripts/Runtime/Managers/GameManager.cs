using System;
using NJG.Utilities.Singletons;
using UnityEngine;

namespace NJG.Runtime.Managers
{
    public class GameManager : SingletonPersistent<GameManager>
    {
        public int Energy { get; private set; }

        public GameObject Player { get; private set; }
        
        public override void Awake()
        {
            base.Awake();
            
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        // private void Start()
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }

        public void AddEnergy(int energy)
        {
            Energy += energy;
        }
        
        public bool HasEnoughEnergy(int energy)
        {
            return Energy >= energy;
        }
    }
}