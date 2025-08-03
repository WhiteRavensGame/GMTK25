using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private void Update()
    {
        float timeLeft = GameManager.Instance.GetTimeLeft();

        timerText.text = timeLeft.ToString("0.00");
        if (timeLeft > 600)
            canvasGroup.alpha = 0;
        else
            canvasGroup.alpha = 1;
    }
}
