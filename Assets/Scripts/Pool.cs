using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private readonly List<GameObject> _pool = new List<GameObject>();

    public GameObject goPrefab;
    public uint poolSize;

    private void Start()
    {
        for (var i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(goPrefab);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
        }
    }


    public List<GameObject> GetPooledObjects(uint quantity)
    {
        var objects = new List<GameObject>();

        foreach (var o in _pool)
        {
            if (o.activeInHierarchy)
                continue;

            if (objects.Count == quantity)
                break;

            objects.Add(o);
        }

        return objects;
    }
}