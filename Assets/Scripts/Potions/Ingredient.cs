using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public Color ingredientColor;
    public string ingredientType;

    [HideInInspector]
    public bool isPlaced = false;
}
