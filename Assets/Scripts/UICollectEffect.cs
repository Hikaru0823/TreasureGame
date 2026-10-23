
using System;
using System.Collections;
using System.Collections.Generic;
using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.UI;

public class UICollectEffect : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private RectTransform endPoint;
    [SerializeField] private RectTransform effectLayer;
    [SerializeField] private RectTransform movingImage;
    [SerializeField] private Image trailPrefab;
    [SerializeField] private Image _bag;

    [Header("Settings")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float arcHeight = 100f;
    [SerializeField] private float spawnInterval = 0.02f;
    [SerializeField] private float trailLifetime = 0.45f;
    [SerializeField] private float scatterRadius = 8f;
    [SerializeField] private Vector2 randomScale =
        new Vector2(0.5f, 1.2f);
    [SerializeField, Min(1f)] private float _bagPunchScale = 1.2f;
    [SerializeField, Min(0f)] private float _bagPunchDuration = 0.25f;

    private Coroutine _moveCoroutine;
    private Vector3 _bagBaseScale;
    private readonly List<Image> _activeTrails = new List<Image>();

    private void Awake()
    {
        if (movingImage != null)
            movingImage.gameObject.SetActive(false);

        if (_bag != null)
            _bagBaseScale = _bag.rectTransform.localScale;
    }

    public void Play(Vector3 startPos, Action onMoved = null)
    {
        if (endPoint == null ||
            effectLayer == null || movingImage == null ||
            trailPrefab == null || _bag == null)
        {
            Debug.LogWarning("UIArcTrailEffect: 参照が未設定です");
            return;
        }

        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _bag.rectTransform.localScale = _bagBaseScale;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(startPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            effectLayer,
            screenPos,
            null,
            out Vector2 localPoint
        );
        _moveCoroutine = StartCoroutine(MoveRoutine(localPoint, onMoved));
    }

    public void Stop()
    {
        StopAllCoroutines();
        _moveCoroutine = null;

        if (movingImage != null)
            movingImage.gameObject.SetActive(false);

        if (_bag != null)
            _bag.rectTransform.localScale = _bagBaseScale;

        foreach (Image trail in _activeTrails)
        {
            if (trail == null)
                continue;

            trail.gameObject.SetActive(false);
            Destroy(trail.gameObject);
        }

        _activeTrails.Clear();
    }

    private IEnumerator MoveRoutine(Vector2 startPos, Action onMoved = null)
    {
        Vector2 start = startPos;
        Vector2 end = ToEffectLocalPoint(endPoint);

        movingImage.gameObject.SetActive(true);
        movingImage.SetAsLastSibling();

        float elapsed = 0f;
        float spawnTimer = spawnInterval;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // イージング
            float moveT = Mathf.SmoothStep(0f, 1f, t);

            // AからBへの移動
            Vector2 position = Vector2.Lerp(start, end, moveT);

            // 放物線（最高点でarcHeight分上昇）
            position.y += 4f * arcHeight * t * (1f - t);

            movingImage.localPosition =
                new Vector3(position.x, position.y, 0f);

            // 移動位置に光の粒を残す
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnTrail(position);
                spawnTimer = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        movingImage.localPosition =
            new Vector3(end.x, end.y, 0f);

        SpawnTrail(end);

        movingImage.gameObject.SetActive(false);
        onMoved?.Invoke();
        yield return AnimateBagPunch();

        _moveCoroutine = null;
        SEManager.Instance.Play(SEPath.SYSTEM23);
    }

    private IEnumerator AnimateBagPunch()
    {
        RectTransform bagRect = _bag.rectTransform;

        if (_bagPunchDuration <= 0f)
        {
            bagRect.localScale = _bagBaseScale;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < _bagPunchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _bagPunchDuration);
            float scale = Mathf.Lerp(1f, _bagPunchScale, Mathf.Sin(t * Mathf.PI));
            bagRect.localScale = _bagBaseScale * scale;
            yield return null;
        }

        bagRect.localScale = _bagBaseScale;
    }

    private Vector2 ToEffectLocalPoint(RectTransform target)
    {
        Canvas canvas =
            effectLayer.GetComponentInParent<Canvas>().rootCanvas;

        Camera cam = canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        Vector2 screenPoint =
            RectTransformUtility.WorldToScreenPoint(
                cam, target.position
            );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            effectLayer,
            screenPoint,
            cam,
            out Vector2 localPoint
        );

        return localPoint;
    }

    private void SpawnTrail(Vector2 position)
    {
        Image particle = Instantiate(trailPrefab, effectLayer);
        _activeTrails.Add(particle);
        particle.gameObject.SetActive(true);
        particle.raycastTarget = false;

        Vector2 offset =
            UnityEngine.Random.insideUnitCircle * scatterRadius;

        Vector2 spawnPos = position + offset;

        particle.rectTransform.localPosition =
            new Vector3(spawnPos.x, spawnPos.y, 0f);

        float scale = UnityEngine.Random.Range(
            randomScale.x, randomScale.y
        );

        particle.rectTransform.localScale *= scale;

        // 移動中のImageより後ろに描画
        particle.rectTransform.SetSiblingIndex(
            movingImage.GetSiblingIndex()
        );

        StartCoroutine(FadeTrail(particle));
    }

    private IEnumerator FadeTrail(Image particle)
    {
        float elapsed = 0f;

        Color startColor = particle.color;
        Vector3 startScale = particle.rectTransform.localScale;

        while (elapsed < trailLifetime)
        {
            float t = Mathf.Clamp01(elapsed / trailLifetime);

            Color color = startColor;
            color.a = startColor.a * (1f - t);
            particle.color = color;

            particle.rectTransform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _activeTrails.Remove(particle);
        Destroy(particle.gameObject);
    }
}
