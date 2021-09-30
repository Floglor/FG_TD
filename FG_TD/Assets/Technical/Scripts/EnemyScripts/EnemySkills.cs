using System.Collections.Generic;
using UnityEngine;

namespace Shooting
{
    [System.Serializable]
    public class EnemySkills : MonoBehaviour
    {
        public Enemy mother { get; set; }
        public List<EnemySkillEffect> effects;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
