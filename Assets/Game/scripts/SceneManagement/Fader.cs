using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActivefade;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
            
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentActivefade != null)
            {
                StopCoroutine(currentActivefade);
            }
            currentActivefade = StartCoroutine(FadeRoutine(target, time));
            return currentActivefade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

    }
}
