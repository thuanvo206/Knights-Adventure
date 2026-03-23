using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;

public class GameManager : MonoBehaviour
{
    Player localPlayer;
    public Slider healthBar;
    public GameObject pauseGame;
    public GameObject endingGame;

    void Update()
    {
        // Tự động tìm Local Player nếu bị mất tham chiếu
        if (localPlayer == null)
        {
            NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
            if (runner != null && runner.IsRunning)
            {
                var obj = runner.GetPlayerObject(runner.LocalPlayer);
                if (obj != null) localPlayer = obj.GetComponent<Player>();
            }
            return;
        }

        if (localPlayer.isDead)
        {
            Invoke("RestartGame", 1.0f);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        healthBar.maxValue = localPlayer.maxPlayerHealth;
        healthBar.value = localPlayer.currentPlayerHealth;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}