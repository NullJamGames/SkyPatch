using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace NJG.Runtime.LevelChangeSystem
{
	public class LevelChanger : MonoBehaviour
	{
        [FoldoutGroup("UI Elements"), SerializeField]
        private GameObject _canvas;
        [FoldoutGroup("UI Elements"), SerializeField]
        private Transform _loadingScreen;
        [FoldoutGroup("UI Elements"), SerializeField]
        private Image _loadingBar;

        [FoldoutGroup("Animation"), SerializeField]
        private float _startX = -1920;
        [FoldoutGroup("Animation"), SerializeField]
        private float _endX = 1920;
        [FoldoutGroup("Animation"), SerializeField]
        private float _enterTime = 0.5f;
        [FoldoutGroup("Animation"), SerializeField]
        private float _exitTime = 0.5f;

        [FoldoutGroup("Delay Times")] [SerializeField]
        private float _hideDelay = 0.1f;


        private bool _isChangingScene;

       public async Task ChangeSceneAsync(string sceneName)
        {
            if (_isChangingScene)
                return;
            
            _isChangingScene = true;

            try
            {
                StartLoadingScreenAnim();
                await Task.Delay(GetMilliseconds(_enterTime));

                AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(sceneName);
                if (sceneLoadOp == null)
                {
                    Debug.LogError($"[LevelChanger] Failed to load scene: {sceneName}");
                    return;
                }

                sceneLoadOp.allowSceneActivation = false;

                while (sceneLoadOp.progress < 0.9f)
                {
                    SetLoadingBarValue(sceneLoadOp.progress);
                    await Task.Delay(10);
                }

                SetLoadingBarValue(1f);
                sceneLoadOp.allowSceneActivation = true;

                await Task.Delay(GetMilliseconds(_hideDelay));
                HideLoadingScreen();
            }
            finally
            {
                _isChangingScene = false;
            }
        }

        private void StartLoadingScreenAnim()
        {
            if (_canvas != null) _canvas.SetActive(true);
            if (_loadingBar != null) _loadingBar.fillAmount = 0f;
            if (_loadingScreen == null) return;
            
            _loadingScreen.DOKill();
            _loadingScreen.localPosition = new Vector3(_startX, 0f, 0f);
            _loadingScreen.DOLocalMove(Vector3.zero, _enterTime).SetEase(Ease.OutQuint);
        }

        private void HideLoadingScreen()
        {
            if (_loadingScreen == null || _canvas == null)
                return;
            
            _loadingScreen.DOKill();
            _loadingScreen
                .DOLocalMove(new Vector3(_endX, 0, 0), _exitTime)
                .SetEase(Ease.InQuint)
                .OnComplete(() => _canvas.SetActive(false));
        }

        private void SetLoadingBarValue(float progress)
        {
            if (_loadingBar == null)
                return;
            
            _loadingBar.fillAmount = Mathf.Clamp01(progress / 0.9f);
            //float barPercentage = loadPercentage / 9 * 10 * 100;
            //_loadingBar.fillAmount = barPercentage;
        }

        private static int GetMilliseconds(float seconds) => Mathf.RoundToInt(seconds * 1000f);
    }
}
