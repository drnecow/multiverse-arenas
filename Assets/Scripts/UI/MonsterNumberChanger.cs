using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterNumberChanger : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private TextMeshProUGUI _numberText;


    private void Awake()
    {
        _numberText = GetComponentInChildren<TextMeshProUGUI>();
        _slider.onValueChanged.AddListener((newNumber) => ChangeNumberText((int)newNumber));
    }
    private void ChangeNumberText(int newNumber)
    {
        _numberText.text = newNumber.ToString();
    }
}
