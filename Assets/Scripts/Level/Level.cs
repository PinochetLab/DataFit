using System.Collections.Generic;
using UnityEngine;

public struct ValueStruct
{
    public readonly float Right;
    public readonly float Min;
    public readonly float Max;
    public readonly float Initial;

    public ValueStruct(float right, float min, float max, float initial)
    {
        Right = right;
        Min = min;
        Max = max;
        Initial = initial;
    }

    public static ValueStruct Random()
    {
        float min = UnityEngine.Random.Range(-10, 10);
        float max = UnityEngine.Random.Range((int)min, 11);
        var right = UnityEngine.Random.Range(min, max);
        var initial = UnityEngine.Random.Range(min, max);
        return new ValueStruct(right, min, max, initial);
    }
}

public class Level
{
    public readonly Expression Expression;
    public readonly Dictionary<string, ValueStruct> ParameterValues;
    public readonly Rect ViewRect;

    private Level(Expression expression, Dictionary<string, ValueStruct> parameterValues, Rect viewRect)
    {
        Expression = expression;
        ParameterValues = parameterValues;
        ViewRect = viewRect;
        
    }


    public static List<Level> Levels => new() { Level0, Level1, Level2 };

    private static readonly Level Level0 = new(
        new Parameter("A"),
        new Dictionary<string, ValueStruct>
        {
            {"A", new ValueStruct(0.2f, -1f, 1f, -0.5f)}
        },
        new Rect(-1, -1, 2, 2)
        );
    
    private static readonly Level Level1 = new(
        new Mul(new Parameter("A"), new VarX()),
        new Dictionary<string, ValueStruct>
        {
            {"A", new ValueStruct(0.3f, -5f, 5f, -3f)}
        },
        new Rect(-1, -1, 2, 2)
    );
    
    private static readonly Level Level2 = new(
        new Mul(new Parameter("A"), new Square(new VarX())),
        new Dictionary<string, ValueStruct>
        {
            {"A", new ValueStruct(-0.5f, -1f, 1f, 0.7f)}
        },
        new Rect(-1, -1, 2, 2)
    );
}
