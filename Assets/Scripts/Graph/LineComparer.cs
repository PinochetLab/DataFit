using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LineComparer
{
    public static float Mse(List<Vector3> current, List<Vector3> target)
    {
        return MseByY(current, target);
    }

    private static float MseByY(List<Vector3> current, List<Vector3> target)
    {
        return current.Select(v => Mathf.Pow(Distance(v, target), 2)).Average();
    }

    private static float Distance(Vector3 point, List<Vector3> line)
    {
        var minDistance = float.PositiveInfinity;
        for (var i = 0; i < line.Count - 1; i++)
        {
            var distance = Distance(point, line[i], line[i + 1]);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;
    }

    private static float Distance(Vector3 point, Vector3 start, Vector3 end)
    {
        var l2 = Mathf.Pow(Vector3.Distance(start, end), 2);
        var t = Mathf.Clamp01(Vector3.Dot(point - start, end - start) / l2);
        var v = Vector3.Lerp(start, end, t);
        return Vector3.Distance(point, v);
    }
}
