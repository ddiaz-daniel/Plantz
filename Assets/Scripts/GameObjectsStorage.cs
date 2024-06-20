using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class GameObjectsStorage : MonoBehaviour
{
    public GameObject seedPrefab;
    public GameObject magicPlantEffectPrefab;

    public List<GameObject> plantPrefabs;
    private GetTagPosition tagPositionFetcher;
    private GetDiceRoll diceRollFetcher;
    private int UserSelectedPlantId;
    private float UserSelectedPositionX;
    private bool isPlantingDone = true;
    private bool isRollingTime = false;

    void Start()
    {
        InvokeRepeating("StartFetchAndUsePosition", 1.0f, 1.0f);
    }

    void StartFetchAndUsePosition()
    {
        StartCoroutine(FetchAndUsePosition());
    }

    private IEnumerator FetchAndUsePosition()
    {
        Debug.Log("isRollingTime: " + isRollingTime);
        Debug.Log("isPlantingDone: " + isPlantingDone);
        if (!isRollingTime)
        {
            Debug.Log("Place card on the table");
            tagPositionFetcher = new GetTagPosition(); // Reset fetcher

            // Start the position fetch coroutine
            Task task = tagPositionFetcher.GetPositionAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
                yield break;
            }

            // Now use the fetched positionX value
            if (tagPositionFetcher.PlantTypeId > 1)
            {
                UserSelectedPlantId = tagPositionFetcher.PlantTypeId;
                UserSelectedPositionX = tagPositionFetcher.PositionX;
                Debug.Log(UserSelectedPlantId);
                isPlantingDone = false;
            }
            else if (tagPositionFetcher.PlantTypeId == 1 && !isPlantingDone)
            {
                //Visualise seed being planted
                PlantSeedRoutine(new Vector3(UserSelectedPositionX, 0, 0));

                isRollingTime = true;
            }
        }
        else if (isRollingTime && !isPlantingDone)
        {
            Debug.Log("Rolling time");
            diceRollFetcher = new GetDiceRoll(); // Reset fetcher

            // Start the dice fetch coroutine
            Task diceTask = diceRollFetcher.GetDiceAsync();
            yield return new WaitUntil(() => diceTask.IsCompleted);

            if (diceTask.Exception != null)
            {
                Debug.LogError(diceTask.Exception);
                yield break;
            }

            // if there are not at least 6 dice rolls, the plant will not be planted
            if (diceRollFetcher.RedDiceCount + diceRollFetcher.PinkDiceCount + diceRollFetcher.GreenDiceCount + diceRollFetcher.BlueDiceCount +
                diceRollFetcher.DarkBlueDiceCount + diceRollFetcher.DarkGreenDiceCount + diceRollFetcher.YellowDiceCount < 6)
            {
                Debug.LogError("Not enough dice rolls");
                yield break;
            }
            else
            {
                Debug.Log("Planting with dice rolls");
                DiceRolls diceRolls = new DiceRolls(diceRollFetcher.RedDiceCount, diceRollFetcher.PinkDiceCount, diceRollFetcher.GreenDiceCount,
                    diceRollFetcher.BlueDiceCount, diceRollFetcher.DarkBlueDiceCount, diceRollFetcher.DarkGreenDiceCount, diceRollFetcher.YellowDiceCount);
                isPlantingDone = true;
                isRollingTime = false;
                PlantSeed(UserSelectedPlantId, UserSelectedPositionX, diceRolls);
            }
        }
    }

    public void PlantSeed(int plantTypeId, float positionX, DiceRolls diceRolls)
    {
        GameObject prefabPlant = plantPrefabs.Find(
            t => t.GetComponent<PlantType>().id == plantTypeId + 100
        );

        if (prefabPlant == null)
        {
            Debug.LogError($"Did not find PlantType: {plantTypeId}");
            return;
        }

        positionX = ClampPositionX(positionX, 1);
        PlantType plantType = prefabPlant.GetComponent<PlantType>();
        Plant plant = new Plant(plantType, positionX, diceRolls);

        GrowVines growVines = prefabPlant.GetComponent<GrowVines>();
        growVines.finalLength = plant.finalLength;
        growVines.timeToGrow = plant.growSpeed;

        Quaternion rotation = prefabPlant.transform.rotation * plant.rotation;

        //Visualise seed being planted
        MagicPlantingEffectRoutine(plant.position);

        //Create Plant
        Instantiate(prefabPlant, plant.position, rotation);

        //If you would want to write everything away for backup purposes writing away the Plant objects would do the trick. Then you would need something else to load them in again
    }

    float ClampPositionX(float positionX, int plantType)
    {
        float clampedPositionX = positionX;
        Debug.Log("The positionX is: " + positionX);
        // Adjust the clamping based on the plant type
        if (plantType == 1)
        {
            clampedPositionX = (positionX * 23f / 580f) - 11f;
        }
        else if (plantType == 2)
        {
            clampedPositionX = (positionX * 26f / 580f) - 13f;
        }
        else if (plantType == 3)
        {
            clampedPositionX = (positionX * 26f / 580f) - 13f;
        }

        return clampedPositionX;
    }

    /**
     * DEBUGGING
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MagicPlantingEffectRoutine(new Vector3(0, 0, 0));
            PlantSeedRoutine(new Vector3(0, 0, 0));
        }
    }

    private void MagicPlantingEffectRoutine(Vector3 position)
    {
        // Create a new GameObject to hold the PlantSeed script
        GameObject magicPlantingEffectManager = new GameObject("MagicPlantingEffectManager");

        // Add the PlantSeed script to the GameObject
        MagicPlantingEffect magicPlantingEffect = magicPlantingEffectManager.AddComponent<MagicPlantingEffect>();

        // Initialize the PlantSeed script to start the planting process
        magicPlantingEffect.Initialize(magicPlantEffectPrefab, position);
    }

    private void PlantSeedRoutine(Vector3 position)
    {
        // Create a new GameObject to hold the PlantSeed script
        GameObject seedManager = new GameObject("SeedManager");

        // Add the PlantSeed script to the GameObject
        PlantSeed plantSeed = seedManager.AddComponent<PlantSeed>();

        // Initialize the PlantSeed script to start the planting process
        plantSeed.Initialize(seedPrefab, position);
    }
}
