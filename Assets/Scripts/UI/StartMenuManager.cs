using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public GameObject startMenuPanel;
    public IntroImageFade introFade;
    public Button startButton;
    public MouseLook mouseLook; 

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonPressed);
    }

    void OnStartButtonPressed()
{
    startMenuPanel.SetActive(false);
    introFade.PlayIntroFade(); 
}

}

