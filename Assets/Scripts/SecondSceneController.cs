using UnityEngine;
using UnityEngine.UI;

public class SecondSceneController : MonoBehaviour
{
    public GameObject openPlantsDisplay;

    void Start()
    {
        int controllerType = GameManager.Instance.controllerType;

        Debug.Log("Controller Type: " + controllerType);

        SetupController(controllerType);
    }

    void SetupController(int controllerType)
    {
        if (controllerType == 1)
        {
            openPlantsDisplay.SetActive(true);
        }
        else if (controllerType == 2)
        {
            openPlantsDisplay.SetActive(false);
        }
        else
        {
            Debug.LogError("Unknown controller type");
        }
    }
}
