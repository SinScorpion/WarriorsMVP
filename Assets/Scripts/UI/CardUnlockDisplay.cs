using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUnlockDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject meatPanel;
    public GameObject goldPanel;
    public Image unitImage;
    public Button button;

    [Header("Cost")]
    public TextMeshProUGUI goldCostText;

    [Header("Index & Unlocks")]
    public UnitSpawner unitSpawner;
    public int unitIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        var data = unitSpawner.units[unitIndex];
        bool isUnlocked = data.unlocked;
        int unlockGoldCost = data.unlockGoldCost;

        if (isUnlocked)
        {
            meatPanel.SetActive(true);
            goldPanel.SetActive(false);

            unitImage.color = Color.white;

            return;

        }
        else
        {
            meatPanel.SetActive(false);
            goldPanel.SetActive(true);

            unitImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            goldCostText.text = unlockGoldCost.ToString();

            // ≈сли достаточно золота Ч делаем €рче
            if (GameManager.Instance.gold >= unlockGoldCost)
            {
                goldCostText.color = Color.white;
                goldPanel.GetComponent<Image>().color = new Color(1f, 0.8f, 0.2f, 1f); //золотистый фон
                button.interactable = true;
            }
            else
            {
                goldCostText.color = Color.red;
                goldPanel.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f); //тусклый фон
                button.interactable = false;
            }
        }
    }

    public void OnUnlockButtonClick()
    {
        var data = unitSpawner.units[unitIndex];
        int unlockGoldCost = data.unlockGoldCost;

        if (data.unlocked) return;

        if (GameManager.Instance.gold >= unlockGoldCost)
        {
            GameManager.Instance.gold -= unlockGoldCost;

            GameManager.Instance.UpdateGoldUI();

            data.unlocked = true;

            UpdateDisplay();

            var spawnBtn = GetComponent<SpawnUnitButton>();
            if (spawnBtn !=null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(spawnBtn.OnClick);
            }
        }
    }
}
