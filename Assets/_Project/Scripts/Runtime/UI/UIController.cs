using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using MEC;
using NJG.Runtime.Interactables;
using NJG.Runtime.Managers;

namespace NJG.Runtime.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _winScreen;
        [SerializeField]
        private TextMeshProUGUI _winText;
        [SerializeField]
        private TextMeshProUGUI _gameVersionText;

        private List<RevivableTree> _revivableTrees;
        private CoroutineHandle _checkForWinRoutine;
        
        private void Start()
        {
            SetGameVersion();
            
            // TODO: This is just for testing...
            _revivableTrees = new List<RevivableTree>();
            GameObject[] objectives = GameObject.FindGameObjectsWithTag("Objective");
            foreach (GameObject go in objectives)
            {
                if (go.TryGetComponent(out RevivableTree tree))
                {
                    _revivableTrees.Add(tree);
                }
            }

            if (_revivableTrees.Count > 0)
                _checkForWinRoutine = Timing.RunCoroutine(CheckForWinRoutine());
        }

        private void SetGameVersion() => _gameVersionText.text = $"Version: {Application.version}";

        private IEnumerator<float> CheckForWinRoutine()
        {
            float checkInterval = 2f;
            while (true)
            {
                float treesAlive = 0;
                foreach (RevivableTree tree in _revivableTrees.Where(tree => tree.State == RevivableTree.ObjectiveState.Completed))
                    treesAlive++;
                
                if (treesAlive >= _revivableTrees.Count)
                {
                    _winScreen.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    yield break;
                }
                
                yield return Timing.WaitForSeconds(checkInterval);
            }
        }

        public void OnClick_NextLevel() => LevelManager.Instance.LoadNextLevel();

        public void OnClick_Restart() => LevelManager.Instance.RestartLevel();

        public void OnClick_Exit() => LevelManager.Instance.ExitGame();
    }
}
