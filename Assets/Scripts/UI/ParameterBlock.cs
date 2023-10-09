using System.Globalization;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Transform sliderHandle;

    public float Value => parameterSlider.value;

    private bool _enabled;

    public void SetUp(string parameterName, ValueStruct valueStruct, UnityAction onChange)
    {
        parameterNameText.text = parameterName;
        parameterSlider.minValue = valueStruct.Min;
        minValueText.text = valueStruct.Min.ToString(CultureInfo.InvariantCulture);
        parameterSlider.maxValue = valueStruct.Max;
        maxValueText.text = valueStruct.Max.ToString(CultureInfo.InvariantCulture);
        SetValue(valueStruct.Initial);
        sliderHandle.eulerAngles = Vector3.forward * (-360 * (valueStruct.Initial - valueStruct.Min) / (
            valueStruct.Max - valueStruct.Min));
        parameterSlider.onValueChanged.AddListener(v =>
        {
            onChange.Invoke();
            SetValue(v);
            sliderHandle.eulerAngles = Vector3.forward * (-360 * (v - parameterSlider.minValue) / (
                    parameterSlider.maxValue - parameterSlider.minValue));
        });
        inputField.onValueChanged.AddListener(s =>
        {
            onChange.Invoke();
            SetValue(float.Parse(s));
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
        inputField.text = value.ToString(CultureInfo.InvariantCulture);
        parameterSlider.value = value;
    }
}
