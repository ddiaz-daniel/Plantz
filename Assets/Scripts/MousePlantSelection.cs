using UnityEngine;
using UnityEngine.UI;

public class MousePlantSelection : MonoBehaviour
{
    public Button selectPlantButton;
    public GameObject selectionPanel;
    public int seedId;
    public GameObjectsStorage gameObjectsStorage;
    public GameObject magicCircleEffectPrefab;

    private GameObject magicCircle;

    private bool isPlanting = false;

    void Start()
    {
        selectionPanel.SetActive(true);

        selectPlantButton.onClick.AddListener(OnSelectPlantButtonClick);
    }

    void Update()
    {
        if (isPlanting && Input.GetMouseButtonDown(0))
        {
            if (magicCircle != null)
            {
                Destroy(magicCircle);
                magicCircle = null;
            }
            PlantSeedAtMousePosition();
        }

        if (isPlanting)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Instantiate or move the magic circle
            if (magicCircle == null)
            {
                magicCircle = Instantiate(magicCircleEffectPrefab, new Vector3(worldPosition.x * 150f, 0f, 10f), Quaternion.identity);
                magicCircle.tag = "MagicCircle";
            }
            else
            {
                magicCircle.transform.position = new Vector3(worldPosition.x * 150f, 0, 10f);
            }

        }
    }

    void OnSelectPlantButtonClick()
    {
        selectionPanel.SetActive(false);
        isPlanting = true;
    }

    void PlantSeedAtMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;

        float positionX = worldPosition.x * 150f;

        gameObjectsStorage.PlantSeedOffline(seedId, positionX);

        isPlanting = false;

    }
}
