using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    [Header("Resources")]
    [SerializeField]
    private Image _panel;

    [Header("Settings")]
    [SerializeField, Min(0f)]
    private float _fadeTime = 0.2f;

    private Coroutine _loadCoroutine;

    public void LoadScene(string scene)
    {
        if (_loadCoroutine != null)
        {
            return;
        }

        if (_panel == null)
        {
            Debug.LogError($"{nameof(FadeManager)}: Panel is not assigned.", this);
            return;
        }

        _loadCoroutine = StartCoroutine(LoadAnimation(scene));
    }

    private IEnumerator LoadAnimation(string scene)
    {
        RectTransform panelRect = _panel.rectTransform;
        Vector2 centerPosition = Vector2.zero;
        Vector2 hiddenPosition = centerPosition + Vector2.up * GetHiddenOffset(panelRect);

        _panel.gameObject.SetActive(true);
        panelRect.anchoredPosition = hiddenPosition;

        // Cover the current scene from the top before starting the load.
        yield return MovePanel(panelRect, hiddenPosition, centerPosition);
        yield return new WaitForSeconds(1f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene);
        if (loadOperation == null)
        {
            Debug.LogError($"{nameof(FadeManager)}: Could not load scene '{scene}'.", this);
            _panel.gameObject.SetActive(false);
            _loadCoroutine = null;
            yield break;
        }

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Reveal the loaded scene by moving the panel back above the screen.
        hiddenPosition = centerPosition + Vector2.up * GetHiddenOffset(panelRect);
        yield return MovePanel(panelRect, centerPosition, hiddenPosition);

        _panel.gameObject.SetActive(false);
        _loadCoroutine = null;
    }

    private IEnumerator MovePanel(RectTransform panelRect, Vector2 from, Vector2 to)
    {
        if (_fadeTime <= 0f)
        {
            panelRect.anchoredPosition = to;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _fadeTime);
            panelRect.anchoredPosition = Vector2.Lerp(from, to, progress);
            yield return null;
        }

        panelRect.anchoredPosition = to;
    }

    private static float GetHiddenOffset(RectTransform panelRect)
    {
        Canvas canvas = panelRect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas != null ? canvas.transform as RectTransform : null;
        float viewportHeight = canvasRect != null ? canvasRect.rect.height : Screen.height;

        return (viewportHeight + panelRect.rect.height) * 0.5f;
    }
}
