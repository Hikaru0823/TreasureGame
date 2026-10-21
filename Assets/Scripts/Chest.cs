using System.Collections;
using System.Collections.Generic;
using KanKikuchi.AudioManager;
using UnityEngine;
using Cinemachine;

public class Chest : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private GameObject _chestTop;
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Collider _mainCollider;
    [SerializeField] private GameObject _mainVisual;
    [SerializeField] private Light _light;
    [SerializeField] private CinemachineVirtualCamera _itemCamera;
    [SerializeField] private Transform _itemSpawnPoint;


    [Header("Settings")]
    [SerializeField] private float _lightmaxRangeAlongtheWay = 1f;
    [SerializeField] private float _lightmaxRangeAtEnd = 3f;
    [SerializeField] private float _openTime = 1.0f;
    [SerializeField] private float _openAngleAlongtheWay = -10f;
    [SerializeField] private float _openAngleAtEnd = -100f;
    [SerializeField, Min(0f)] private float _openAnimationDuration = 0.3f;
    [SerializeField, Min(0f)] private float _openOvershoot = 1.2f;
    [SerializeField] private float _shakeAngle = 2f;
    [SerializeField] private float _shakeFrequency = 15f;

    private Quaternion _mainVisualBaseRotation;
    private MaterialPropertyBlock _propertyBlock;
    private float _initialLightRange;
    private float _openingElapsed;

    private bool _isSelected = false;
    private bool _isOpened = false;
    private bool _hasFallen = false;

#region Unity Methods
    void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = _mainVisual.GetComponentsInChildren<Renderer>(true);
        }
    }

    void Start()
    {
        _mainVisualBaseRotation = _mainVisual.transform.localRotation;
        _initialLightRange = _light.range;
        _openTime = SettingData.Instance?.GameData.GameTime ?? _openTime;
        Stage.Instance.SetIgnoreFloorCollision(_mainCollider);
        SEManager.Instance.Play(SEPath.FALL);
    }

    void Update()
    {
        if(_isSelected)
        {
            _openingElapsed += Time.deltaTime;
            float openingProgress = _openTime > 0f ? Mathf.Clamp01(_openingElapsed / _openTime) : 1f;
            _light.range = Mathf.Lerp(_initialLightRange, _lightmaxRangeAlongtheWay, openingProgress);
            float angleX = Mathf.DeltaAngle(0f, _chestTop.transform.localEulerAngles.x);
            if(openingProgress >= 1f || angleX <= _openAngleAlongtheWay)
            {
                _light.range = _lightmaxRangeAlongtheWay;
                _chestTop.transform.localEulerAngles = new Vector3(_openAngleAlongtheWay, 0, 0);
                _mainVisual.transform.localRotation = _mainVisualBaseRotation;
                OnChestOpen();
                _isOpened = true;
                return;
            }
            float angle = (_openAngleAlongtheWay / _openTime) * Time.deltaTime;
            _chestTop.transform.Rotate(Vector3.right, angle);
            float shakeAngle = Mathf.Sin(Time.time * _shakeFrequency * Mathf.PI * 2f) * _shakeAngle;
            _mainVisual.transform.localRotation = _mainVisualBaseRotation * Quaternion.Euler(0f, 0f, shakeAngle);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            if(_hasFallen)
                return;
            _hasFallen = true;
            _animator.Play("Fell", 0, 0f);
            SEManager.Instance.Play(SEPath.TREASURE_DROP);
        }
    }

#endregion

    private void OnChestOpen()
    {
        if(_isOpened) return;
        OnDeselected();
        SEManager.Instance.Play(SEPath.CHEST_OPEN);
        StartCoroutine(AnimateChestOpen());
    }

    private IEnumerator AnimateChestOpen()
    {
        Vector3 startAngles = _chestTop.transform.localEulerAngles;
        float startAngleX = Mathf.DeltaAngle(0f, startAngles.x);
        float startLightRange = _light.range;
        float elapsed = 0f;
        SpawnItem();

        while (elapsed < _openAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float timeProgress = Mathf.Clamp01(elapsed / _openAnimationDuration);
            _light.range = Mathf.Lerp(startLightRange, _lightmaxRangeAtEnd, timeProgress);

            float t = timeProgress - 1f;
            float progress = 1f + (_openOvershoot + 1f) * t * t * t + _openOvershoot * t * t;
            float angleX = Mathf.LerpUnclamped(startAngleX, _openAngleAtEnd, progress);
            _chestTop.transform.localEulerAngles = new Vector3(angleX, startAngles.y, startAngles.z);
            yield return null;
        }

        _chestTop.transform.localEulerAngles = new Vector3(_openAngleAtEnd, startAngles.y, startAngles.z);
        _light.range = _lightmaxRangeAtEnd;
        _itemCamera.Priority = 20;
    }

    private void SpawnItem()
    {
        GameObject itemPrefab = ItemHolder.Instance.GetRandomItem();
        if (itemPrefab != null)
        {
            GameObject itemInstance = Instantiate(itemPrefab, _itemSpawnPoint.position, Quaternion.identity, _itemSpawnPoint);
            
        }
    }

    private void SetOutlineColor(Color color)
    {
        foreach (Renderer renderer in _renderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_OutlineColor", color);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public void OnSelected()
    {
        if(_isOpened)
            return;

        _isSelected = true;
        SEManager.Instance.Play(SEPath.SHAKE);
        SetOutlineColor(Color.yellow);
    }

    public void OnDeselected()
    {
        _isSelected = false;
        _mainVisual.transform.localRotation = _mainVisualBaseRotation;
        SEManager.Instance.Stop(SEPath.SHAKE);
        SetOutlineColor(Color.black);
    }
}
