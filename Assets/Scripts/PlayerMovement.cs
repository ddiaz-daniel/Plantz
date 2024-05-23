using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private float playerSpeed = 2.0f;
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
        
        //Action
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Current Position: {transform.position}");
            Location playerLocation = new Location(transform.position.x, transform.position.y);

            this.gameObjectsStorage.PlantSeed(plantType, playerLocation, new DiceRolls());
        }
    }
}
