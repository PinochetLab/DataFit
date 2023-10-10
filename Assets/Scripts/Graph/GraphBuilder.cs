using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraphBuilder : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    //[SerializeField] private MeshFilter zoneMeshFilter;
    //[SerializeField] private MeshFilter zoneLineMeshFilter;
    [SerializeField] private LineRenderer targetLineRenderer;

    private Expression _expression;
    private Rect _viewRect;

    [SerializeField] private GameObject axisLinePrefab;
    [SerializeField] private GameObject axisNumberPrefab;
    //[SerializeField] private float validRange = 0.02f;
    //[SerializeField] private float rangeWidth = 0.005f;

    private const int cellCount = 10;

    private static float percent = 0;

    private List<Vector3> _targetPoints;
    private List<Vector3> _currentPoints;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetUp(Expression expression, Rect viewRect, Dictionary<string, float> values)
    {
        _expression = expression;
        _viewRect = viewRect;
        Build(values);
    }

    private void Update()
    {
        var noise = 0.02f * (1 - percent / 105);
        _lineRenderer.SetPositions(_currentPoints
            .Select(v => v + Vector3.up * (Random.Range(-noise, noise))).ToArray());
    }

    private List<Vector3> EvaluatePoints(Dictionary<string, float> values)
    {
        var points = new List<Vector3>();
        var xMin = _viewRect.xMin;
        var xMax = _viewRect.xMax;
        var yMin = _viewRect.yMin;
        var yMax = _viewRect.yMax;
        const int samplesCount = 100;

        Vector2 ToLimited(Vector2 v)
        {
            return new Vector2((v.x - xMin) / (xMax - xMin) * 2 - 1, (v.y - yMin) / (yMax - yMin) * 2 - 1);
        }
        
        for (var i = 0; i <= samplesCount; i++)
        {
            var t = (float)i / samplesCount;
            var x = Mathf.Lerp(xMin, xMax, t);
            var y = _expression.Evaluate(values, x);
            if (float.IsNaN(y) || float.IsInfinity(y))
            {
                continue;
            }
            var v = ToLimited(new Vector2(x, y));
            points.Add(v);
        }

        return points;
    }

    public Similarity UpdateParameters(Dictionary<string, float> values)
    {
        var points = EvaluatePoints(values);
        var mse = LineComparer.Mse(points, _targetPoints);
        _currentPoints = points;
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());
        var s = new Similarity(mse);
        percent = s.Percent;
        return s;
    }

    private static float ClosestPowerOfTwo(float x)
    {
        return Mathf.Pow(2, Mathf.Round(Mathf.Log(x * 512, 2))) / 512;
    }

    private void Build(Dictionary<string, float> values)
    {
        var xMin = _viewRect.xMin;
        var xMax = _viewRect.xMax;
        var yMin = _viewRect.yMin;
        var yMax = _viewRect.yMax;

        Vector2 ToLimited(Vector2 v)
        {
            return new Vector2((v.x - xMin) / (xMax - xMin) * 2 - 1, (v.y - yMin) / (yMax - yMin) * 2 - 1);
        }

        var points = EvaluatePoints(values);

        _targetPoints = points;

        var center = ToLimited(Vector2.zero);

        var rot90 = Quaternion.Euler(Vector3.forward * 90);

        const float axisWidth = 3;

        if (center.x is > -1 and < 1)
        {
            var axis = Instantiate(axisLinePrefab, new Vector3(center.x, 0), rot90)
                .GetComponent<LineRenderer>();
            axis.widthMultiplier *= axisWidth;
        }
        
        if (center.y is > -1 and < 1)
        {
            var axis = Instantiate(axisLinePrefab, new Vector3(0, center.y), Quaternion.identity)
                .GetComponent<LineRenderer>();
            axis.widthMultiplier *= axisWidth;
        }

        if (center.x is > -1 and < 1 && center.y is > -1 and < 1)
        {
            Instantiate(axisNumberPrefab, center, Quaternion.identity).GetComponent<AxisNumber>().SetNumber(0);
        }
        
        var dx = 2f / cellCount;
        var dy = 2f / cellCount;

        dx = ClosestPowerOfTwo(dx);
        dy = ClosestPowerOfTwo(dy);
        
        for (var xCor = center.x + dx; xCor <= 1; xCor += dx)
        {
            Instantiate(axisLinePrefab, new Vector3(xCor, 0), rot90);
            if (center.y is > -1 and < 1)
            {
                var axisNumber = Instantiate(axisNumberPrefab, 
                    new Vector3(xCor, center.y), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.SetNumber((xCor - center.x) * (yMax - yMin) / 2);
            }
        }
        
        for (var xCor = center.x - dx; xCor >= -1; xCor -= dx)
        {
            Instantiate(axisLinePrefab, new Vector3(xCor, 0), rot90);
            if (center.y is > -1 and < 1)
            {
                var axisNumber = Instantiate(axisNumberPrefab, 
                    new Vector3(xCor, center.y), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.SetNumber((xCor - center.x) * (yMax - yMin) / 2);
            }
        }
        
        for (var yCor = center.y + dy; yCor <= 1; yCor += dy)
        {
            Instantiate(axisLinePrefab, new Vector3(0, yCor), Quaternion.identity);
            if (center.x is > -1 and < 1)
            {
                var axisNumber = Instantiate(axisNumberPrefab, 
                    new Vector3(center.x, yCor), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.SetNumber((yCor - center.y) * (yMax - yMin) / 2);
            }
        }
        
        for (var yCor = center.y - dy; yCor >= -1; yCor -= dy)
        {
            Instantiate(axisLinePrefab, new Vector3(0, yCor), Quaternion.identity);
            if (center.x is > -1 and < 1)
            {
                var axisNumber = Instantiate(axisNumberPrefab, 
                    new Vector3(center.x, yCor), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.SetNumber((yCor - center.y) * (yMax - yMin) / 2);
            }
        }
        
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());

        targetLineRenderer.positionCount = points.Count;
        targetLineRenderer.SetPositions(points.ToArray());

        //zoneMeshFilter.mesh = CreateMesh(points, validRange);
        //zoneLineMeshFilter.mesh = CreateMesh(points, validRange + rangeWidth);
    }

    /*private static Mesh CreateMesh(List<Vector3> points, float radius)
    {
        var mesh = new Mesh();

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        
        for (var i = 0; i < points.Count - 1; i++)
        {
            var v = points[i];
            if (i == 0) v += (v - points[1]).normalized;
            var v1 = points[i + 1] - v;
            if (i == points.Count - 2)
            {
                v1 += (points[^1] - v).normalized;
            }
            var v2 = new Vector2(-v1.y, +v1.x).normalized * radius;
            var p1 = v - (Vector3)v2 / 2;
            var p2 = v + (Vector3)v2 / 2;
            var p3 = v + (Vector3)v2 / 2 + v1;
            var p4 = v - (Vector3)v2 / 2 + v1;

            var n = vertices.Count;
            
            vertices.AddRange(new List<Vector3> {p1, p2, p3, p4});
            normals.AddRange(Enumerable.Repeat(-Vector3.forward, 4));
            uv.AddRange(new List<Vector2>{p1, p2, p3, p4});
            triangles.AddRange(new List<int>{n + 0, n + 1, n + 2, n + 0, n + 2, n + 3});
        }

        const int s = 16;
        foreach (var p in points)
        {
            for (var i = 0; i < s; i++)
            {
                var a1 = 360f * i / s;
                var a2 = 360f * (i + 1) / s;
                
                
                var v1 = Quaternion.Euler(Vector3.forward * a1) * (Vector3.right * radius / 2);
                var v2 = Quaternion.Euler(Vector3.forward * a2) * (Vector3.right * radius / 2);
                
                var n = vertices.Count;
                
                vertices.AddRange(new List<Vector3>{p, p + v1, p + v2});
                normals.AddRange(Enumerable.Repeat(-Vector3.forward, 3));
                uv.AddRange(new List<Vector2> {p, p + v1, p + v2});
                triangles.AddRange(new List<int>{n + 0, n + 1, n + 2});
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }*/
}
