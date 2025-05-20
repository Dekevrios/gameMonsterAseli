using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBoss : MonoBehaviour
{
    [Header("UI Elements")]
    public Text victoryText;
    public Text scoreText;
    public Button mainMenuButton;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    public float autoTransitionDelay = 5f; // Waktu dalam detik untuk transisi otomatis

    private float timer = 0f;
    private bool autoTransition = true;

    private void Start()
    {
        // Awalnya sembunyikan panel
        gameObject.SetActive(false);

        // Setup button event listeners
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

    }

    private void Update()
    {
        // Jika auto transition diaktifkan, hitung waktu
        if (autoTransition && gameObject.activeSelf)
        {
            timer += Time.deltaTime;

            if (timer >= autoTransitionDelay)
            {
                GoToMainMenu();
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
