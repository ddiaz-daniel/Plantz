using System.Collections.Generic;
using UnityEngine;

public class GameObjectsStorage : MonoBehaviour
{
    public List<GameObject> plantPrefabs;

    void Start()
    {
        PlantSeed(0, 0, new DiceRolls());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DiceRolls diceRolls = new DiceRolls();
            PlantSeed(Random.Range(100, 108), Random.Range(0, 20), diceRolls);
        }
    }

    /**
     * plantTypeId from 0 to 7
     * positionX from ??? to ???
     * diceRolls
     */
    public void PlantSeed(int plantTypeId, float positionX, DiceRolls diceRolls)
    {
        GameObject prefabPlant = plantPrefabs.Find(
            t => t.GetComponent<PlantType>().id == plantTypeId
        );

        if (prefabPlant == null)
        {
            Debug.LogError($"Did not find PlantType: {plantTypeId}");
            return;
        }

        PlantType plantType = prefabPlant.GetComponent<PlantType>();
        Plant plant = new Plant(plantType, positionX, diceRolls);

        GrowVines growVines = prefabPlant.GetComponent<GrowVines>();
        growVines.finalLength = plant.finalLength;
        growVines.timeToGrow = plant.growSpeed;

        Quaternion rotation = prefabPlant.transform.rotation * plant.rotation;
        Instantiate(prefabPlant, plant.position, rotation);

        //If you would want to write everything away for backup purposes writing away the Plant objects would do the trick. Then you would need something else to load them in again
    }
}
