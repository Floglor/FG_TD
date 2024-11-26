using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 5)]
public class Recipe : ScriptableObject
{
    public List<Item> combination;

    public Item result;

    public Recipe(List<Item> combination, Item result)
    {
        this.combination = combination;
        this.result = result;
    }
}
