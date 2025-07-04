using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public static SkyboxManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ChangeSkybox(Material newSkybox)
    {
        RenderSettings.skybox = newSkybox;
        DynamicGI.UpdateEnvironment();
    }
}

