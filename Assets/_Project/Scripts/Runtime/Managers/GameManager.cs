using System;
using NJG.Utilities.Singletons;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class GameManager : IInitializable, ILateDisposable
    {
        public int Energy { get; private set; }

        public GameObject Player { get; private set; }
        
        public void Initialize()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        
        public void LateDispose()
        {
            
        }

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