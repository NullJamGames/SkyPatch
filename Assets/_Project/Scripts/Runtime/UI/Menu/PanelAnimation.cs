using System;
using DG.Tweening;
using UnityEngine;

namespace NJG.Runtime.UI
{
    public class PanelAnimation : MonoBehaviour
    {
        [SerializeField] private float _startScale = 0.2f;
        [SerializeField] private float _animTime = 0.3f;
        
        private void OnEnable()
        {
            Transform tr = transform;
            
            tr.localScale = Vector3.one * _startScale;
            
            tr.DOKill();
            tr.DOScale(Vector3.one, _animTime).SetEase(Ease.OutCubic);
        }
    }
}
