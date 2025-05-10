using DG.Tweening;
using NJG.Runtime.LevelChangeSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NJG.Runtime.UI
{
    public class Splash : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Image _logo;

        [FoldoutGroup("Settings"), SerializeField]
        private float _fadeDuration = 2f;

        private Tween _fadeTween;
        private LevelChangeManager _levelManager;

        [Inject]
        private void Construct(LevelChangeManager levelManager) => _levelManager = levelManager;

        private void Start()
        {
            _logo.color = new Color(_logo.color.r, _logo.color.g, _logo.color.b, 0f);
            FadeIn();
        }

        private void FadeIn()
        {
            _fadeTween?.Kill();
            _fadeTween = _logo.DOFade(1f, _fadeDuration).OnComplete(OnFadedIn);
        }

        private void OnFadedIn() => FadeOut();

        private void FadeOut()
        {
            _fadeTween?.Kill();
            _fadeTween = _logo.DOFade(0f, _fadeDuration).OnComplete(OnFadedOut);
        }

        private void OnFadedOut() => _levelManager.LoadMenuScene();
    }
}