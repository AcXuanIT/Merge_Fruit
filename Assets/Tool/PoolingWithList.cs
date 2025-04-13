using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PoolingWithList : Singleton<PoolingWithList>
{
    [SerializeField] private static List<GameObject> pools = new List<GameObject>();

    public static GameObject Spawn(GameObject gameObject,Vector3 position,Quaternion quaternion, Transform parents = null)
    {
        GameObject newObject;
        if(pools.Count != 0 )
        {
            newObject = Instantiate(gameObject, position, quaternion, parents);
        }
        else
        {
            foreach(var newO in pools)
            {
                if(newO.name.Equals(gameObject.name))
                {
                    newObject = newO;
                    pools.Remove(newO);
                    
                }
            }
            newObject = pools[0];
            pools.RemoveAt(0);
        }
        newObject.transform.SetPositionAndRotation(position, quaternion);
        newObject.transform.SetParent(parents);
        newObject.name = gameObject.name;
        newObject.SetActive(true);
        return newObject;
    }

    public static void DeSpawn(GameObject gameObject)
    {
        gameObject.SetActive(false);
        pools.Add(gameObject);
    }
}
