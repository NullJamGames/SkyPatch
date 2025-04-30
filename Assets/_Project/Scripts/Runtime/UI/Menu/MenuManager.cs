using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using NJG.Runtime.LevelChangeSystem;

namespace NJG.Runtime.UI
{
    public class MenuManager : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private SPanelOpener[] _panelOpeners;
        [FoldoutGroup("References"), SerializeField]
        private GameObject _menuPanel;
        [FoldoutGroup("References"), SerializeField]
        private Button _playButton;
        [FoldoutGroup("References"), SerializeField]
        private Button _exitButton;
        
        
        
        private List<GameObject> _panels = new List<GameObject>();
        
        private LevelChangeManager _levelChangeManager;

        [Inject]
        void Construct(LevelChangeManager levelChangeManager)
        {
            _levelChangeManager = levelChangeManager;
        }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable()
        {
            
            foreach (var VARIABLE in _panelOpeners)
            {
                _panels.Add(VARIABLE.Panel);
                var panel = VARIABLE.Panel;
                VARIABLE.Button.onClick.AddListener(()=>OpenPanel(panel));
            }
            
            _panels.Add(_menuPanel);
            
            _playButton.onClick.AddListener(OnPlay);
            _exitButton.onClick.AddListener(OnExit);
            
            OpenPanel(_menuPanel);
        }

        private void OnPlay()
        {
            _levelChangeManager.LoadGameScene();
        }
        
        private void OnExit()
        {
            Application.Quit();
        }

        private void OpenPanel(GameObject panel)
        {
            foreach (var VARIABLE in _panels)
                VARIABLE.SetActive(false);
            
            panel.SetActive(true);
        }
    }

    [Serializable]
    public struct SPanelOpener
    {
        public Button Button;
        public GameObject Panel;
    }
}
