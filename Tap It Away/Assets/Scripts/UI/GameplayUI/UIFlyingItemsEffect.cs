using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIFlyingItemsEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform coinIconPrefab;
        [SerializeField] private RectTransform destination;
        [SerializeField] private TMP_Text coinText;
        [SerializeField, Min(1)] private int coinIconCount = 8;
        [SerializeField, Min(0f)] private float flyDuration = 0.65f;
        [SerializeField, Min(0f)] private float staggerDelay = 0.04f;
        [SerializeField, Min(0f)] private float spawnSpread = 45f;
        [SerializeField, Min(0f)] private float startScale = 0.85f;
        [SerializeField, Min(0f)] private float endScale = 0.35f;
        [SerializeField, Min(0f)] private float arrivalSoundLeadTime = 0.08f;

        private RectTransform spawnOrigin;
        private Sequence sequence;
        private readonly List<RectTransform> spawnedIcons = new();
        private int arrivedCoinCount;

        private RectTransform Root => transform as RectTransform;

        public bool IsPlaying => sequence != null && sequence.IsActive();

        public void SetSpawnOrigin(RectTransform origin)
        {
            spawnOrigin = origin;
        }

        public void Play(int startCoins, int rewardAmount, Action onComplete = null)
        {
            if (rewardAmount <= 0)
            {
                onComplete?.Invoke();
                return;
            }

            if (coinIconPrefab == null || destination == null || Root == null)
            {
                Debug.LogWarning("[UIFlyingItemsEffect] Missing coinIconPrefab, destination, or RectTransform root.", this);
                onComplete?.Invoke();
                return;
            }

            Stop();
            gameObject.SetActive(true);

            int iconCount = Mathf.Max(1, coinIconCount);
            arrivedCoinCount = 0;
            Vector3 startPosition = GetStartLocalPosition();
            Vector3 endPosition = Root.InverseTransformPoint(destination.position);

            SetCoinText(startCoins);
            sequence = DOTween.Sequence().SetUpdate(true);
            AudioManager.Instance?.PlayCoinFlySound();

            for (int i = 0; i < iconCount; i++)
            {
                float delay = i * staggerDelay;
                int coinIndex = i;
                sequence.InsertCallback(delay, () =>
                {
                    SpawnAndFlyCoin(coinIndex, iconCount, startPosition, endPosition, startCoins, rewardAmount);
                });
            }

            float lastCoinArrivalTime = flyDuration + (iconCount - 1) * staggerDelay;
            float arrivalSoundTime = Mathf.Max(0f, lastCoinArrivalTime - arrivalSoundLeadTime);
            sequence.InsertCallback(arrivalSoundTime, () =>
            {
                AudioManager.Instance?.PlayCoinFlySound();
            });
            sequence.AppendInterval(0.05f);

            sequence.OnComplete(() =>
            {
                SetCoinText(startCoins + rewardAmount);
                sequence = null;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private void SpawnAndFlyCoin(int coinIndex, int iconCount, Vector3 startPosition, Vector3 endPosition, int startCoins, int rewardAmount)
        {
            RectTransform coinIcon = Instantiate(coinIconPrefab, Root);
            coinIcon.gameObject.SetActive(true);
            coinIcon.localScale = Vector3.one * startScale;
            coinIcon.localPosition = startPosition + GetSpawnOffset(coinIndex, iconCount);
            spawnedIcons.Add(coinIcon);

            coinIcon
                .DOLocalMove(endPosition, flyDuration)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .SetLink(coinIcon.gameObject);

            coinIcon
                .DOScale(endScale, flyDuration)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .SetLink(coinIcon.gameObject)
                .OnComplete(() =>
                {
                    arrivedCoinCount++;
                    int displayedCoins = startCoins + Mathf.RoundToInt(rewardAmount * arrivedCoinCount / (float)iconCount);
                    SetCoinText(displayedCoins);
                    spawnedIcons.Remove(coinIcon);
                    Destroy(coinIcon.gameObject);
                });
        }

        public void Stop()
        {
            if (sequence != null && sequence.IsActive())
            {
                sequence.Kill();
            }

            sequence = null;

            for (int i = spawnedIcons.Count - 1; i >= 0; i--)
            {
                if (spawnedIcons[i] != null)
                {
                    Destroy(spawnedIcons[i].gameObject);
                }
            }

            spawnedIcons.Clear();
        }

        private Vector3 GetStartLocalPosition()
        {
            if (spawnOrigin != null)
            {
                return Root.InverseTransformPoint(spawnOrigin.position);
            }

            return Vector3.zero;
        }

        private Vector3 GetSpawnOffset(int index, int total)
        {
            if (total <= 1)
            {
                return Vector3.zero;
            }

            float angle = index * Mathf.PI * 2f / total;
            float radius = spawnSpread * (0.4f + (index % 3) * 0.3f);
            return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }

        private void SetCoinText(int coins)
        {
            if (coinText == null)
            {
                return;
            }

            coinText.text = coins.ToString();
        }
    }
}
