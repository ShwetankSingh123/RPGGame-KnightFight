
using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using RPG.Saving;
using RPG.Attribute;



namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40;


        NavMeshAgent agent;
        Health health;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            agent.enabled = !health.IsDead();
            UpdateAnimator();
        }


        

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            
            MoveTo(destination,speedFraction);

        }


        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath(); //path can never be null because it is of refrence type
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
            
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }



        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            //print(total);
            return total;
        }

        private void UpdateAnimator()
        {
            Vector3 globalVal = agent.velocity;
            Vector3 localVal = transform.InverseTransformDirection(globalVal);
            float speed = localVal.z;
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

}