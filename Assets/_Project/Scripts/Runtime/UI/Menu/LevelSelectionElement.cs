using System;
using NJG.Runtime.LevelChangeSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NJG.Runtime.UI
{
    public class LevelSelectionElement : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private TextMeshProUGUI _levelIndexText;
        [FoldoutGroup("References"), SerializeField]
        private Image _image;
        
        [FoldoutGroup("Sprite"), SerializeField]
        private Sprite _unlockedTexture;

        [FoldoutGroup("Sprite"), SerializeField]
        private Sprite _lockedTexture;
        
        
        private SLevelData _levelData;

        private Action<int> _onLevelSelected;
        

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void InitializeElement(SLevelData levelData, Action<int> onLevelSelected)
        {
            _levelData = levelData;
            _levelIndexText.text = (levelData.LevelIndex + 1).ToString();
            _onLevelSelected = onLevelSelected;
            
            if(_levelData.Islocked)
                InitLocked();
            else 
                InitUnlocked();
                
        }

        private void OnClick()
        {
            if(!_levelData.Islocked)
                _onLevelSelected?.Invoke(_levelData.LevelIndex);
        }

        private void InitUnlocked()
        {
            _image.sprite = _unlockedTexture;
        }
        
        private void InitLocked()
        {
            _image.sprite = _lockedTexture;

            if(_image.TryGetComponent(out ButtonAnimation buttonAnimation))
                Destroy(buttonAnimation);
        }
    }
}
