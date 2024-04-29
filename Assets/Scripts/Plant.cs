using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Guid Id { get; private set; }
    public PlantType plantType { get; }
    public float currentLength { get; set; } = 0;
    public float maxLength { get; set; }
    public Location location { get; } //The location of the stem (or simply the center of the mesh)
    public DateTime plantedAt { get; }
    public int growChance { get; set; } // 0% - 100%

    public Plant(PlantType plantType, Location location)
    {
        Id = Guid.NewGuid();
        this.plantType = plantType;
        this.location = location;
        plantedAt = DateTime.Now;

        //Generate a default length variating by 10%, with a minimum of 1
        this.maxLength = Mathf.Max(1, UnityEngine.Random.Range(this.plantType.defaultLength * 0.95f, this.plantType.defaultLength * 1.05f));

        //Generate a default growChance variating by 2%, with a minimum of 1%
        this.growChance = Mathf.Max(1, (int)UnityEngine.Random.Range(this.plantType.defaultGrowChance - 1, this.plantType.defaultGrowChance + 1));
    }

    public void GiveWater()
    {
        // The max length is increased by 5%
        float newMaxLength = this.maxLength * 1.05f;
        if (newMaxLength <= this.plantType.maxLength)
        {
            this.maxLength = newMaxLength;
        }
    }

    public void GiveFertilization()
    {
        //The growspeed is increased by 1%
        int newGrowChance = this.growChance + 1;
        if (newGrowChance > 100)
        {
            this.growChance = 100;
        }
        else
        {
            this.growChance = newGrowChance;
        }
    }

    public void UpdateGameTick()
    {
        if (this.growChance > 0)
        {
            // It has a chance to grow every game tick
            float growChance = UnityEngine.Random.Range(1, 101);
            if (growChance <= this.growChance)
            {
                // Grow a tiny bit
                float newLength = this.currentLength + this.plantType.lengthIncreasePerGameTick;
                if (newLength > this.maxLength)
                {
                    this.currentLength = this.maxLength;
                    this.growChance = 0; //Stop growing
                }
                else
                {
                    this.currentLength = newLength;
                }

                Debug.Log("PLANT GREW: " + Id.ToString() + " CURRENT LENGTH: " + currentLength.ToString());
            }
        }
    }

    public bool CheckInFacinity(Location location)
    {
        // 1f is hardcoded faccinity for now, but if you want every planttype could have a custom value
        return this.location.CalcDistance(location) < 1f;
    }
}
