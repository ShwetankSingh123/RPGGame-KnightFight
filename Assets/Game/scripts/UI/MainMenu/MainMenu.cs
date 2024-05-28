using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

namespace RPG.MainMen
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Screen")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private GameObject parentCanvas;

        [Header("Slider")]
        [SerializeField] private Slider slider;

        private const string ProgressKey = "gameProgress";
        private string sceneToLoad = "SceneToLoad";
        public Button newGameButton;
        public Button continueButton;
        SavingWrapper savingWrapper;

        //static bool hasSpawned = false;

        public CinemachineVirtualCamera currentCamera;




        private void Awake()
        {
            //if (hasSpawned) { return; }

            //SpawnPersistentObjects();
            //hasSpawned = true;
        }

        


        private void Start()
        {
            currentCamera.Priority++;
            // Check if there is saved progress
            if (PlayerPrefs.HasKey(ProgressKey))
            {
                ShowContinueOption();
            }
            else
            {
                ShowNewGameOption();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerPrefs.SetString(sceneToLoad, SceneManager.GetActiveScene().name);
                PlayerPrefs.Save();
                menuScreen.SetActive(true);
            }
        }

        public void Play(string sceneToLoad)
        {
            loadingScreen.SetActive(true);
            menuScreen.SetActive(false);

            StartCoroutine(LoadLevelAsync(sceneToLoad));

        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Option()
        {

        }

        IEnumerator LoadLevelAsync(string sceneToLoad)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

            while (!loadOperation.isDone)
            {
                float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
                slider.value = progressValue;
                yield return null;
            }
            loadingScreen.SetActive(false);
        }


        void ShowNewGameOption()
        {
            // Show only New Game button
            newGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }

        void ShowContinueOption()
        {
            // Show both New Game and Continue buttons
            newGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }

        public void OnNewGameButton()
        {
            // Start a new game and reset progress
            PlayerPrefs.DeleteKey(ProgressKey);
            StartNewGame();
        }

        public void OnContinueButton()
        {
            // Continue the game from saved progress
            ContinueGame();
        }

        void StartNewGame()
        {
            
            //savingWrapper.Delete();
            Play("Scene01");
            File.Delete(Path.Combine(Application.persistentDataPath, "save" + ".sav"));  //this code is taken from SavingSystem.cs
            PlayerPrefs.SetInt(ProgressKey, 1);

        }

        void ContinueGame()
        {
            // Load the saved game state and continue
            //int savedLevel = PlayerPrefs.GetInt(ProgressKey);
            //SceneManager.LoadScene(savedLevel); // Replace with your level loading logic
            //Play(PlayerPrefs.GetString("SceneToLoad"));

            //savingWrapper.donebaby = true;
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            StartCoroutine(savingWrapper.LoadLastScene());

        }

        public void UpdateCamera(CinemachineVirtualCamera target)
        {
            currentCamera.Priority--;
            currentCamera = target;
            currentCamera.Priority++;
        }
        
    }
}
