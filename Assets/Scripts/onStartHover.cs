using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class onStartHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private TextMeshProUGUI textMeshPro;
    private TextMeshProUGUI title;
    public GameObject startPanel; // Assign in the Inspector
    public GameObject gameTypeSelectionPanel; // Assign in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Find the TextMeshProUGUI component in the children of this GameObject
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        //the title component name is "Title"
        title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component not found in children.");
            return;
        }

        // Initially disable glow effect
        textMeshPro.fontMaterial.EnableKeyword("GLOW_OFF");
    }

    // This function is called when the pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Enable glow effect
        textMeshPro.fontMaterial.DisableKeyword("GLOW_OFF");
        textMeshPro.fontMaterial.EnableKeyword("GLOW_ON");
    }

    // This function is called when the pointer exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        // Disable glow effect
        textMeshPro.fontMaterial.DisableKeyword("GLOW_ON");
        textMeshPro.fontMaterial.EnableKeyword("GLOW_OFF");
    }

    // This function is called when the button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Start the size change coroutine
        StartCoroutine(ChangePanelSize());
    }

    // Coroutine to change the size of the panel
    private IEnumerator ChangePanelSize()
    {
        RectTransform panelRectTransform = startPanel.GetComponent<RectTransform>();
        if (panelRectTransform == null)
        {
            yield break;
        }

        // Get the current width
        float initialWidth = panelRectTransform.sizeDelta.x;
        float targetWidth = 650f;
        float elapsedTime = 0f;
        float duration = 1f;

        // Gradually reduce the width to 650 over 2 seconds
        while (elapsedTime < duration)
        {
            float newWidth = Mathf.Lerp(initialWidth, targetWidth, elapsedTime / duration);
            panelRectTransform.sizeDelta = new Vector2(newWidth, panelRectTransform.sizeDelta.y);
            elapsedTime += Time.deltaTime;

            //alse reduce the size of the title text slowlly by 0.1f
            title.fontSize -= 0.15f;


            yield return null;
        }

        //wait for 1 second
        yield return new WaitForSeconds(0.4f);

        // Ensure the final width is exactly 650
        panelRectTransform.sizeDelta = new Vector2(targetWidth, panelRectTransform.sizeDelta.y);

        // Deactivate start panel
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // Activate game type selection panel
        if (gameTypeSelectionPanel != null)
        {
            gameTypeSelectionPanel.SetActive(true);
        }
    }
}
