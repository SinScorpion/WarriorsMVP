using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string battleSceneName = "SampleScene";
    [SerializeField] string homeSceneName = "Home";

    public void LoadBattle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(battleSceneName);
    }

    public void LoadHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeSceneName);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
