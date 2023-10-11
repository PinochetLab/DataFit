using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraphBuilder : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private LineRenderer targetLineRenderer;

    private Expression _expression;
    private Rect _viewRect;

    [SerializeField] private GameObject axisLinePrefab;
    [SerializeField] private GameObject axisNumberPrefab;
    [SerializeField] private Transform placeForSpawn;
    [SerializeField] private Transform canvasForSpawn;
    [SerializeField] private AudioSource labNoise;

    private const int cellCount = 10;

    private static float percent;

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
        if (GameManager.IsMenu) return;
        var p = (1 - percent / 120);
        var noise = 0.06f * p;
        labNoise.volume = 0.35f * p;
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

    private GameObject MyInst(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(prefab, position, rotation);
        go.transform.parent = placeForSpawn;
        return go;
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
            var axis = MyInst(axisLinePrefab, new Vector3(center.x, 0), rot90).GetComponent<LineRenderer>();
            axis.widthMultiplier *= axisWidth;
        }
        
        if (center.y is > -1 and < 1)
        {
            var axis = MyInst(axisLinePrefab, new Vector3(0, center.y), Quaternion.identity)
                .GetComponent<LineRenderer>();
            axis.widthMultiplier *= axisWidth;
        }

        if (center.x is > -1 and < 1 && center.y is > -1 and < 1)
        {
            var axisNumber = MyInst(axisNumberPrefab, center, Quaternion.identity).GetComponent<AxisNumber>();
            axisNumber.transform.parent = canvasForSpawn;
            axisNumber.transform.localScale = Vector3.one;
            axisNumber.SetNumber(0);
        }
        
        var dx = 2f / cellCount;
        var dy = 2f / cellCount;

        dx = ClosestPowerOfTwo(dx);
        dy = ClosestPowerOfTwo(dy);
        
        for (var xCor = center.x + dx; xCor <= 1; xCor += dx)
        {
            MyInst(axisLinePrefab, new Vector3(xCor, 0), rot90);
            if (center.y is > -1 and < 1)
            {
                var axisNumber = MyInst(axisNumberPrefab, 
                    new Vector3(xCor, center.y), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.transform.parent = canvasForSpawn;
                axisNumber.transform.localScale = Vector3.one;
                axisNumber.SetNumber((xCor - center.x) * (yMax - yMin) / 2);
            }
        }
        
        for (var xCor = center.x - dx; xCor >= -1; xCor -= dx)
        {
            MyInst(axisLinePrefab, new Vector3(xCor, 0), rot90);
            if (center.y is > -1 and < 1)
            {
                var axisNumber = MyInst(axisNumberPrefab, 
                    new Vector3(xCor, center.y), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.transform.parent = canvasForSpawn;
                axisNumber.transform.localScale = Vector3.one;
                axisNumber.SetNumber((xCor - center.x) * (yMax - yMin) / 2);
            }
        }
        
        for (var yCor = center.y + dy; yCor <= 1; yCor += dy)
        {
            MyInst(axisLinePrefab, new Vector3(0, yCor), Quaternion.identity);
            if (center.x is > -1 and < 1)
            {
                var axisNumber = MyInst(axisNumberPrefab, 
                    new Vector3(center.x, yCor), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.transform.parent = canvasForSpawn;
                axisNumber.transform.localScale = Vector3.one;
                axisNumber.SetNumber((yCor - center.y) * (yMax - yMin) / 2);
            }
        }
        
        for (var yCor = center.y - dy; yCor >= -1; yCor -= dy)
        {
            MyInst(axisLinePrefab, new Vector3(0, yCor), Quaternion.identity);
            if (center.x is > -1 and < 1)
            {
                var axisNumber = MyInst(axisNumberPrefab, 
                    new Vector3(center.x, yCor), Quaternion.identity).GetComponent<AxisNumber>();
                axisNumber.transform.parent = canvasForSpawn;
                axisNumber.transform.localScale = Vector3.one;
                axisNumber.SetNumber((yCor - center.y) * (yMax - yMin) / 2);
            }
        }
        
        _lineRenderer.positionCount = points.Count;
        _lineRenderer.SetPositions(points.ToArray());

        targetLineRenderer.positionCount = points.Count;
        targetLineRenderer.SetPositions(points.ToArray());
    }
}
