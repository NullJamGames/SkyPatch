using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using NJG.Runtime.LevelChangeSystem;
using NJG.Runtime.Managers;

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
        
        private readonly List<GameObject> _panels = new ();
        private GameManager _gameManager;
        private LevelChangeManager _levelChangeManager;

        [Inject]
        private void Construct(GameManager gameManager, LevelChangeManager levelChangeManager)
        {
            _gameManager = gameManager;
            _levelChangeManager = levelChangeManager;
        }

        private void Start()
        {
            _gameManager.ToggleCursor(true);
        }

        private void OnEnable()
        {
            foreach (SPanelOpener panelOpener in _panelOpeners)
            {
                _panels.Add(panelOpener.Panel);
                GameObject panel = panelOpener.Panel;
                panelOpener.Button.onClick.AddListener(()=>OpenPanel(panel));
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
            foreach (GameObject go in _panels)
                go.SetActive(false);
            
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
