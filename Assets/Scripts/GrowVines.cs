using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVines : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5.0f;
    public float refreshRate = 0.05f;
    public float finalLength = 1.0f;

    private List<Material> growVinesMaterials = new List<Material>();
    private bool fullyGrown = false;
    private Dictionary<Material, Coroutine> runningCoroutines = new Dictionary<Material, Coroutine>();

    void Start()
    {
        //Init
        foreach (MeshRenderer meshRenderer in growVinesMeshes)
        {
            foreach (Material material in meshRenderer.materials)
            {
                if (material.HasProperty("Grow_"))
                {
                    material.SetFloat("Grow_", 0f);
                    growVinesMaterials.Add(material);
                }
            }
        }

        //Start growing
        foreach (Material material in growVinesMaterials)
        {
            if (!runningCoroutines.ContainsKey(material))
            {
                Coroutine coroutine = StartCoroutine(GrowVinesOverTime(material));
                runningCoroutines[material] = coroutine;
            }
        }
    }

    IEnumerator GrowVinesOverTime(Material material)
    {
        float growValue = material.GetFloat("Grow_");
        float targetValue = fullyGrown ? 0f : finalLength;
        float initialGrowValue = growValue;
        float elapsedTime = 0f;

        while (elapsedTime < timeToGrow)
        {
            elapsedTime += refreshRate;
            growValue = Mathf.Lerp(initialGrowValue, targetValue, elapsedTime / timeToGrow);
            material.SetFloat("Grow_", growValue);
            yield return new WaitForSeconds(refreshRate);
        }

        material.SetFloat("Grow_", targetValue);

        fullyGrown = !fullyGrown;
        runningCoroutines.Remove(material);
    }
}
