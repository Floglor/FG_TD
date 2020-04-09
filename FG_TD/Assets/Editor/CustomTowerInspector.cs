using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerEffects))]
public class CustomTowerInspector : Editor
{
    
    public override void OnInspectorGUI()
    {
        TowerEffects towerEffects = target as TowerEffects;

        

        var list = towerEffects.effects;
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("size", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(null);

        for (int i = 0; i < list.Count; i++)
        {
           // list[i] = (Effect)EditorGUILayout.EnumPopup(selected: list[i].effectType);
        }


        for (int i = 0; i < towerEffects.effects.Count; i++)
        {
            switch(towerEffects.effects[i].effectType)
            {
                case Effects.LinearAOE:
                    towerEffects.waveDistance = EditorGUILayout.FloatField(0);
                    break;
            }
        }

    }

}
