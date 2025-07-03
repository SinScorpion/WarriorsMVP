using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    [Header("Economy")]
    public float meat = 0f;
    public float maxMeat = 1000f;
    public float meatPerSecond = 1f;
    public int gold = 0; // Текущее золото игрока

    [Header("UI")]
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI goldText;

    [Header("Upgrade Meat Per Second")]
    public float baseMeatUpgradeCost = 10f; //Базовая цена улучшения
    public float meatUpgradeCostMultiplier = 1.15f;
    public int meatUpgradeLevel = 0;
    public float meatUpgradeStep = 0.5f;
    public TextMeshProUGUI meatUpgradeCostText;
    public TextMeshProUGUI meatUpgradeLevelText;

    [Header("Upgrade Base HP")]
    public float baseHpUpgradeCost = 10f; //Базовая цена улучшения базы
    public float hpUpgradeCostMultiplier = 1.15f;
    public int hpUpgradeLevel = 0;
    public int hpUpgradeStep = 1;
    public TextMeshProUGUI hpUpgradeCostText;
    public TextMeshProUGUI hpUpgradeLevelText;
    public Base playerBase;



    private void Awake()
    {
        Instance = this;
        UpdateMeatUpgradeUI();
        UpdateBaseHpUpgradeUI();
    }


    void Update()
    {
        // Копим мясо постепенно
        if (meat<maxMeat)
        {
            meat += meatPerSecond * Time.deltaTime;
            if (meat > maxMeat)
            {
                meat = maxMeat;
            }
        }

        // Обновляем UI
        if (meatText != null)
        {
            meatText.text = Mathf.FloorToInt(meat).ToString();
        }
    }

    // Метод для траты мяса
    public bool SpendMeat(float amount)
    {
        if (meat >= amount)
        {
            meat -= amount;
            return true;
        }
        return false;
    }

    // Метод увеличения золота
    public void AddGold(int amount)
    {
        gold += amount;
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    // Метод вычисления цены улучшения мяса
    public float GetMeatUpgradeCost()
    {
        return Mathf.Floor(baseMeatUpgradeCost * Mathf.Pow(meatUpgradeCostMultiplier, meatUpgradeLevel));
    }

    // Метод вычисления цены улучшения базы
    public float GetBaseHpUpgradeCost()
    {
        return Mathf.Floor(baseHpUpgradeCost * Mathf.Pow(hpUpgradeCostMultiplier, hpUpgradeLevel));
    }

    public void UpgradeMeatPerSecond()
    {
        float cost = GetMeatUpgradeCost();
        if (gold >= cost)
        {
            gold -= (int)cost;
            meatUpgradeLevel++;

            meatPerSecond += meatUpgradeStep;
            UpdateMeatUpgradeUI();
            if (goldText !=null)
            {
                goldText.text = gold.ToString();
            }
        }
    }

    public void UpgradeBaseHp()
    {
        float cost = GetBaseHpUpgradeCost();
        if (gold >= cost)
        {
            gold -= (int)cost;
            hpUpgradeLevel++;

            if (playerBase !=null)
            {
                playerBase.maxHP += hpUpgradeStep;
                playerBase.currentHP = playerBase.maxHP; // Лечим базу до фулла после апгрейда
            }
            
            UpdateBaseHpUpgradeUI();

            if (goldText != null)
            {
                goldText.text = gold.ToString();
            }
        }
    }

    public void UpdateMeatUpgradeUI()
    {
        if (meatUpgradeCostText != null)
        {
            meatUpgradeCostText.text = GetMeatUpgradeCost().ToString();
        }
        if (meatUpgradeLevelText !=null)
        {
            meatUpgradeLevelText.text = meatUpgradeLevel.ToString();
        }
    }

    public void UpdateBaseHpUpgradeUI()
    {
        if (hpUpgradeCostText != null)
        {
            hpUpgradeCostText.text = GetBaseHpUpgradeCost().ToString();
        }
        if (hpUpgradeLevelText != null)
        {
            hpUpgradeLevelText.text = hpUpgradeLevel.ToString();
        }
    }

}
