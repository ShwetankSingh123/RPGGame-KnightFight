

using UnityEngine;

namespace RPG.Core
{
    public class ActionSchedular : MonoBehaviour
    {
        IAction currentAction;
        public void StartAction(IAction action)
        {
            if(currentAction == action) { return; }
            if (currentAction != null)
            {
                currentAction.Cancel();
               // print("cancelling" + currentAction);
            }
            currentAction = action;
        }

        public void CancelCurrentAction() 
        { 
            StartAction(null); 
        }
    }
}
