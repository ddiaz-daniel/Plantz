using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsStorage : MonoBehaviour
{
    public List<PlantType> plantTypes { get; } = new List<PlantType>();
    public List<Plant> plants { get; } = new List<Plant>();
    public Mesh defaultPlantMesh { get; set; } = null;

    void Start()
    {
        //Data for debugging
        DiceRolls diceRolls = new DiceRolls(1, 4, 2, 3, 4, 5, 1);
        plantTypes.Add(new PlantType(1, "Daffodil", 100f, 160f, 2.5f, 1, defaultPlantMesh));
        PlantSeed(plantTypes[0], new Location(0f, 0f), diceRolls);



        //Check for plant growth every 10 seconds
        InvokeRepeating("GameTickPlantGrowth", 0f, 10f);
    }

    void GameTickPlantGrowth()
    {
        foreach (Plant plant in this.plants) {
            plant.UpdateGameTick();
        }
    }

    public void PlantSeed(PlantType plantType, Location location, DiceRolls diceRolls)
    {
        this.plants.Add(new Plant(plantType, location, diceRolls));
        Debug.Log("PLANTED SUCCESSFULLY; Total plants: " + plants.Count.ToString());
    }
}
