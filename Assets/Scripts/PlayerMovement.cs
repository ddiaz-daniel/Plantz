using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private float playerSpeed = 2.0f;
    private int toolIndex = 0; // 0 = planting, 1 = watering, 2 = fertilize, 3 = digging
    private string[] tools = { "PLANTING", "WATERING", "FERTILIZE", "DIGGING" };
    private int maxToolIndex = 3;
    public GameObjectsStorage gameObjectsStorage;
    public PlantType plantType { get; set; }

    void Start()
    {
        plantType = this.gameObjectsStorage.plantTypes[0];
    }

    void Update()
    {
        //Move
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        transform.Translate(move * Time.deltaTime * playerSpeed);

        //Change Action Index
        if (Input.GetKeyDown(KeyCode.X))
        {
            toolIndex++;
            if (toolIndex > maxToolIndex)
            {
                toolIndex = 0;
            }
            Debug.Log($"Index: {tools[toolIndex]}");
        }
        
        //Action
        if (Input.GetKeyDown(KeyCode.E))
        {
            //TODO - Add a cooldown for all the actions

            Debug.Log($"Current Position: {transform.position}");
            Location playerLocation = new Location(transform.position.x, transform.position.y);

            switch (toolIndex)
            {
                //Plant
                case 0:
                    this.gameObjectsStorage.PlantSeed(plantType, playerLocation);
                    break;
                
                //Water
                case 1:
                    this.gameObjectsStorage.GiveWater(playerLocation);
                    break;

                //Fertilize
                case 2:
                    this.gameObjectsStorage.GiveFertilization(playerLocation);
                    break;

                //Dig
                case 3:
                    this.gameObjectsStorage.Dig(playerLocation);
                    break;

                default: break;
            }
        }
    }
}
