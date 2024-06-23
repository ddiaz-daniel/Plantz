using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        // Find the TextMeshProUGUI component in the children of this GameObject
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
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


}
