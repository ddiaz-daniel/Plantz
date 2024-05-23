using System.Collections;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

/**
 * In the camera is something different which makes a video.
 * This makes screenshots every 10 seconds
 */
public class TimelapseCapture : MonoBehaviour
{
    public float captureInterval = 10f; // Interval in seconds
    private int screenshotCount = 0;
    private string screenshotsFolder;

    private void Start()
    {
        screenshotsFolder = Path.Combine(Application.dataPath, "timelapse_screenshots");
        if (!Directory.Exists(screenshotsFolder))
        {
            Directory.CreateDirectory(screenshotsFolder);
        }
        StartCoroutine(CaptureTimelapse());
    }

    private IEnumerator CaptureTimelapse()
    {
        while (true)
        {
            yield return new WaitForSeconds(captureInterval);
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        string screenshotFilename = Path.Combine(screenshotsFolder, $"screenshot_{screenshotCount:D4}.png");
        ScreenCapture.CaptureScreenshot(screenshotFilename);
        screenshotCount++;
        Debug.Log($"Screenshot saved: {screenshotFilename}");
    }
}
