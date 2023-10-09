using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class Expression
{
    public abstract float Evaluate(Dictionary<string, float> parameters, float x);

    public abstract string ToTex();
}

public class Value : Expression
{
    private readonly float value;

    public Value(float value)
    {
        this.value = value;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return value;
    }

    public override string ToTex()
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}

public class Parameter : Expression
{
    private readonly string name;

    public Parameter(string name)
    {
        this.name = name;
    }

    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        if (!parameters.ContainsKey(name))
        {
            throw new Exception("No parameter named " + name);
        }
        return parameters[name];
    }

    public override string ToTex()
    {
        return name;
    }
}

public class VarX : Expression
{
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return x;
    }

    public override string ToTex()
    {
        return "x";
    }
}

public class Brackets : Expression
{
    private readonly Expression expr;

    public Brackets(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return expr.Evaluate(parameters, x);
    }

    public override string ToTex()
    {
        return $"{{ ({expr.ToTex()}) }}";
    }
}

public class Sum : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Sum(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return left.Evaluate(parameters, x) + right.Evaluate(parameters, x);
    }

    public override string ToTex()
    {
        return $"{{{{{left.ToTex()}}}+{{{right.ToTex()}}}}}";
    }
}

public class Pow : Expression
{
    private readonly Expression basis;
    private readonly Expression power;
    
    public Pow(Expression basis, Expression power)
    {
        this.basis = basis;
        this.power = power;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Pow(basis.Evaluate(parameters, x), power.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{{{{basis.ToTex()}}}^{{{power.ToTex()}}}}}";
    }
}

public class Exp : Expression
{
    private readonly Expression power;

    public Exp(Expression power)
    {
        this.power = power;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Exp(power.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{\\exp{{{power.ToTex()}}}}}";
    }
}

public class Square : Expression
{
    private readonly Expression basis;

    public Square(Expression basis)
    {
        this.basis = basis;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Pow(basis.Evaluate(parameters, x), 2);
    }

    public override string ToTex()
    {
        return $"{{{{{basis.ToTex()}}}^2}}";
    }
}

public class Sqrt : Expression
{
    private readonly Expression basis;

    public Sqrt(Expression basis)
    {
        this.basis = basis;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Sqrt(basis.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{\\sqrt{{{basis.ToTex()}}}^2}}";
    }
}

public class Log : Expression
{
    private readonly Expression basis;
    private readonly Expression power;

    public Log(Expression basis, Expression power)
    {
        this.basis = basis;
        this.power = power;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Log(basis.Evaluate(parameters, x), power.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{\\log_{{{power.ToTex()}}}{{{basis.ToTex()}}}}}";
    }
}

public class Ln : Expression
{
    private readonly Expression basis;

    public Ln(Expression basis)
    {
        this.basis = basis;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Log(basis.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        throw new NotImplementedException();
    }
}

public class Sin : Expression
{
    private readonly Expression expr;

    public Sin(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Sin(expr.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{\\sin{{{expr.ToTex()}}}}}";
    }
}

public class Cos : Expression
{
    private readonly Expression expr;

    public Cos(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Cos(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\cos{{{expr.ToTex()}}}}}";
    }
}

public class Tan : Expression
{
    private readonly Expression expr;

    public Tan(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Tan(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\tan{{{expr.ToTex()}}}}}";
    }
}

public class Cot : Expression
{
    private readonly Expression expr;

    public Cot(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return 1 / Mathf.Tan(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\cot{{{expr.ToTex()}}}}}";
    }
}

public class Asin : Expression
{
    private readonly Expression expr;

    public Asin(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Asin(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\asin{{{expr.ToTex()}}}}}";
    }
}

public class Acos : Expression
{
    private readonly Expression expr;

    public Acos(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Acos(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\acos{{{expr.ToTex()}}}}}";
    }
}

public class Atan : Expression
{
    private readonly Expression expr;

    public Atan(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Atan(expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\atan{{{expr.ToTex()}}}}}";
    }
}

public class Acot : Expression
{
    private readonly Expression expr;

    public Acot(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Atan(1 / expr.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        return $"{{\\acot{{{expr.ToTex()}}}}}";
    }
}

public class Minus : Expression
{
    private readonly Expression expr;

    public Minus(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return -expr.Evaluate(parameters, x);
    }
    
    public override string ToTex()
    {
        return $"{{\\-{{{expr.ToTex()}}}}}";
    }
}

public class Abs : Expression
{
    private readonly Expression expr;

    public Abs(Expression expr)
    {
        this.expr = expr;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Abs(expr.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        return $"{{\\abs{{{expr.ToTex()}}}}}";
    }
}

public class Sub : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Sub(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return left.Evaluate(parameters, x) - right.Evaluate(parameters, x);
    }
    
    public override string ToTex()
    {
        return $"{{ {{ {left.ToTex()} }} - {{ {right.ToTex()} }} }}";
    }
}

public class Frac : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Frac(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return left.Evaluate(parameters, x) / right.Evaluate(parameters, x);
    }

    public override string ToTex()
    {
        return $"{{ \\frac {{{left.ToTex()}}} {{{right.ToTex()}}} }}";
    }
}

public class Mul : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Mul(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return left.Evaluate(parameters, x) * right.Evaluate(parameters, x);
    }

    public override string ToTex()
    {
        return $"{{ {{{left.ToTex()}}} * {{{right.ToTex()}}} }}";
    }
}

public class Max : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Max(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Max(left.Evaluate(parameters, x),right.Evaluate(parameters, x));
    }

    public override string ToTex()
    {
        throw new NotImplementedException();
    }
}

public class Min : Expression
{
    private readonly Expression left;
    private readonly Expression right;

    public Min(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override float Evaluate(Dictionary<string, float> parameters, float x)
    {
        return Mathf.Min(left.Evaluate(parameters, x),right.Evaluate(parameters, x));
    }
    
    public override string ToTex()
    {
        throw new NotImplementedException();
    }
}