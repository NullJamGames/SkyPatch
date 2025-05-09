using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class PipeVisual : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _pipeVisuals;

        public void TurnOnVisuals()
        {
            foreach (GameObject _pipeVisual in _pipeVisuals)
                _pipeVisual.SetActive(true);
        }

        public void TurnOffVisuals()
        {
            foreach (GameObject _pipeVisual in _pipeVisuals)
                _pipeVisual.SetActive(false);
        }
    }
}