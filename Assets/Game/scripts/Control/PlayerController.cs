using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

using RPG.Movement;
using RPG.Attribute;


namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        // this script is responsible for controlling all the things like input controls 

       

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float rayCastRadius = 1f;
        

        private void Awake()
        {
            health = GetComponent<Health>();
        }


        void Update()
        {
            if (InteractWithUI()) { return; }
            if (health.IsDead()) 
            {
                SetCursor(CursorType.None);
                return; 
            }
            if (InteractWithComponent()) { return; }
            //if (InteractWithCombat()) { return; }
            if (InteractWithMovement()) { return; }
            //print("nothing is there");
            SetCursor(CursorType.None);

        }

        

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycstAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycstAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), rayCastRadius);

            float[] distance = new float[hits.Length];
            for(int i = 0; i < hits.Length; i++)
            {
                distance[i] = hits[i].distance;
            }
            Array.Sort(distance, hits);

            return hits;
        }

        //private bool InteractWithCombat()
        //{
        //    RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        //    foreach (RaycastHit hit in hits)
        //    {
        //        CombatTarget target = hit.transform.GetComponent<CombatTarget>();

        //        if(target == null) { continue; }

        //        if (!GetComponent<Fighter>().CanAttack(target.gameObject)) {  continue; }
        //        if (target == null) { continue; }

        //        if(Input.GetMouseButton(0))
        //        {
        //            GetComponent<Fighter>().Attack(target.gameObject);                   
        //        }
        //        SetCursor(CursorType.Combat);
        //        return true;
        //    }
        //    return false;
        //}

        

        private bool InteractWithMovement()
        {

            
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target))
                {
                    return false;
                }

                if(Input.GetMouseButton(0))
                    {
                        GetComponent<Mover>().StartMoveAction(target,1f); 
                    }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) { return false; }

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) { return false; }
            target = navMeshHit.position;

            return true;
        }

        

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

}