
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;


namespace RPG.Attribute
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Experience experience;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }
        private void Update()
        {
            float healthOfPlayer = health.GetHealthPoints() / health.GetMaxHealthPoints();
            if(healthOfPlayer <= 0)
            {
                print("score : "+experience.GetPoints());
            }
            //GetComponent<Text>().text = string.Format("{0:0}%",health.GetPercentage());
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}",health.GetHealthPoints(),health.GetMaxHealthPoints());
        }
    }
}
