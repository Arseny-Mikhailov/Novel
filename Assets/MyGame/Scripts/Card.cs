using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Scripts
{
    public class CardController : MonoBehaviour
    {
        public Sprite Face { get; private set; }
        public event Action<CardController> OnRevealed;

        private Sprite back;
        private Image img;
        private RectTransform rect;
        private Button btn;
        private bool isAnimating;
        private bool isRevealed;       
        private Func<bool> canReveal;

        public void Initialize(Sprite face, Sprite backSprite, Func<bool> canRev)
        {
            Face = face;
            back = backSprite;
            canReveal = canRev;
            img = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
            
            img.sprite = face;
            rect.localScale = Vector3.zero;

            btn = GetComponent<Button>();
            btn.onClick.AddListener(Reveal);
            isRevealed = false;   
        }

        public async UniTask AppearAsync(float duration)
        {
            await rect.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce).ToUniTask();
        }

        public void HideInstant()
        {
            rect.localScale = Vector3.zero;
            img.sprite = back;
            rect.localScale = Vector3.one;
            isRevealed = false;  
            btn.interactable = true;
        }

        public async UniTask HideAsync()
        {
            if (isAnimating) return;
            isAnimating = true;

            await rect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutCubic).ToUniTask();
            img.sprite = back;

            await rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).ToUniTask();

            isAnimating = false;
            
            isRevealed = false;
            btn.interactable = true;
        }

        public async UniTask MatchAsync(float duration)
        {
            if (isAnimating) return;
            isAnimating = true;
            await rect.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).ToUniTask();
            Destroy(gameObject);
        }

        private void Reveal()
        {
            RevealAsync().Forget();
        }

        private async UniTaskVoid RevealAsync()
        {
            if (isAnimating || isRevealed || (canReveal != null && !canReveal())) 
                return;

            isAnimating = true;
            
            await rect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InCubic).ToUniTask();
            
            img.sprite = Face;
            
            await rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).ToUniTask();

            isAnimating = false;
            
            isRevealed = true;
            btn.interactable = false; 

            OnRevealed?.Invoke(this);
        }
    }
}
