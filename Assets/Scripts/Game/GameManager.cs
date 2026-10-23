using System.Collections;
using System.Collections.Generic;
using EyeMoT;
using TMPro;
using UnityEngine;

public class GameManager : SceneSingleton<GameManager>
{
    [Header("Resources")]
    [SerializeField] private TabManager _mainTab;
    [SerializeField] private UICollectEffect _effect;
    [SerializeField] private TMP_Text _itemCountText;
    [SerializeField] private TMP_Text _gameTimeText;

    private int _itemCount = 0;
    private float _gameTime = 0;
    private float _gameLimitTime = 10;
    private bool _isStart = false;
    private bool _isItemGetting = false;

    void Start()
    {
        _isStart = true;
        _gameLimitTime = SettingData.Instance?.GameData.GameTime ?? _gameLimitTime;
    }

    void Update()
    {
        if(_isStart)
        {
            _gameTime += Time.deltaTime;
            var timeRemaining = _gameLimitTime - _gameTime;
            _gameTimeText.text = timeRemaining.ToString("F1") + " s";
            if(timeRemaining <= 0)
            {
                _gameTimeText.text = "0.0 s";
                _gameTimeText.color = Color.red;
                _isStart = false;
                if(!_isItemGetting) //エフェクト終わったら終了
                    GameEnd();
            }
        }
    }

    void Init()
    {
        _itemCount = 0;
        _gameTime = 0;
    }

    void GameEnd()
    {
        Init();
        _mainTab.OpenPanel("Result");
    }

    public void ReturnTitle()
    {
        GameEnd();
        FadeManager.Instance.LoadScene("TitleScene");
    }

    public void OnGetItem()
    {
        _itemCount++;
        _isItemGetting = true;
    }

    public void PlayEffect(Vector3 itemPos)
    {
        _effect.Play(itemPos, OnEffectFinished);
    }

    public void OnEffectFinished()
    {
        _isItemGetting = false;
        _itemCountText.text = _itemCount.ToString();

        if(!_isStart)
            GameEnd();
    }
}
