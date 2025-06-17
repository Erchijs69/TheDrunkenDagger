using UnityEngine;

public class Cart : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportTarget; 
    [SerializeField] private Material newSkyboxMaterial; 

    public void Interact()
    {
        Debug.Log("Interacting with the cart. Starting teleport...");
        FindObjectOfType<SoundManager>().SwitchToNightAmbience();

        if (PlayerMovement.Instance != null && ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeAndTeleport(PlayerMovement.Instance.transform, teleportTarget.position, () =>
            {
                if (newSkyboxMaterial != null)
                {
                    RenderSettings.skybox = newSkyboxMaterial;
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







