using System;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger Instance;

    private Logger()
    { }

    public void WriteLogMessage(string message)
    {
        string path = Application.dataPath + "/Log.txt";
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(message + " " + DateTime.Now);
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.WriteLogMessage("Game started!");
        }
    }
}
