using System.Globalization;
using TMPro;
using UnityEngine;

public class AxisNumber : MonoBehaviour
{
    private TMP_Text _text;

    private TMP_Text Text
    {
        get
        {
            if (!_text)
            {
                _text = GetComponentInChildren<TMP_Text>();
            }

            return _text;
        }
    }

    public void SetNumber(float number)
    {
        Text.text = number.ToString(CultureInfo.InvariantCulture);
    }
}
