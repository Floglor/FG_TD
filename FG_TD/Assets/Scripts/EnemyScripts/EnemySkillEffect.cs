using System;
using UnityEngine;

namespace Shooting
{
    [Serializable]
    public abstract class EnemySkillEffect : ScriptableObject
    {
        public abstract float propCounter { get; }
        public int buffID { get; set; }
        public float trueCounter { get; set; }
        public abstract void DoEffect(Enemy self);
    }
}