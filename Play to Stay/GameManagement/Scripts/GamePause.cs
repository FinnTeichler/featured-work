using UnityEngine;
using UnityEngine.Events;

public class GamePause : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    private float timeScalePrevious = 1f;

    public UnityEvent OnPause;
    public UnityEvent OnResume;

    
    public bool IsPaused {  get { return isPaused; } }
    private bool isPaused;

    public void Pause()
    {
        isPaused = true;
        timeScalePrevious = Time.timeScale;
        SetTimeScale(0f);

        OnPause.Invoke();

        if (debugMode)
            Debug.Log("Paused Game");
    }

    public void Resume()
    {
        isPaused = false;
        SetTimeScale(1f);

        OnResume.Invoke();

        if (debugMode)
            Debug.Log("Resumed Game");
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}