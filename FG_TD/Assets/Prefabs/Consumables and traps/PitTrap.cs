using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Prefaps.Consumables_and_traps
{
    public class PitTrap : Consumable
    {

        public static readonly string MyTag = "PitTrap";
        public GameObject pitGameObject;
        


        public PitTrap(int quantity, int cost) : base(quantity, cost)
        {

        }

        public override void TakeEffect(GameObject rail, Vector2 clickCoordinates)
        {
            if (rail == null) return;

            bool moneySpent = false;
            bool essencesSpent = false;
            
            if (!PlayerStats.instance.SpendEssences(cost))
                if (!PlayerStats.instance.SpendMoney(cost))
                    return;
                else
                {
                    moneySpent = true;
                }
            else
            {
                essencesSpent = true;
            }

            //Debug.Log($"rail is null: {rail == null}");
            
            Rail railScript = rail.GetComponent<Rail>();
            
            //Debug.Log($"railScript is null: {railScript == null}");
            

            GameObject newPitGameObject = null;

            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(clickCoordinates, PlayerStats.NodeWidth*1.85f);

            Transform waypoint = null;

            List<Transform> traps = new List<Transform>();
            
            foreach (Collider2D collider2D1 in colliders2D)
            {
                if (collider2D1.CompareTag(WaypointGizmos.MyTag))
                {
                    waypoint = collider2D1.transform;
                }
                
                if (!collider2D1.CompareTag(MyTag)) continue;

                traps.Add(collider2D1.transform);
            }
            
            //Debug.Log(traps.Count);
            
            if (!traps.IsNullOrEmpty())
            {
                if (traps.Count > 2)
                {
                    PlayerStats.instance.CancelConsumable();
                    ReturnCost(moneySpent, essencesSpent);
                    return;
                }
                else if (traps.Count == 2)
                {
                    Transform firstTrap = traps[0];
                    Transform secondTrap = traps[1];

                    if (railScript.orientation == Orientation.Horizontal)
                    {
                        float distance = firstTrap.position.x - secondTrap.position.x;
                        
                        if (Math.Abs(distance) >= PlayerStats.NodeWidth*4)
                        {
                            if (firstTrap.position.x > secondTrap.position.x)
                            {
                                newPitGameObject = Instantiate(pitGameObject,
                                    new Vector3(firstTrap.position.x - PlayerStats.NodeWidth*2, railScript.yAlignment),
                                    Quaternion.identity);
                            }
                            else
                            {
                                newPitGameObject = Instantiate(pitGameObject,
                                    new Vector3(secondTrap.position.x - PlayerStats.NodeWidth*2, railScript.yAlignment),
                                    Quaternion.identity);
                            }
                        }
                        else
                        {
                            PlayerStats.instance.CancelConsumable();
                            ReturnCost(moneySpent, essencesSpent);
                            return;
                        }
                    }
                    else
                    {
                        float distance = firstTrap.position.y - secondTrap.position.y;
                        if (Math.Abs(distance) >= PlayerStats.NodeWidth*4)
                        {
                            if (firstTrap.position.y > secondTrap.position.y)
                            {
                                newPitGameObject = Instantiate(pitGameObject,
                                    new Vector3(railScript.xAlignment, secondTrap.position.y + PlayerStats.NodeWidth*2),
                                    Quaternion.identity);
                            }
                            else
                            {
                                newPitGameObject = Instantiate(pitGameObject,
                                    new Vector3(railScript.xAlignment, firstTrap.position.y + PlayerStats.NodeWidth*2),
                                    Quaternion.identity);
                            }
                        }
                        else
                        {
                            PlayerStats.instance.CancelConsumable();
                            ReturnCost(moneySpent, essencesSpent);
                            return;
                        }
                    }
                }
                else
                {
                    //Debug.Log($"railScript is null: {railScript == null}");
                    if (railScript.orientation == Orientation.Horizontal)
                    {
                       // Debug.Log($"traps[0] is null: {traps[0] == null}, " +$"railScript is null: {railScript == null}, " +$"PlayerStats.NodeWidth is null: {PlayerStats.NodeWidth == null}");

                        Instantiate(pitGameObject,
                            clickCoordinates.x > traps[0].position.x
                                ? new Vector3(traps[0].position.x + PlayerStats.NodeWidth * 2, railScript.yAlignment)
                                : new Vector3(traps[0].position.x - PlayerStats.NodeWidth * 2, railScript.yAlignment),
                            Quaternion.identity);
                    }
                    else
                    {
                       // Debug.Log($"traps[0] is null: {traps[0] == null}," +$" railScript is null: {railScript == null}," +$" PlayerStats.NodeWidth is null: {PlayerStats.NodeWidth == null}");
                        Instantiate(pitGameObject,
                            clickCoordinates.y > traps[0].position.y
                                ? new Vector3(railScript.xAlignment, traps[0].position.y + PlayerStats.NodeWidth * 2)
                                : new Vector3(railScript.xAlignment, traps[0].position.y - PlayerStats.NodeWidth * 2),
                            Quaternion.identity);
                    }
                }
            }
            else
            {
                newPitGameObject = Instantiate(pitGameObject, 
                    railScript.orientation == Orientation.Horizontal
                        ? new Vector3(clickCoordinates.x, railScript.yAlignment)
                        : new Vector3(railScript.xAlignment, clickCoordinates.y), Quaternion.identity);
            }

            if (newPitGameObject != null)
            {
                newPitGameObject.GetComponent<PitTrapObject>().quantitiy = quantity;
            }
            

        }

        public bool CheckKek(Vector2 position)
        {
            //Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(position, PlayerStats.NodeWidth*2f);

            return false;

        }
        

        private void ReturnCost(bool moneySpent, bool essencesSpent)
        {
            if (essencesSpent)
            {
                PlayerStats.instance.SpendEssences(-cost);
            }
            else if (moneySpent)
            {
                PlayerStats.instance.SpendMoney(-cost);
            }
        }

        public bool CheckPlace(Vector2 coordinates)
        {
            return false;
        }
    }
}
