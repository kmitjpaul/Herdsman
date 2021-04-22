using System.Collections.Generic;
using Enums;
using Helpers;
using UnityEngine;
using UnityEngine.U2D;

namespace SplineShapeDetection
{
    public class SplineClosedShapeDetector : MonoBehaviour
    {
        private static readonly Vector2Int[] Sides = {Vector2Int.up, Vector2Int.right, Vector2Int.left, Vector2Int.down};

        private List<Vector3> _bezierInterpolatedPoints;
        private float _boundsRatio;
        private Vector3 _boundsSize = Vector3.zero;
        private Vector3 _boundStart = Vector3.zero;

        private List<Cell> _cells = new List<Cell>();

        private Bounds _edgeColliderBounds;

        private CellType[,] _grid;
        private float _gridResolutionRatio;

        private Vector2Int _startingGridPoint = Vector2Int.zero;
        public float bezierSegmentsDiscretizationStep = 0.4f;
        public uint spawnGridResolution = 25;

        public SpriteShapeController spawningArea;

        public List<Cell> GetCells()
        {
            Init();
            return _cells;
        }

        private void Init()
        {
            _edgeColliderBounds = spawningArea.edgeCollider.bounds;
            _boundStart = _edgeColliderBounds.min;
            _boundsSize = _edgeColliderBounds.size;
            _boundsRatio = _boundsSize.y / _boundsSize.x;

            _gridResolutionRatio = _boundsSize.x / spawnGridResolution;

            _cells = new List<Cell>();

            CreateSpawnGrid();
            GetPointsOnSpline();
            CalculateGrid();

            ChooseStartingPoint();
            FillGrid();
            FillCells();
        }

        private void OnDrawGizmos()
        {
            Init();

            DrawPoints();
            DrawGrid();
        }

        private void ChooseStartingPoint()
        {
            var point = _bezierInterpolatedPoints[0] / _gridResolutionRatio;
            _startingGridPoint = new Vector2Int((int) Mathf.Ceil(point.x), (int) Mathf.Ceil(point.y));
        }

        private void FillCells()
        {
            for (var x = 0; x < _grid.GetLength(0); x++)
            for (var y = 0; y < _grid.GetLength(1); y++)
                _cells.Add(new Cell
                    {Pos = _boundStart + new Vector3(x, y, 0f) * _gridResolutionRatio, Type = _grid[x, y]});
        }

        private void FillGrid()
        {
            var stack = new Stack<Vector2Int>();
            var current = new Vector2Int(_startingGridPoint.x, _startingGridPoint.y);

            while (true)
            {
                _grid[current.x, current.y] = CellType.Allowed;
                var isSideToMoveFound = false;
                foreach (var side in Sides)
                {
                    var newCurrent = current + side;
                    if (!CheckGrid(newCurrent)) continue;
                    stack.Push(newCurrent);
                    current = newCurrent;
                    isSideToMoveFound = true;
                    break;
                }

                if (stack.Count == 0)
                    break;

                if (!isSideToMoveFound)
                    current = stack.Pop();
            }

            bool CheckGrid(Vector2Int vector)
            {
                if (vector.x < 0 || vector.y < 0)
                    return false;

                var outOfBounds = vector.x >= _grid.GetLength(0) || vector.y >= _grid.GetLength(1);
                return !outOfBounds && _grid[vector.x, vector.y] == CellType.Default;
            }
        }

        private void CalculateGrid()
        {
            for (var i = 0; i < _bezierInterpolatedPoints.Count - 1; i++)
            {
                var p1 = _bezierInterpolatedPoints[i];
                var p2 = _bezierInterpolatedPoints[i + 1];

                GeneratePoints(p1, p2);
            }

            GeneratePoints(_bezierInterpolatedPoints[_bezierInterpolatedPoints.Count - 1], _bezierInterpolatedPoints[0]);

            void GeneratePoints(Vector3 p1, Vector3 p2)
            {
                var (minX, maxX) = GenerateBounds(p1.x, p2.x);
                var (minY, maxY) = GenerateBounds(p1.y, p2.y);

                minX = Mathf.Max(0, minX);
                maxX = Mathf.Min(maxX, _grid.GetLength(0));

                minY = Mathf.Max(0, minY);
                maxY = Mathf.Min(maxY, _grid.GetLength(1));

                for (var x = minX; x < maxX; x++)
                for (var y = minY; y < maxY; y++)
                    _grid[x, y] = CellType.Border;
            }


            (int min, int max) GenerateBounds(float a, float b)
            {
                (a, b) = Number.Swap(a, b);

                var min = (int) Mathf.Floor(a / _gridResolutionRatio);
                var max = (int) Mathf.Ceil(b / _gridResolutionRatio);

                return (min, max);
            }
        }

        private void DrawPoints()
        {
            Gizmos.color = Color.yellow;
            foreach (var p in _bezierInterpolatedPoints) Gizmos.DrawSphere(_boundStart + p, 0.2f);
        }

        private void DrawGrid()
        {
            var size = Vector3.one * 0.2f;

            foreach (var cell in _cells)
            {
                Gizmos.color = cell.Type switch
                {
                    CellType.Default => Color.cyan,
                    CellType.Border => Color.magenta,
                    CellType.Allowed => Color.yellow,
                    _ => Color.red
                };

                Gizmos.DrawCube(cell.Pos, size);
            }
        }

        private void GetPointsOnSpline()
        {
            _edgeColliderBounds = spawningArea.edgeCollider.bounds;

            _bezierInterpolatedPoints = new List<Vector3>();

            for (var i = 0; i < spawningArea.spline.GetPointCount() - 1; i++)
            {
                var pos1 = spawningArea.spline.GetPosition(i);
                var pos2 = spawningArea.spline.GetPosition(i + 1);

                var t1 = spawningArea.spline.GetRightTangent(i);
                var t2 = spawningArea.spline.GetLeftTangent(i + 1);

                for (var j = 0f; j < 1; j += bezierSegmentsDiscretizationStep)
                {
                    var p = BezierUtility.BezierPoint(pos1, t1 + pos1, t2 + pos2, pos2, j);
                    _bezierInterpolatedPoints.Add(p - _boundStart);
                }
            }
        }

        private void CreateSpawnGrid()
        {
            var spawnGridResolutionY = (int) Mathf.Round(spawnGridResolution * _boundsRatio);

            _grid = new CellType[spawnGridResolution, spawnGridResolutionY];

            for (var x = 0; x < spawnGridResolution; x++)
            for (var y = 0; y < spawnGridResolutionY; y++)
                _grid[x, y] = CellType.Default;
        }
    }
}