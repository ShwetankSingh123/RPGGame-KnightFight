using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;

    static bool hasSpawned = false;

    private void Update()
    {
        if (hasSpawned)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuScreen.SetActive(false);
                hasSpawned = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {    
                menuScreen.SetActive(true);
                hasSpawned = true;
            }
        }

        
    }

    public void Continue()
    {
        menuScreen.SetActive(false);
    }

    public void Option()
    {

    }

    public void MainMenu()
    {
        PlayerPrefs.SetString("SceneToLoad", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        print(PlayerPrefs.GetString("SceneToLoad"));
        menuScreen.SetActive(false);
        SceneManager.LoadSceneAsync(0);
    }




}
