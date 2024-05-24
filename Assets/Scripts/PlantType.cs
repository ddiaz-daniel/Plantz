using System;
using UnityEngine;

public class PlantType : MonoBehaviour
{
    public int id;
    public string name;
    [Range(0, 1)]
    public float minLength;
    [Range(0, 1)]
    public float maxLength;
    [Range(0, 30)]
    public int defaultGrowthSpeed;
    [Range(0, 100)]
    public int layerValue; //Position Z
    [Range(0, 3)]
    public float maxHeightVariance; //The amount the plant can be lowered at max 
}
