using System;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Guid Id { get; private set; }
    public PlantType plantType { get; }
    public float finalLength { get; set; }
    public Vector3 position { get; } //The location of the stem (or simply the center of the mesh)
    public DateTime plantedAt { get; }
    public float growSpeed { get; set; }
    public Quaternion rotation { get; set; }

    public Plant(PlantType plantType, float positionX, DiceRolls diceRolls)
    {
        Id = Guid.NewGuid();
        this.plantType = plantType;

        float heightVariance = diceRolls.GetHeightFactor() * plantType.maxHeightVariance - plantType.maxHeightVariance;
        this.position = new Vector3(positionX, heightVariance, plantType.layerValue);
        plantedAt = DateTime.Now;

        this.rotation = Quaternion.Euler(0, diceRolls.GetRotationY(), diceRolls.GetRotationZ());

        //It either grows untl the plantType.maxLength with a LengthFactor of 1
        //Or it grows 50% smaller then the difference between the max and default length.
        float lengthDiff = plantType.maxLength - plantType.minLength;
        this.finalLength = Mathf.Min(plantType.minLength + (diceRolls.GetLengthFactor() * lengthDiff), plantType.maxLength);

        //A grow chance from plantType.defaultGrowChance until 100%
        this.growSpeed = plantType.defaultGrowthSpeed * diceRolls.GetGrowFactor();
    }
}
