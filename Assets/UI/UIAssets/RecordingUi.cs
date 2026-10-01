using UnityEngine;
using UnityEngine.UI;

public class RecordingUi : MonoBehaviour
{
    public string recordingTime = "00:00:00";

    [SerializeField]
    private Text recordingTimeText;

    private float recordingTimeInSeconds = 0f;

    void Update()
    {
        recordingTimeInSeconds += Time.deltaTime;
        recordingTime = FormatTime((int)recordingTimeInSeconds);

        if (recordingTimeText != null)
            recordingTimeText.text = recordingTime;
    }

    private string FormatTime(int timeInSeconds)
    {
        int hours = timeInSeconds / 3600;
        int minutes = (timeInSeconds % 3600) / 60;
        int seconds = timeInSeconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
