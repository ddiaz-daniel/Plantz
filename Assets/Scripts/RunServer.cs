using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.IO;
using TMPro;

public class RunServer : MonoBehaviour
{
    public GameObject connectButton;
    private Process pythonProcess;
    public GameObject FeedbackMessage;

    void Start()
    {
        Button btn = connectButton.GetComponent<Button>();
        btn.onClick.AddListener(OnConnectToServerClick);
    }

    public void OnConnectToServerClick()
    {
        FeedbackMessage.SetActive(true);
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            UnityEngine.Debug.LogWarning("Python process is already running.");
            return;
        }

        string pythonScriptPath = Application.dataPath + "/Scripts/pythonScript.py";

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "python";
        startInfo.Arguments = pythonScriptPath;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        pythonProcess = new Process();
        pythonProcess.StartInfo = startInfo;

        pythonProcess.EnableRaisingEvents = true;
        pythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        pythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();

        connectButton.SetActive(false);
        FeedbackMessage.GetComponent<TextMeshProUGUI>().text = "Connected!";
        FeedbackMessage.GetComponent<TextMeshProUGUI>().color = Color.green;

    }

    public void StopPythonProcess()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess = null;
            UnityEngine.Debug.Log("Python process stopped.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Python process is not running.");
        }
    }

    void OnApplicationQuit()
    {
        StopPythonProcess();
    }
}
