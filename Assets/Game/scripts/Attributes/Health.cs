
using UnityEngine;
using UnityEngine.Events;

using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System;

namespace RPG.Attribute
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        //edit
        public event Action isdead;
        public event Action healthReduced;


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        LazyValue<float> healthPoints;
        bool isDead = false;


        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {

            healthPoints.ForceInit();
            
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }
        public void TakeDamage(GameObject instigator,float damage)
        {
            //print(gameObject.name + " took damage : " + damage);
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);



            //TODO 
            healthReduced?.Invoke();


            if (healthPoints.value == 0)
            {
                AwardExperience(instigator);//put this after die
                onDie.Invoke();
                Die();

            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float HealthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + HealthToRestore, GetMaxHealthPoints());
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100* (GetFraction());
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public bool IsDead()
        {
            return isDead;
        }

        private void Die()
        {
            if(isDead)
            {
                return;
            }
            isDead = true;

            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionSchedular>().CancelCurrentAction();
            isdead?. Invoke();
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if(healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}
