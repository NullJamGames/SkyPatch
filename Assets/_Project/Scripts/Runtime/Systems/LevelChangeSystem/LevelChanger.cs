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
        [FoldoutGroup("UI Elements")]
        [SerializeField] private GameObject _canvas;
        [FoldoutGroup("UI Elements")]
        [SerializeField] Transform _loadingScreen;
        [FoldoutGroup("UI Elements")]
        [SerializeField] private Image _loadingBar;

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

       public async void ChangeScene(string sceneName)
        {
            if (_isChangingScene)
                return;
            _isChangingScene = true;
            
            StartLoadingScreenAnim();
            await Task.Delay(GetWaitTime(_enterTime));
            LoadNewScene(sceneName);
            await Task.Delay(GetWaitTime(_hideDelay));
            HideLoadingScreen();

            _isChangingScene = false;
        }

        private async void LoadNewScene(string sceneName)
        {
            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;
            
            do {
                await Task.Delay(10);
                SetLoadingBarValue(scene.progress);
            } while (scene.progress < 0.9f);
            
            scene.allowSceneActivation = true;
        }


        private void HideLoadingScreen()
        {
            _loadingScreen.DOKill();
            _loadingScreen.DOLocalMove(new Vector3(_endX, 0, 0), _exitTime).SetEase(Ease.InQuint)
                .OnComplete(() => _canvas.SetActive(false));
        }

        private void StartLoadingScreenAnim()
        {
            SetLoadingBarValue(0);
            _canvas.SetActive(true);
            
            Vector3 pos = _loadingScreen.localPosition;
            pos.x = _startX;
            _loadingScreen.localPosition = pos;
            
            _loadingScreen.DOKill();
            _loadingScreen.DOLocalMove(new Vector3(0, 0, 0), _enterTime).SetEase(Ease.OutQuint);
        }

        private void SetLoadingBarValue(float loadPercentage)
        {
            float barPercentage = loadPercentage / 9 * 10 * 100;
            _loadingBar.fillAmount = barPercentage;
        }

        private int GetWaitTime(float waitTime)
        {
            return (int)(waitTime * 1000);
        }
    }
}
