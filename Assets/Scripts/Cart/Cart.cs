using UnityEngine;

public class Cart : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private Material newSkyboxMaterial;

    public void Interact()
    {
        Debug.Log("Interacting with the cart. Starting teleport...");

        // Ensure player and fader exist
        if (PlayerMovement.Instance != null && ScreenFader.Instance != null)
        {
            // Use fade + teleport
            ScreenFader.Instance.FadeAndTeleport(PlayerMovement.Instance.transform, teleportTarget.position, () =>
            {
                // After teleport callback
                Debug.Log("Teleported via ScreenFader to: " + teleportTarget.position);
                FindObjectOfType<SoundManager>()?.SwitchToNightAmbience();

                if (newSkyboxMaterial != null)
                {
                    RenderSettings.skybox = newSkyboxMaterial;
                    EnvironmentLightingManager.Instance?.SetNightLighting();
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