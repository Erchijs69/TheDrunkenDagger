using UnityEngine;

public class Cart : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportTarget; // teleport destination
    [SerializeField] private Material newSkyboxMaterial; // assign the new skybox here

    public void Interact()
    {
        Debug.Log("Interacting with the cart. Starting teleport...");

        if (PlayerMovement.Instance != null && ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeAndTeleport(PlayerMovement.Instance.transform, teleportTarget.position, () =>
            {
                if (newSkyboxMaterial != null)
                {
                    RenderSettings.skybox = newSkyboxMaterial;
                    // Update environment lighting if needed
                    if (EnvironmentLightingManager.Instance != null)
                    {
                        EnvironmentLightingManager.Instance.SetNightLighting();
                    }
                }
                else
                {
                    Debug.LogWarning("No new skybox material assigned!");
                }
            });
        }
        else
        {
            Debug.LogWarning("PlayerMovement.Instance or ScreenFader.Instance is null, cannot teleport.");
        }
    }
}







