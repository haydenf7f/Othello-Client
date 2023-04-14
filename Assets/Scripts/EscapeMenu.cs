using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{

    [SerializeField] GameObject escapeMenu;

    public static EscapeMenu instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("More than one EscapeMenu instance found!");
            Destroy(gameObject);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        escapeMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        escapeMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        Client.instance.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Client.instance.Disconnect();
        Application.Quit();
    }
}
