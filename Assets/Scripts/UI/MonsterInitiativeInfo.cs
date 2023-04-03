using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInitiativeInfo : MonoBehaviour
{
    private Sprite _sprite;
    private string _name;
    private int _initiativeNumber;

    [SerializeField] private Image _spriteElement;
    [SerializeField] private TextMeshProUGUI _nameElement;
    [SerializeField] private TextMeshProUGUI _numberElement;
    
    public void SetInfo(Sprite monsterSprite, string monsterName, int initiativeNumber)
    {
        _sprite = monsterSprite;
        _name = monsterName;
        _initiativeNumber = initiativeNumber;

        _spriteElement.sprite = _sprite;
        _nameElement.text = _name;
        _numberElement.text = _initiativeNumber.ToString();
    }
}
