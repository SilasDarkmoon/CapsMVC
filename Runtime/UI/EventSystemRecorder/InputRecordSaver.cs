using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Capstones.UnityEngineEx
{
    public class InputRecordSaver : MonoBehaviour
    {
        public RecorderInput Recorder;

        private int FrameIndex;

        private void Awake()
        {
            FrameIndex = -1;
        }
        private void OnDestroy()
        {
        }
        private void Update()
        {
            if (!Recorder)
            {
                Recorder = GetComponent<RecorderInput>();
            }
            if (!Recorder)
            {
                return;
            }

            var data = Recorder.Data;
            if (!data || data.Frames == null)
            {
                return;
            }

            for (int i = FrameIndex + 1; i < data.Frames.Count; ++i)
            {
                var frame = data.Frames[i];
                FrameIndex = i;
                RecordSaver.EnqueueRecord("Input", frame);
            }
        }

        #region Static Methods for Global Use
        public static RecordedInputData LoadFile(string path)
        {
            if (!PlatDependant.IsFileExist(path))
            {
                return null;
            }
            using (var sr = PlatDependant.OpenReadText(path))
            {
                if (sr == null)
                {
                    return null;
                }
                var data = ScriptableObject.CreateInstance<RecordedInputData>();
                data.Frames = new List<RecordedInputFrame>();
                bool inframe = false;
                string line = null;
                StringBuilder sbframe = new StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "-- StartOfInput --")
                    {
                        inframe = true;
                        sbframe.Clear();
                    }
                    else if (line == "-- EndOfInput --")
                    {
                        inframe = false;
                        if (sbframe.Length > 0)
                        {
                            try
                            {
                                RecordedInputFrame frame = JsonUtility.FromJson<RecordedInputFrame>(sbframe.ToString());
                                data.Frames.Add(frame);
                            }
                            catch (Exception e)
                            {
                                PlatDependant.LogError(e);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (inframe)
                        {
                            sbframe.AppendLine(line);
                        }
                    }
                }

                return data;
            }
        }
        public static RecordedInputData LoadFile()
        {
            var filename = Path.Combine(ThreadSafeValues.LogPath, "rec/record.json");
            return LoadFile(filename);
        }
        #endregion
    }
}