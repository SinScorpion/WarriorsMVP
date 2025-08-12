using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonGuard : MonoBehaviour
{
    public enum UpgradeType { MeatPerSecond, BaseHP}

    public UpgradeType type;
    public Button button;
    public TextMeshProUGUI costText;

    private void Reset()
    {
        button = GetComponent<Button>();
        if (costText == null) costText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var gm = GameManager.Instance;
        if (button == null || gm == null)
        {
            if (button != null) button.interactable = false;
            return;
        }

        float cost = (type == UpgradeType.MeatPerSecond)
            ? gm.GetMeatUpgradeCost()
            : gm.GetBaseHpUpgradeCost();

        bool canAfford = gm.gold >= cost;
        button.interactable = canAfford;

        if (costText != null)
            costText.color = canAfford ? Color.white : Color.red;
    }
}
