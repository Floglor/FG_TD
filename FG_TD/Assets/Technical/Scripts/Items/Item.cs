using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Shooting;
using UI;
using UnityEngine;


namespace Items
{
    
    
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 4)]
    [Serializable]
    public class Item : ScriptableObject
    {
        public bool isBasic; 
        public Sprite itemImage;
        public int tier;

        [Header("Buffs/Debuffs")]
        public List<IntegerTowerBuff> integerTowerBuffs;
        public List<IntegerTowerDebuff> integerTowerDebuffs;
        public List<FloatTowerBuff> floatTowerBuffs;
        public List<FloatTowerDebuff> floatTowerDebuffs;
        
        [Header("Effects")]
        public  List<FalseCrit> falseCrits;

        [Header("Recipe")]
        public  List<Item> combination;

        public int itemID { get; set; }

        public Item()
        {
            if (falseCrits.IsNullOrEmpty()) return;
            
            foreach (FalseCrit falseCrit in falseCrits)
            {
                falseCrit.itemID = GetInstanceID();
            }
        }
    }
}
