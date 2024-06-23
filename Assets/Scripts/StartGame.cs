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
            loadSceneMouse.onClick.AddListener(OnLoadSceneButtonClick);
            Debug.Log("Button listener added.");
        }
        else
        {
            Debug.LogError("Load scene button is not assigned.");
        }

        if (loadSceneCamera != null)
        {
            loadSceneCamera.onClick.AddListener(OnLoadSceneButtonClick);
            Debug.Log("Button listener added.");
        }
        else
        {
            Debug.LogError("Load scene button is not assigned.");
        }
    }

    void OnLoadSceneButtonClick()
    {
        Debug.Log("Load scene button clicked.");
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        // Check if scene name is assigned
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene to load is not specified.");
            yield break;
        }

        // Unload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"Unloading current scene: {currentScene.name}");
        SceneManager.UnloadScene(currentScene);



        Debug.Log($"Loading new scene: {sceneToLoad}");
        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);



        Debug.Log("New scene loaded successfully.");
    }
}
