using UnityEngine;
using UnityEngine.Playables;

using RPG.Control;
using RPG.Core;


namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        private void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionSchedular>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;

        }

        private void EnableControl(PlayableDirector pd)
        {
            //check later
            player.GetComponent<PlayerController>().enabled = true;
        }


    }

}