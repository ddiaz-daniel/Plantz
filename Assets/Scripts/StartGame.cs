using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartGame : MonoBehaviour
{
    public Button loadSceneCamera;
    public Button loadSceneMouse;

    public string sceneToLoad;

    void Start()
    {
        if (loadSceneMouse != null)
        {
            loadSceneMouse.onClick.AddListener(() => OnLoadSceneButtonClick(1));
        }
        else
        {
        }

        if (loadSceneCamera != null)
        {
            loadSceneCamera.onClick.AddListener(() => OnLoadSceneButtonClick(2));
        }
        else
        {
        }
    }

    void OnLoadSceneButtonClick(int controllerType)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetupControllers(controllerType);
            StartCoroutine(LoadScene());
        }
        else
        {
        }
    }

    IEnumerator LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            yield break;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.UnloadSceneAsync(currentScene);

        SceneManager.LoadScene(sceneToLoad);

    }
}
