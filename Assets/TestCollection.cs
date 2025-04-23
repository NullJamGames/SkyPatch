using System;
using UnityEngine;

namespace NJG
{
    public class TestCollection : MonoBehaviour
    {
        private TestBox[] _testBoxes;
        
        private void Awake()
        {
            _testBoxes = GetComponentsInChildren<TestBox>();
        }

        private void Start()
        {
            PickRandomBox();
        }

        private void PickRandomBox()
        {
            int randomIndex = UnityEngine.Random.Range(0, _testBoxes.Length);
            TestBox box = _testBoxes[randomIndex];
            box.SetAsHeart();
        }
    }
}
