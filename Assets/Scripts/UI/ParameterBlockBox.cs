using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParameterBlockBox : MonoBehaviour
{
    private static ParameterBlockBox _instance;

    [SerializeField] private GameObject parameterBlockPrefab;
    [SerializeField] private GameObject emptyBlockPrefab;

    private void Awake()
    {
        _instance = this;
    }

    public static ParameterBlock CreateOne()
    {
        return Instantiate(_instance.parameterBlockPrefab, _instance.transform).GetComponent<ParameterBlock>();
    }

    public static void Clear()
    {
        while (_instance.transform.childCount > 0) {
            DestroyImmediate(_instance.transform.GetChild(0).gameObject);
        }
    }

    public static void CreateEmpty()
    {
        Instantiate(_instance.emptyBlockPrefab, _instance.transform);
    }

    public static void Shake()
    {
        foreach (var t in _instance.transform.Cast<Transform>())
        {
            var block = t.GetComponent<ParameterBlock>();
            if (block.IsEnabled)
            {
                GlobalSoundPlayer.Play("RemoveSocket");
                block.Shake();
                return;
            }
        }
    }

    public static void Shuffle()
    {
        var childCount = _instance.transform.childCount;
        for (var i = 1; i < childCount; i++)
        {
            _instance.transform.GetChild(i).SetSiblingIndex(Random.Range(1, childCount));
        }
    }

    public static void DisableAll()
    {
        foreach (Transform child in _instance.transform)
        {
            (child.GetComponent<ParameterBlock>())?.SetEnabled(false);
        }
    }

    public static bool IsAnyEnabled()
    {
        return _instance.transform.Cast<Transform>().Any(t => t.GetComponent<ParameterBlock>().IsEnabled);
    }
}
