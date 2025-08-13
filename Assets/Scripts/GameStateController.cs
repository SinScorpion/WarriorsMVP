using UnityEngine;

public enum BattleState { Playing, Victory, Defeat }

public class GameStateController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject winPanel;
    public GameObject losePanel;
    public UnitSpawner unitSpawner;
    public EnemySpawner enemySpawner;

    public BattleState State { get; private set; } = BattleState.Playing;

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    void StopGame()
    {
        if (unitSpawner) unitSpawner.enabled = false;
        if (enemySpawner) enemySpawner.enabled = false;
        Time.timeScale = 0f;
    }

    public void OnPlayerBaseDestroyed()   // вызвать при смерти ЛЕВОЙ базы
    {
        if (State != BattleState.Playing) return;
        State = BattleState.Defeat;
        StopGame();
        if (losePanel) losePanel.SetActive(true);
    }

    public void OnEnemyBaseDestroyed()    // вызвать при смерти ПРАВОЙ базы
    {
        if (State != BattleState.Playing) return;
        State = BattleState.Victory;
        StopGame();
        if (winPanel) winPanel.SetActive(true);
    }
}
