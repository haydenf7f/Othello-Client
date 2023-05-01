using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{

    [SerializeField] GameObject escapeMenu;

    [SerializeField] GameObject playerDisconnectedMenu;

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
        escapeMenu.SetActive(true);
    }

    public void Resume()
    {
        escapeMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Client.instance.Disconnect();
        GameManager.instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerDisconnectedPopup()
    {
        playerDisconnectedMenu.SetActive(true);
    }

    public void Quit()
    {
        Client.instance.Disconnect();
        Application.Quit();
    }
}
