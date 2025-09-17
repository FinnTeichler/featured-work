using SpiritGarden;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [HideInInspector] public UIReference refUI;

    [SerializeField] private TimeMeasurement timeMeasurement;

    [SerializeField] private TMP_Text tmp;

    private void Update()
    {
        if (refUI)
        {
            tmp.text = $"{Mathf.Round(refUI.refGame.TimeMeasurement.TotalTimeElapsedMinutes)}";
        }
        else
        {
            tmp.text = $"{Mathf.Round(timeMeasurement.TotalTimeElapsedMinutes)}";
        }
    }
}