using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class OutlineEffect : MonoBehaviour
{
    public Material outlineMaterial;
    private Material[] originalMaterials;

    void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        originalMaterials = meshRenderer.materials;
    }

    public void EnableOutline()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        var newMats = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(newMats, 0);
        newMats[originalMaterials.Length] = outlineMaterial;
        meshRenderer.materials = newMats;
    }

    public void DisableOutline()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = originalMaterials;
    }
}

