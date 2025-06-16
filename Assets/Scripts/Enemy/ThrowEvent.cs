using UnityEngine;

public class ScoutThrowEventRelay : MonoBehaviour
{
    public ScoutEnemy scoutEnemy;

    public void OnThrowMoment()
    {
        if (scoutEnemy != null)
            scoutEnemy.OnThrowMoment();
    }
}

