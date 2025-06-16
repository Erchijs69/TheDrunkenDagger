using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    public Image buffIcon;         
    public Image buffFillImage;    
    public Image backgroundImage;  
    public float buffDuration;    
    private float timer;
    private bool isBuffActive;
    public Vector2 backgroundSize = new Vector2(150, 150); 


    void Start()
    {
       
        buffIcon.enabled = false;
        buffFillImage.enabled = false;
        backgroundImage.enabled = false;
    }

    public void ActivateBuff(Sprite icon, float duration)
{
    buffIcon.sprite = icon;
    backgroundImage.sprite = icon;

    buffIcon.enabled = true;
    buffFillImage.enabled = true;
    backgroundImage.enabled = true;

    SetBackgroundTransparency(0.4f);
    backgroundImage.rectTransform.sizeDelta = backgroundSize; 

    buffDuration = duration;
    timer = duration;
    isBuffActive = true;
    buffFillImage.fillAmount = 1f;
}




    void Update()
    {
        if (isBuffActive)
        {
            timer -= Time.deltaTime;
            buffFillImage.fillAmount = Mathf.Clamp01(timer / buffDuration);

            if (timer <= 0f)
            {
                isBuffActive = false;
                buffIcon.enabled = false;
                buffFillImage.enabled = false;
                backgroundImage.enabled = false;
            }
        }
    }

    private void SetBackgroundTransparency(float alpha)
    {
        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }
}





