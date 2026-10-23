using System;
using UnityEngine;

public class SettingData : Singleton<SettingData>
{
    public GameData GameData {get; set;} = new GameData();
    public ChestData ChestData {get; set;} = new ChestData();

    override protected void OnAwake()
    {
        var saveData = Load();
        SettingManager.Instance.Initialize(saveData);
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public static void Save()
    {
        SaveData saveData = new SaveData
        {
            GameData = SettingData.Instance.GameData,
            ChestData = SettingData.Instance.ChestData
        };

        string json =JsonUtility.ToJson(saveData, true);
        ES3.Save<string>(SaveKeys.GameData, json);
        //Debug.Log(json);
    }

    public static SaveData Load()
    {
        if (!ES3.KeyExists(SaveKeys.GameData))
        {
            // Debug.LogWarning("セーブデータがありません");
            return null;
        }

        string json = ES3.Load<string>(SaveKeys.GameData);
        //Debug.Log(json);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        return saveData;
    }

}

[Serializable]
public class SaveData
{
    public GameData GameData;
    public ChestData ChestData;
}


[Serializable]
public class GameData
{
    public int ChestAmount;
    public float GameTime;
    public BGColor BGColor;
    public int IsSwitchMode;
    public int ActiveRecord;
    public int ActiveMic;
}

[Serializable]
public class ChestData
{
    public float CollisionScale;

    public float VisualScale;
    public float GazeTime;
    public int TargetShapeIdx;
    public int VFXIdx;
}