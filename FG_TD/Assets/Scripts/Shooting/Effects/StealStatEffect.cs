using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Shooting;
using UnityEngine;

[Serializable]
public class FarmableStatInt
{
    public Stats.IntStatName intStatName;
    public int statStep;
    public int statCounter { get; set; }
}

[Serializable]
public class FarmableStatFloat
{
    public Stats.FloatStatName floatStatName;
    public float statStep;
    public float statCounter { get; set; }

}

public class StealStatEffectObj : EffectObj
{
    public List<FarmableStatInt> farmableStatsInt;
    public List<FarmableStatFloat> farmableStatsFloat;
    public TowerAI tower;
    public int bossDamageForStat;
    public int damageCounter;

    public StealStatEffectObj(StealStatEffect statEffect, TowerAI towerAI)
    {
        bossDamageForStat = statEffect.bossDamageForStat;
        farmableStatsFloat = statEffect.farmableStatsFloat;
        farmableStatsInt = statEffect.farmableStatsInt;
        damageCounter = 0;
        tower = towerAI;
    }

    public void AddDamage(int dmg)
    {
        damageCounter += dmg;

        if (damageCounter < bossDamageForStat) return;
        
        damageCounter -= bossDamageForStat;
        ProcStatChange();
    }

    private void ProcStatChange()
    {
        foreach (FarmableStatFloat farmableStatFloat in farmableStatsFloat)
        {
            farmableStatFloat.statCounter += farmableStatFloat.statStep;
            tower.FloatStatRecalculate(farmableStatFloat.floatStatName);
        }
        
        foreach (FarmableStatInt farmableStatInt in farmableStatsInt)
        {
            farmableStatInt.statCounter += farmableStatInt.statStep;
            tower.IntegerStatRecalculate(farmableStatInt.intStatName);
        }
    }
}



[CreateAssetMenu(fileName = "Effect", menuName = "Effects/Stat Farm", order = 3)]
public class StealStatEffect : Effect
{
    public List<FarmableStatInt> farmableStatsInt;
    public List<FarmableStatFloat> farmableStatsFloat;
    public int bossDamageForStat;

    public StealStatEffect(bool isAffectingStats, bool appliesOnEnemy = false) : base(isAffectingStats, appliesOnEnemy)
    {
    }

    public override void ChangeStats(Projectile projectile)
    {
    }
}