using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int gold;
    public float meat;
    public int meatUpgradeLevel;
    public int hpUpgradeLevel;
    public bool[] unitUnlocked;
}

public static class SaveSystem
{
    private const string KEY = "WARRIORS_SAVE_V1";

    public static void SaveToPrefs(GameManager gm)
    {
        if (gm == null) return;

        SaveData data = new SaveData();
        data.gold = gm.gold;
        //data.meat = gm.meat;
        data.meatUpgradeLevel = gm.meatUpgradeLevel;
        data.hpUpgradeLevel = gm.hpUpgradeLevel;

        //Сохраняем разблокировку карточек юнитов
        UnitSpawner spawner = Object.FindObjectOfType<UnitSpawner>();
        if (spawner !=null && spawner.units != null)
        {
            int n = spawner.units.Length;
            data.unitUnlocked = new bool[n];
            for (int i = 0; i < n; i++)
            {
                data.unitUnlocked[i] = spawner.units[i].unlocked;
            }
        }
        else
        {
            data.unitUnlocked = new bool[0];
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        Debug.Log("[SaveSystem] Saved: " + json);
#endif
    }

    public static void LoadFromPrefs(GameManager gm)
    {
        if (gm == null) return;
        if (!PlayerPrefs.HasKey(KEY)) return;

        string json = PlayerPrefs.GetString(KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null) return;

       

        gm.gold = data.gold;
        gm.meatUpgradeLevel = data.meatUpgradeLevel;
        gm.hpUpgradeLevel = data.hpUpgradeLevel;

        // <-- тут меняем формулу подсчёта мясо/сек
        float baseMeatPerSecond = gm.meatPerSecond; // базовое значение из инспектора
        gm.meatPerSecond = baseMeatPerSecond + gm.meatUpgradeLevel * gm.meatUpgradeStep;

        
        float basePlayerMaxHp = gm.playerBase != null ? gm.playerBase.maxHP : 100f;
        if (gm.playerBase != null)
        {
            gm.playerBase.maxHP = basePlayerMaxHp + gm.hpUpgradeLevel * gm.hpUpgradeStep;
            gm.playerBase.currentHP = gm.playerBase.maxHP;
            if (gm.playerBase.hpBar != null)
                gm.playerBase.hpBar.SetHP(gm.playerBase.currentHP, gm.playerBase.maxHP);
        }

        // Применяем разблокировки карточек
        UnitSpawner spawner = Object.FindObjectOfType<UnitSpawner>();
        if (spawner !=null && spawner.units !=null && data.unitUnlocked !=null)
        {
            int n = Mathf.Min(spawner.units.Length, data.unitUnlocked.Length);
            for (int i = 0; i < n; i++)
                spawner.units[i].unlocked = data.unitUnlocked[i];
        }

        //Обновляем UI
        gm.UpdateGoldUI();
        gm.UpdateMeatUpgradeUI();
        gm.UpdateBaseHpUpgradeUI();

#if UNITY_EDITOR
        Debug.Log("[SaveSystem] Loaded: " + json);
#endif
    }


}


