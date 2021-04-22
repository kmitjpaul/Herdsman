using System.Collections.Generic;
using SplineShapeDetection;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Pool pool;

    public List<T> Spawn<T>(List<Cell> cells)
    {
        var cellsCount = (uint) cells.Count;
        var objects = new List<T>();
        var gameObjects = pool.GetPooledObjects(cellsCount);

        for (var i = 0; i < cellsCount; i++)
        {
            var go = gameObjects[i];
            go.transform.position = cells[i].Pos;
            go.SetActive(true);
            objects.Add(go.GetComponent<T>());
        }

        return objects;
    }
}