using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioClip explosionClip;
    public AudioSource soundManagerSource;

    public static bool isPaused = false;
    public static bool wallDestroyed = false;

    private bool canPlaySound = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                UnPauseGame();
            }
        }

        if (wallDestroyed && canPlaySound)
        {
            canPlaySound = false;
            soundManagerSource.PlayOneShot(explosionClip, 0.7f);
        }

    }

    public void PauseGame()
    {
        PlayerFPSController.current.enabled = false;
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnPauseGame()
    {
        PlayerFPSController.current.enabled = true;
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
