using UnityEngine;

public class PlacementSpot : MonoBehaviour
{
    public enum SpotType { Bottle, Ingredient }
    public SpotType spotType;

    private void OnTriggerEnter(Collider other)
    {
        PotionSystem potionSystem = FindObjectOfType<PotionSystem>();

        if (spotType == SpotType.Bottle && other.CompareTag("Bottle"))
        {
            Debug.Log("Bottle entered bottle placement trigger.");
            potionSystem.PlaceBottle(other.gameObject);
        }
        else if (spotType == SpotType.Ingredient && other.CompareTag("Ingredient"))
        {
            Debug.Log($"Ingredient entered ingredient placement trigger: {other.name}");
            potionSystem.PlaceIngredient(other.gameObject);
        }
    }
}
