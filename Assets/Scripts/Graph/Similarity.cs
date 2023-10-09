using UnityEngine;

public class Similarity
{

    private const float WinMSE = 0.0001f;
    public float MSE { get; private set; }

    public Similarity(float mse)
    {
        MSE = mse;
    }

    public int Percent
    {
        get
        {
            if (MSE < WinMSE) return 100;
            return (int)(100 * Mathf.Exp(-Mathf.Pow((MSE - WinMSE) * 100, 2)));
        }
    }
}
