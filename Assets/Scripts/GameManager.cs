using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioClip explosionClip;
    public AudioSource soundManagerSource;
    public GameObject gameOverTextOBJ;
    public TextMeshProUGUI gameOverText;

    public static bool isPaused = false;
    public static bool wallDestroyed = false;
    public static bool isRestarting = false;

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
            gameOverTextOBJ.SetActive(true);          
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
       Application.Quit();  
    }

    public void RestartGame()
    {
        
        isRestarting = true;
        isPaused = false;
        wallDestroyed = false;
       
        gameOverTextOBJ.SetActive(false);
        WallCount wallCount = FindObjectOfType<WallCount>();
        wallCount.wallCounts = 40;
        StartCoroutine(restartCountDown());
        UnPauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //isRestarting = false;
    }


    IEnumerator restartCountDown()
    {
        yield return new WaitForSeconds(3);
        isRestarting = false;
    }

}
