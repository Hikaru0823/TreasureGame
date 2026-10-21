using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class SettingManager : SceneSingleton<SettingManager>
{
    [SerializeField] private GameDataSelecter _gameDataSelecter;
    [SerializeField] private ChestDataSelecter _chestDataSelecter;

    public void Initialize(SaveData saveData)
    {
        if(saveData != null)
        {
            SettingData.Instance.GameData = saveData.GameData;
            SettingData.Instance.ChestData = saveData.ChestData;
            //_gameDataSelecter.ChestAmount.Initialize(Array.FindIndex(_gameDataSelecter.ChestAmount.GetItems(), x => x.Contains(SettingData.Instance.GameData.ChestAmount.ToString() + " 個")) );
            _gameDataSelecter.GameTime.Initialize(Array.FindIndex(_gameDataSelecter.GameTime.GetItems(), x => x.Contains(SettingData.Instance.GameData.GameTime.ToString() + " 秒")) );
            // _gameDataSelecter.BGColor.Initialize((int)SettingData.Instance.GameData.BGColor);
            // _gameDataSelecter.IsSwitchMode.Initialize(SettingData.Instance.GameData.IsSwitchMode);
            // _gameDataSelecter.ActiveRecord.Initialize(SettingData.Instance.GameData.ActiveRecord);
            // _gameDataSelecter.ActiveMic.Initialize(SettingData.Instance.GameData.ActiveMic);

            // _chestDataSelecter.CollisionScale.Initialize(Array.FindIndex(_chestDataSelecter.CollisionScale.GetItems(), x => x.Contains(ChestData.CollisionScale.ToString("F1") + " 倍")) );
            // _chestDataSelecter.VisualScale.Initialize(Array.FindIndex(_chestDataSelecter.VisualScale.GetItems(), x => x.Contains(ChestData.VisualScale.ToString("F1") + " 倍")) );
            // _chestDataSelecter.LifeTime.Initialize(Array.FindIndex(_chestDataSelecter.LifeTime.GetItems(), x => x.Contains(ChestData.LifeTime.ToString("F1") + " 秒")) );
            // _chestDataSelecter.TargetShape.Initialize(ChestData.TargetShapeIdx);
            // _chestDataSelecter.VFXIdx.Initialize(ChestData.VFXIdx);
        }
        else
        {
            // _gameDataSelecter.ChestAmount.Initialize(-1);
            _gameDataSelecter.GameTime.Initialize(-1);
            // _gameDataSelecter.BGColor.Initialize(-1);
            // _gameDataSelecter.IsSwitchMode.Initialize(-1);
            // _gameDataSelecter.ActiveRecord.Initialize(-1);
            // _gameDataSelecter.ActiveMic.Initialize(-1);

            // _chestDataSelecter.CollisionScale.Initialize(-1);
            // _chestDataSelecter.VisualScale.Initialize(-1);
            // _chestDataSelecter.LifeTime.Initialize(-1);
            // _chestDataSelecter.TargetShape.Initialize(-1);
            // _chestDataSelecter.VFXIdx.Initialize(-1);
        }
    }

    #region Setter (UI Hook)
    public void SetBalloonAmount(string value) => SettingData.Instance.GameData.ChestAmount = int.Parse(value.Replace("個", ""));
    public void SetGameTime(string value) => SettingData.Instance.GameData.GameTime = float.Parse(value.Replace("秒", ""));
    public void SetBGColor(string value) => SettingData.Instance.GameData.BGColor = (BGColor)Enum.Parse(typeof(BGColor), value);
    public void SetIsSwitchMode(bool value) => SettingData.Instance.GameData.IsSwitchMode = value ? 0 : 1;
    public void SetActiveRecord(bool value) => SettingData.Instance.GameData.ActiveRecord = value ? 0 : 1;
    public void SetActiveMic(bool value) => SettingData.Instance.GameData.ActiveMic = value ? 0 : 1;

    public void SetCollisionScale(string value) => SettingData.Instance.ChestData.CollisionScale = float.Parse(value.Replace("倍", ""));
    public void SetVisualScale(string value) => SettingData.Instance.ChestData.VisualScale = float.Parse(value.Replace("倍", ""));
    public void SetLifeTime(string value) => SettingData.Instance.ChestData.LifeTime = float.Parse(value.Replace("秒", ""));
    public void SetTargetShape(int idx) => SettingData.Instance.ChestData.TargetShapeIdx = idx;
    public void SetVFXType(int idx) => SettingData.Instance.ChestData.VFXIdx = idx;
    #endregion
}

public enum BGColor { Default, White, Black}

[Serializable]
public class GameDataSelecter
{
    public SelecterUI ChestAmount;
    public SelecterUI GameTime;
    public SelecterUI BGColor;
    public SelecterUI IsSwitchMode;
    public SelecterUI ActiveRecord;
    public SelecterUI ActiveMic;
}

[Serializable]
public class ChestDataSelecter
{
    public SelecterUI CollisionScale;
    public SelecterUI VisualScale;
    public SelecterUI LifeTime;
    public SelecterUI TargetShape;
    public SelecterUI VFXIdx;
}


