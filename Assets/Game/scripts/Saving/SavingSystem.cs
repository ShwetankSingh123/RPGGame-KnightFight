
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        [Header("Menu Screen")]
        [SerializeField] private GameObject loadingScreen;

        [Header("Slider")]
        [SerializeField] private Slider slider;


        private int buildIndex;


        public int GetBuildIndex()
        {
            return buildIndex;
        }


        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }


            //start
            
            if (buildIndex == 0)
            {
                buildIndex++;
            }
            //StartCoroutine(LoadSceneMenu(buildIndex));
            //end
            yield return buildIndex;
            //yield return new WaitForSeconds(1);
            StartCoroutine(RestoringState(state));
            
        }

        IEnumerator RestoringState(Dictionary<string, object> state)
        {
            yield return new WaitForSeconds(0);
            print(state);
            RestoreState(state);
        }

        //used to load the new scene
        IEnumerator LoadSceneMenu(int buildIndex) 
        {
            loadingScreen.SetActive(true);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
            print(buildIndex);
            while (!loadOperation.isDone)
            {
                float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
                print(progressValue);

                slider.value = progressValue;
                yield return null;
            }
            loadingScreen.SetActive(false);
            print("loading screen set to false");
        }

        

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            PlayerPrefs.SetInt("gameProgress", 1); //test
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}