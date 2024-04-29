using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlantType : MonoBehaviour
{
    public BigInteger id { get; }
    public string name { get; }
    public float defaultLength { get; }
    public float maxLength { get; }
    public float lengthIncreasePerGameTick { get; }
    public int defaultGrowChance { get; }
    public Mesh mesh { get; }

    public PlantType(BigInteger id, string name, float defaultLength, float maxLength, float lengthIncreasePerGameTick, int defaultGrowChance, Mesh mesh)
    {
        this.id = id;
        this.name = name;
        this.defaultLength = defaultLength;
        this.maxLength = maxLength;
        this.lengthIncreasePerGameTick = lengthIncreasePerGameTick;
        this.defaultGrowChance = defaultGrowChance;
        this.mesh = mesh;
    }
}
