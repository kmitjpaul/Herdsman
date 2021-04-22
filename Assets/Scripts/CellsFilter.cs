using System.Collections.Generic;
using System.Linq;
using Enums;
using SplineShapeDetection;
using UnityEngine;

public class CellsFilter : MonoBehaviour
{
    public LayerMask checkSphereTarget;

    public List<Cell> Filter(IEnumerable<Cell> cells)
    {
        return cells.Where(
            c => c.Type == CellType.Allowed && !Physics2D.OverlapPoint(c.Pos, checkSphereTarget)
        ).ToList();
    }
}