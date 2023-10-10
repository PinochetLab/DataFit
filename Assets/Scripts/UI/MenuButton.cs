using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button _button;
    private TMP_Text _text;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = Color.green;
    }
}
