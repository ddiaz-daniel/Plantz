using UnityEngine;

public class MagicPlantingEffect : MonoBehaviour
{
    private GameObject magicEffectInstance;
    private float effectTimer;
    private bool isEffectActive;
    private float effectDuration = 2f;

    public void Initialize(GameObject magicPlantingEffectPrefab, Vector3 targetPosition)
    {
        // Instantiate the magic planting effect prefab at the target position
        magicEffectInstance = Instantiate(magicPlantingEffectPrefab, targetPosition, Quaternion.identity);
        isEffectActive = true;
        effectTimer = 0f;
    }

    void Update()
    {
        if (isEffectActive)
        {
            effectTimer += Time.deltaTime;

            // If the effect has been active for 2 seconds, delete it
            if (effectTimer >= effectDuration)
            {
                Destroy(magicEffectInstance);
                isEffectActive = false;

                // Optionally, destroy the entire MagicPlantingEffect game object if needed
                Destroy(gameObject);
            }
        }
    }
}