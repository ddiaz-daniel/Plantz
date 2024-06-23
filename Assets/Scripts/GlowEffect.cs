using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlowEffect : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    public float minGlowPower = 0f;
    public float maxGlowPower = 0.4f;
    public float glowSpeed = 1f;

    private Material textMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Get the TextMeshPro component
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component not found.");
            return;
        }

        // Get the material instance of the TextMeshPro component
        textMaterial = textMeshPro.fontMaterial;

        // Start the glow effect coroutine
        StartCoroutine(GlowEffectCoroutine());
    }

    // Coroutine to animate the glow power
    private IEnumerator GlowEffectCoroutine()
    {
        float glowPower = 0f;
        bool increasing = true;

        while (true)
        {
            if (increasing)
            {
                glowPower += Time.deltaTime * glowSpeed;
                if (glowPower >= maxGlowPower)
                {
                    glowPower = maxGlowPower;
                    increasing = false;
                }
            }
            else
            {
                glowPower -= Time.deltaTime * glowSpeed;
                if (glowPower <= minGlowPower)
                {
                    glowPower = minGlowPower;
                    increasing = true;
                }
            }

            // Apply the glow power to the material
            textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);

            yield return null;
        }
    }
}
