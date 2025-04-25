using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MEC;

namespace NJG
{
    public class TestWinScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _winScreen;
        [SerializeField]
        private TextMeshProUGUI _winText;

        private List<TestRevivableTree> _revivableTrees;
        private CoroutineHandle _checkForWinRoutine;
        
        private void Start()
        {
            // TODO: This is just for testing...
            _revivableTrees = new List<TestRevivableTree>();
            GameObject[] objectives = GameObject.FindGameObjectsWithTag("Objective");
            foreach (GameObject go in objectives)
            {
                if (go.TryGetComponent(out TestRevivableTree tree))
                {
                    _revivableTrees.Add(tree);
                }
            }

            if (_revivableTrees.Count > 0)
                _checkForWinRoutine = Timing.RunCoroutine(CheckForWinRoutine());
        }

        private IEnumerator<float> CheckForWinRoutine()
        {
            float checkInterval = 2f;
            while (true)
            {
                float treesAlive = 0;
                foreach (TestRevivableTree tree in _revivableTrees.Where(tree => tree.IsRevived))
                    treesAlive++;
                
                if (treesAlive >= _revivableTrees.Count)
                {
                    _winScreen.SetActive(true);
                    yield break;
                }
                
                yield return Timing.WaitForSeconds(checkInterval);
            }
        }

        public void OnClick_Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void OnClick_Exit()
        {
            Application.Quit();
        }
    }
}
