using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParameterBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text parameterNameText;
    [SerializeField] private Slider parameterSlider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text minValueText;
    [SerializeField] private TMP_Text maxValueText;
    [SerializeField] private GameObject disabledGO;
    [SerializeField] private GameObject enabledGO;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Sprite indicatorDisabledSprite;
    [SerializeField] private Sprite indicatorEnabledSprite;

    public float Value => parameterSlider.value;

    private bool _enabled;

    private float _currentValue;
    private const int FluctuationStartIndex = 3;

    private const float FluctuationPeriod = 0.2f;
    private float _time;

    public void SetUp(string parameterName, ValueStruct valueStruct, UnityAction onChange)
    {
        parameterNameText.text = parameterName;
        parameterSlider.minValue = valueStruct.Min;
        minValueText.text = valueStruct.Min.ToString(CultureInfo.InvariantCulture);
        parameterSlider.maxValue = valueStruct.Max;
        maxValueText.text = valueStruct.Max.ToString(CultureInfo.InvariantCulture);
        SetValue(valueStruct.Initial);
        parameterSlider.onValueChanged.AddListener(v =>
        {
            onChange.Invoke();
            SetValue(v);
        });
        inputField.onValueChanged.AddListener(s =>
        {
            //onChange.Invoke();
            //SetValue(float.Parse(s));
        });
        SetEnabled(false);
    }

    public void SetEnabled(bool en)
    {
        _enabled = en;
        enabledGO.SetActive(en);
        disabledGO.SetActive(!en);
        indicatorImage.sprite = _enabled ? indicatorEnabledSprite : indicatorDisabledSprite;
    }

    public bool IsEnabled => _enabled;

    public void SwitchEnabled()
    {
        if (_enabled)
        {
            SetEnabled(false);
        }
        else
        {
            if (ParameterBlockBox.IsAnyEnabled()) return;
            //ParameterBlockBox.DisableAll();
            SetEnabled(true);
        }
    }

    private void SetValue(float value)
    {
        _currentValue = value;
        SetText(value);
        parameterSlider.value = value;
    }

    private void SetText(float value)
    {
        const int digitCount = 8;
        var v = value.ToString(CultureInfo.InvariantCulture);
        if (v.Length > digitCount) v = v[..digitCount];
        if (IsEnabled) Debug.Log(v);
        inputField.text = v;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time < FluctuationPeriod) return;
        _time = 0;
        const int pow = 1000000;
        var a = (int)(_currentValue * pow);
        var p = a.ToString().Length - pow.ToString().Length - FluctuationStartIndex;
        var f1 = Mathf.Pow(10, p);
        var f = Random.Range(-f1, f1);
        SetText(_currentValue + f);
    }
}
