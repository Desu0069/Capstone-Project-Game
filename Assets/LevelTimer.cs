using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float elapsedTime = 0f;
    public bool isTiming = true;

    void Update()
    {
        if (isTiming)
            elapsedTime += Time.deltaTime;
    }

    public void StopTimer()
    {
        isTiming = false;
    }
}