using System.Collections;
using System.Collections.Generic;
using Shooting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/Summon", order = 5)]
public class EnemySummonSkill : EnemySkillEffect
{
    public float counter;
    public Transform childType;
    public int childCount;
  
    public override float propCounter => counter;

    public override void DoEffect(Enemy self)
    {
        if (childType == null || childCount == 0) return;


        for (int i = 0; i < childCount; i++)
        {

            Vector3 parentPosition = self.transform.position;

            Vector3 position = new Vector3();

            float newX = parentPosition.x + Random.Range(-0.3f, 0.3f);
            float newY = parentPosition.y + Random.Range(-0.3f, 0.3f);

            position.x = newX;
            position.y = newY;
            position.z = 0;

            Enemy child = Instantiate(childType, position, Quaternion.identity).GetComponent<Enemy>();

            child.target = self.target;
            child.waypointIndex = self.waypointIndex;

            PlayerStats.enemiesAlive++;
        }
    }
    
}
