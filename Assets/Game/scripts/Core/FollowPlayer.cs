
using UnityEngine;

namespace RPG.Core
{
    public class FollowPlayer : MonoBehaviour
    {
        [SerializeField] Transform target;



        void LateUpdate()
        {
            this.transform.position = target.position;
        }
    }
}
