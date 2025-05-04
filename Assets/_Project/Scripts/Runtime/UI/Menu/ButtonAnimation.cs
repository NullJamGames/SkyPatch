using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NJG.Runtime.UI
{
    public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _hoverScale = 1.2f;
        [SerializeField] private float _animTime = 0.2f;

        private Tween _tween;
        
        private void OnDisable()
        {
            _tween.Kill();
            transform.localScale = Vector3.one;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween.Kill();
            _tween = transform.DOScale(Vector3.one * _hoverScale, _animTime)
                .SetEase(Ease.Linear)
                .SetUpdate(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween.Kill();
            _tween = transform.DOScale(Vector3.one, _animTime)
                .SetEase(Ease.Linear)
                .SetUpdate(true);
        }
    }
}
