using UnityEngine;

public class EnvironmentLightingManager : MonoBehaviour
{
    public static EnvironmentLightingManager Instance;

    [SerializeField] private Light directionalLight; // Assign your main directional light here

    [Header("Lighting Profiles")]
    [SerializeField] private LightingSettings daySettings = new LightingSettings()
    {
        ambientLight = Color.white,
        fogColor = Color.gray,
        fogDensity = 0.01f,
        directionalLightColor = Color.white,
        directionalLightIntensity = 1f
    };

    [SerializeField] private LightingSettings nightSettings = new LightingSettings()
    {
        ambientLight = new Color(0.05f, 0.05f, 0.1f),
        fogColor = new Color(0.02f, 0.02f, 0.05f),
        fogDensity = 0.05f,
        directionalLightColor = new Color(0.1f, 0.1f, 0.3f),
        directionalLightIntensity = 0.3f
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyLightingSettings(LightingSettings settings)
    {
        if (settings == null) return;

        RenderSettings.ambientLight = settings.ambientLight;
        RenderSettings.fogColor = settings.fogColor;
        RenderSettings.fogDensity = settings.fogDensity;

        if (directionalLight != null)
        {
            directionalLight.color = settings.directionalLightColor;
            directionalLight.intensity = settings.directionalLightIntensity;
        }
    }

    public void SetDayLighting()
    {
        ApplyLightingSettings(daySettings);
    }

    public void SetNightLighting()
    {
        ApplyLightingSettings(nightSettings);
    }
}




