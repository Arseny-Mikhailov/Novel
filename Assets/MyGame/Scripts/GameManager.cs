using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class MemoryGame : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private RectTransform container;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int rows = 4;
        [SerializeField] private int columns = 4;
        [SerializeField] private float spacing = 10f;
        [SerializeField] private float bounceDuration = 0.5f;
        [SerializeField] private float revealDuration = 3f;
        [SerializeField] private Image winPanel;

        [Header("Sprites")]
        [SerializeField] private List<Sprite> cardFaces;
        [SerializeField] private Sprite cardBack;

        private List<Sprite> shuffledFaces;
        private List<CardController> openedCards;
        private bool isBusy; 
        
        private int matchedPairs;
        private int totalPairs;
        
        private void Start()
        {
            StartAsync().Forget();
        }

        private async UniTaskVoid StartAsync()
        {
            openedCards = new List<CardController>();
            matchedPairs = 0;
            totalPairs = (rows * columns) / 2;

            if (!ValidateSetup()) return;
            ShuffleDeck();
            await PopulateGridAsync();
            isBusy = true;
            await UniTask.Delay(TimeSpan.FromSeconds(revealDuration));
            foreach (Transform t in container)
            {
                var c = t.GetComponent<CardController>();
                if (c != null) c.HideInstant();
            }
            isBusy = false;
        }


        private bool ValidateSetup()
        {
            if (container == null || cardPrefab == null)
            {
                return false;
            }

            return cardFaces != null && cardFaces.Count >= (rows * columns) / 2;
        }

        private void ShuffleDeck()
        {
            shuffledFaces = new List<Sprite>(rows * columns);
            for (var i = 0; i < (rows * columns) / 2; i++)
            {
                shuffledFaces.Add(cardFaces[i]);
                shuffledFaces.Add(cardFaces[i]);
            }
            for (var i = shuffledFaces.Count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                (shuffledFaces[i], shuffledFaces[j]) = (shuffledFaces[j], shuffledFaces[i]);
            }
        }

        private async UniTask PopulateGridAsync()
        {
            var size = CalculateCardSize();
            var offset = CalculateStartOffset(size);

            var tasks = new List<UniTask>();
            var idx = 0;
            for (var r = 0; r < rows; r++)
            for (var c = 0; c < columns; c++)
            {
                var pos = offset + new Vector2(c * (size + spacing), -r * (size + spacing));
                var card = CreateCard(pos, size, shuffledFaces[idx++]);
                card.OnRevealed += HandleCardRevealed;
                tasks.Add(card.AppearAsync(bounceDuration));
            }

            await UniTask.WhenAll(tasks);
        }

        private float CalculateCardSize()
        {
            var s = container.rect.size;
            var totalX = (columns - 1) * spacing;
            var totalY = (rows - 1) * spacing;
            return Mathf.Min((s.x - totalX) / columns, (s.y - totalY) / rows);
        }

        private Vector2 CalculateStartOffset(float size)
        {
            var w = columns * size + (columns - 1) * spacing;
            var h = rows * size + (rows - 1) * spacing;
            return new Vector2(-w / 2 + size / 2, h / 2 - size / 2);
        }

        private CardController CreateCard(Vector2 pos, float size, Sprite face)
        {
            var go = Instantiate(cardPrefab, container);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = Vector2.one * size;
            rect.anchoredPosition = pos;

            var img = go.GetComponent<Image>();
            img.sprite = face;

            var ctrl = go.AddComponent<CardController>();
            ctrl.Initialize(face, cardBack, () => !isBusy && openedCards.Count < 2);
            return ctrl;
        }

        private async void HandleCardRevealed(CardController card)
        {
            openedCards.Add(card);
                
            if (openedCards.Count < 2) return;
            isBusy = true;

            await UniTask.Delay(300);

            var match = openedCards[0].Face == openedCards[1].Face;
            
            foreach (var c in openedCards)
            {
                if (match)
                    await c.MatchAsync(bounceDuration);
                else
                    await c.HideAsync();
            }
            
            if (match)
            {
                matchedPairs++;
                if (matchedPairs >= totalPairs)
                { 
                    OnWin();
                }
            }
            
            openedCards.Clear();
            isBusy = false;
        }

        private void OnWin()
        {
            winPanel.gameObject.SetActive(true);
        }

    }
}
