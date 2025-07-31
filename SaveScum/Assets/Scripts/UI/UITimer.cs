using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    private void Update()
    {
        timerText.text = GameManager.Instance.GetTimeLeft().ToString("0.00");
    }
}
