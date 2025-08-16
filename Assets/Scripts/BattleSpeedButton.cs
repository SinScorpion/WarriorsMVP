using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleSpeedButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] float[] speeds = new float[] { 1f, 2f, 3f };
    int i = 0;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (button) button.onClick.AddListener(Cycle);
        Apply();
    }

    void Cycle()
    {
        i = (i + 1) % speeds.Length;
        Apply();
    }

    void Apply()
    {
        Time.timeScale = speeds[i];
        if (label) label.text = speeds[i].ToString("0") + "x";
    }
}
