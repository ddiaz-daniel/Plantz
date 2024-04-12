using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVines : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5.0f;
    public float refreshRate = 0.05f;
    [Range(0, 1)]
    public float minGrowAmount = 0.2f;
    [Range(0, 1)]
    public float maxGrowAmount = 0.97f;

    private List<Material> growVinesMaterials = new List<Material>();
    private bool fullyGrown = false;

    void Start()
    {
        for (int i = 0; i < growVinesMeshes.Count; i++)
        {
            for (int j = 0; j < growVinesMeshes[i].materials.Length; j++)
            {
                if (growVinesMeshes[i].materials[j].HasProperty("Grow_"))
                {
                    growVinesMeshes[i].materials[j].SetFloat("Grow_", minGrowAmount);
                    growVinesMaterials.Add(growVinesMeshes[i].materials[j]);
                }
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < growVinesMaterials.Count; i++)
            {
                StartCoroutine(GrowVinesOverTime(growVinesMaterials[i]));
            }
        }

    }

    IEnumerator GrowVinesOverTime(Material material)
    {
        float growValue = material.GetFloat("Grow_");
        if (!fullyGrown)
        {
            while (growValue < maxGrowAmount)
            {
                growValue += refreshRate / timeToGrow;
                material.SetFloat("Grow_", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrowAmount)
            {
                growValue -= refreshRate / timeToGrow;
                material.SetFloat("Grow_", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (growValue >= maxGrowAmount)
        {
            fullyGrown = true;
        }
        else if (growValue <= minGrowAmount)
        {
            fullyGrown = false;
        }
    }
}
