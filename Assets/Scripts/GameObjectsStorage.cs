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
        plantTypes.Add(new PlantType(1, "Daffodil", 100f, 160f, 2.5f, 1, defaultPlantMesh));
        plants.Add(new Plant(plantTypes[0], new Location(0f, 0f)));

        //Check for plant growth every 3 seconds
        InvokeRepeating("GameTickPlantGrowth", 0f, 3f);
    }

    void GameTickPlantGrowth()
    {
        foreach (Plant plant in this.plants) {
            plant.UpdateGameTick();
        }
    }

    public void PlantSeed(PlantType plantType, Location location)
    {
        this.plants.Add(new Plant(plantType, location));
        Debug.Log("PLANTED SUCCESSFULLY; Total plants: " + plants.Count.ToString());
    }

    public void GiveWater(Location location)
    {
        // Check for all plants if they get affected
        foreach (Plant plant in plants)
        {
            if (plant.CheckInFacinity(location))
            {
                plant.GiveWater();
                Debug.Log("WATERED SUCCESSFULLY: " + plant.Id.ToString() + " MAX LENGTH: " + plant.maxLength.ToString());
            }
        }
    }

    public void GiveFertilization(Location location)
    {
        // Check for all plants if they get affected
        foreach (Plant plant in plants)
        {
            if (plant.CheckInFacinity(location))
            {
                plant.GiveFertilization();
                Debug.Log("COMPOSTED SUCCESSFULLY: " + plant.Id.ToString() + " GROW CHANCE: " + plant.growChance.ToString());
            }
        }
    }

    public void Dig(Location location)
    {
        // Check for all plants if they get affected
        foreach (Plant plant in plants)
        {
            if (plant.CheckInFacinity(location))
            {
                plants.Remove(plant);
                Debug.Log("DESTROYED PLANT: " + plant.Id.ToString());
            }
        }
    }
}
