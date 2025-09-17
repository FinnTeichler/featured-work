using FinnTeichler.TimeSystem;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    [SerializeField] private TimeValue testEndTime;
    [SerializeField] private GameObject shutdownMessage;

    public float TotalTimeElapsedMinutes { get { return totalTimeElapsed / 60; } }
    public float TotalTimeElapsedSeconds { get { return totalTimeElapsed; } }
    private float totalTimeElapsed = 0f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        totalTimeElapsed += Time.deltaTime;

        if (totalTimeElapsed >= testEndTime.GetValueIn(TimeTickSystem.TimeMeasurement.RealSecond))
        {
            transform.GetComponent<GamePause>().Pause();
            shutdownMessage.SetActive(true);
        }
    }
}
