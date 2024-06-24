using UnityEngine;
using UnityEngine.UI;

public class NewSeed : MonoBehaviour
{
    public Button openPanelButton;   // Button to open the panel
    public Button cancelButton;      // Button to close the panel
    public GameObject panel;         // Panel to be shown/hidden

    void Start()
    {
        // Ensure the panel is initially inactive
        panel.SetActive(false);

        // Add listeners to the buttons
        openPanelButton.onClick.AddListener(OpenPanel);
        cancelButton.onClick.AddListener(ClosePanel);
    }

    void OpenPanel()
    {
        panel.SetActive(true);
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }
}
