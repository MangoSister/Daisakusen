using UnityEngine;
using System.Collections;

public class SteeringMonitorTest : MonoBehaviour
{
    public GUIText debugText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GetComponent<SteeringMonitor>().ReCalibrate();

        if (debugText != null)
        {
            debugText.text = GetComponent<SteeringMonitor>().GetSteeringAxis().ToString();
        }
    }
}
