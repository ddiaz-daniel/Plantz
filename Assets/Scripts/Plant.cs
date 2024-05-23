using System;
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

    public Plant(PlantType plantType, Location location, DiceRolls diceRolls)
    {
        Id = Guid.NewGuid();
        this.plantType = plantType;
        this.location = location;
        plantedAt = DateTime.Now;

        //It either grows untl the plantType.maxLength with a LengthFactor of 1
        //Or it grows 50% smaller then the difference between the max and default length.
        float lengthDiff = plantType.maxLength - plantType.defaultLength;
        this.maxLength = plantType.defaultLength - (diceRolls.GetLengthFactor() * lengthDiff);

        //A grow chance from plantType.defaultGrowChance until 100%
        this.growChance = (100 - plantType.defaultGrowChance) / 100 * diceRolls.GetGrowChance() + plantType.defaultGrowChance;
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
}
