using System;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Guid Id { get; private set; }
    public PlantType plantType { get; }
    public float finalLength { get; set; }
    public Vector3 position { get; set; }

    public DateTime plantedAt { get; }
    public float growSpeed { get; set; }
    public Quaternion rotation { get; set; }

    public Plant(PlantType plantType, float positionX, DiceRolls diceRolls)
    {
        Id = Guid.NewGuid();
        this.plantType = plantType;

        float heightVariance = diceRolls.GetHeightFactor() * plantType.maxHeightVariance - plantType.maxHeightVariance;

        //if plant type is 1, z position is a random number between -3 and 12
        if (plantType.layerValue == 1)
        {
            this.position = new Vector3(positionX, 0.5f, UnityEngine.Random.Range(-3, 12));
        }
        // if plant type is 2, z position is a random number between -4 and 13
        else if (plantType.layerValue == 2)
        {
            this.position = new Vector3(positionX, 0.5f, UnityEngine.Random.Range(13, 15));
        }
        // if plant type is 3, z position is a random number between -5 and 14
        else if (plantType.layerValue == 3)
        {
            this.position = new Vector3(positionX, 0.5f, 17);
        }
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
