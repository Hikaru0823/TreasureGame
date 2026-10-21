using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using EyeMoT.Heatmap;
using UnityEngine;

namespace EyeMoT
{
    public class RecordManager : Singleton<RecordManager>
    {
        [SerializeField] private string _saveDir = "/YOUR_RECORD/";
        [SerializeField] private HeatmapRenderer _heatmapRenderer;
        [SerializeField] private GameRecoder _gameRecoder;
        [SerializeField] private FocusCalmManager _focusCalmManager;
        
        private string _recordPath => System.IO.Path.GetDirectoryName(Application.dataPath) + _saveDir + $"/{Application.productName + "_" + _startDate.ToString("yyyyMMddHHmmss")}/";
        private bool _isRecording = false;
        private ConcurrentQueue<RecordSample> _receivedData = new();
        private List<string[]> _recordedData = new();
        private List<string> _types = new();
        private List<string> _customtypes = new();
        private DateTime _startDate;
        private string _fileName;

        void ClearData()
        {
            _receivedData.Clear();
            _recordedData.Clear();
            _types.Clear();
        }

        public void Enqueue(RecordSample data)
        {
            if (data == null) return;
            if(!_isRecording) return;
            _receivedData.Enqueue(data);
        }

        public void AddCustomType(string[] types)
        {
            foreach (var type in types)
            {
                if (!_customtypes.Contains(type))
                {
                    _customtypes.Add(type);
                    _types.Add(type);
                }
            }
        }

        public void StartRecord(bool activeRecord, bool activeMic)
        {
            ClearData();

            _isRecording = true;
            _startDate = System.DateTime.Now;
            _fileName = Application.productName + "_" + _startDate.ToString("yyyyMMddHHmmss");
            _types.Add("#GameTime");
            if(_customtypes.Count > 0)
            {
                _types.AddRange(_customtypes);
            }
            if(_heatmapRenderer != null)
            {
                _heatmapRenderer.StartHeatmap(true, onDataReceived: Enqueue);
                _types.AddRange(_heatmapRenderer.Type);
            }
            if(_focusCalmManager != null)
            {
                _focusCalmManager?.StartRecord(Enqueue);
                _types.AddRange(_focusCalmManager.Type);
            }
            if(activeRecord)
            {
                _gameRecoder?.RecordStart(_recordPath, _fileName, activeMic);
            }
        }

        public void StopRecord(bool writeCSV = true, List<string[]> header = null, Action<HeatmapResult> onHeatmapComplete = null)
        {
            if (!_isRecording) return;

            Update(); // Flush queued samples before marking the recording complete.
            _isRecording = false;
            try
            {
                _heatmapRenderer?.StopHeatmap(onHeatmapComplete);
            }
            finally
            {
                try { _gameRecoder?.RecordEnd(); }
                finally { _focusCalmManager?.StopRecord(); }
            }
            DrainReceivedData();

            if(writeCSV)
            {
                WriteCsv(_recordedData);
            }
        }

        public void WriteCsv(List<string[]> data)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            return;
            #endif
            data = MergeRowsByTimestamp(data);
            if (!System.IO.Directory.Exists(_recordPath))
            {
                System.IO.Directory.CreateDirectory(_recordPath);
            }

            var csvManager = new CSVManager();
            var writeData = new List<string[]>();
            writeData.Add(new string[] { "#Date" });
            writeData.Add(new string[] { _startDate.ToString("yyyy/MM/dd HH:mm:ss") });
            writeData.Add(new string[] { "#Screen_X", "Screen_Y", "DataCount" });
            writeData.Add(new string[] { Screen.width.ToString(), Screen.height.ToString(), data.Count.ToString() });
            writeData.Add(_types.ToArray());
            writeData.AddRange(data);
            csvManager.CSVWrite(writeData, _recordPath + _fileName + ".csv", isAppend: false);

        }

        private List<string[]> MergeRowsByTimestamp(List<string[]> data)
        {
            var mergedData = new List<string[]>();
            if(data == null) return mergedData;

            var rowsByTimestamp = new SortedDictionary<double, List<string[]>>();
            int eegIndex = _types.IndexOf("EEG");
            foreach(var row in data)
            {
                if(row == null || row.Length == 0) continue;

                string timestamp = row[0];
                if(string.IsNullOrEmpty(timestamp)) continue;
                double time = double.Parse(timestamp, CultureInfo.InvariantCulture);

                if(!rowsByTimestamp.TryGetValue(time, out var rows))
                {
                    rows = new List<string[]>();
                    rowsByTimestamp.Add(time, rows);
                }

                // Preserve every EEG sample, even if two packets share a timestamp.
                bool hasEeg = eegIndex >= 0 && eegIndex < row.Length
                    && !string.IsNullOrEmpty(row[eegIndex]);
                string[] mergedRow = rows.Count > 0 ? rows[rows.Count - 1] : null;
                if(mergedRow == null || (hasEeg && !string.IsNullOrEmpty(mergedRow[eegIndex])))
                {
                    mergedRow = new string[_types.Count];
                    rows.Add(mergedRow);
                }
                int length = Mathf.Min(row.Length, mergedRow.Length);
                for(int i = 0; i < length; i++)
                {
                    if(!string.IsNullOrEmpty(row[i]))
                    {
                        mergedRow[i] = row[i];
                    }
                }
            }

            foreach(var rows in rowsByTimestamp.Values)
            {
                mergedData.AddRange(rows);
            }
            FillEmptyValuesWithLatest(mergedData);
            return mergedData;
        }

        private void FillEmptyValuesWithLatest(List<string[]> data)
        {
            if(data == null || data.Count == 0) return;

            var latestValues = new string[_types.Count];
            for(int i = 1; i < latestValues.Length; i++)
            {
                latestValues[i] = "0";
            }

            foreach(var row in data)
            {
                if(row == null) continue;

                int length = Mathf.Min(row.Length, latestValues.Length);
                for(int i = 1; i < length; i++)
                {
                    if(string.IsNullOrEmpty(row[i]))
                    {
                        row[i] = latestValues[i];
                    }
                    else
                    {
                        latestValues[i] = row[i];
                    }
                }
            }
        }

        void Update()
        {
            if(!_isRecording) return;
            DrainReceivedData();
        }

        private void DrainReceivedData()
        {
            while(_receivedData.TryDequeue(out var data))
            {
                string[] recordData = new string[_types.Count];
                recordData[0] = data.TimeStamp.ToString("R", CultureInfo.InvariantCulture);
                for(int i = 0; i < data.Type.Length; i++)
                {
                    int index = _types.IndexOf(data.Type[i]);
                    if(index >= 0)
                    {
                        recordData[index] = data.Data[i];
                    }
                }
                _recordedData.Add(recordData);
            }
        }

        void OnApplicationQuit()
        {
            if(_isRecording)
            {
                StopRecord(false);
            }
        }
    }

    public class RecordSample
    {
        public double TimeStamp;
        public string[] Type;
        public string[] Data;
    }
}
