using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterInitiativeInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Monster _monster;
    public Monster Monster { get => _monster; }
    private int _initiativeNumber;

    [SerializeField] private HPBarAnimation _hpBar;

    [SerializeField] private Image _spriteElement;
    [SerializeField] private TextMeshProUGUI _initiativeNumberElement;
    

    public void SetInfo(Monster monster, int initiativeNumber)
    {
        _monster = monster;
        _initiativeNumber = initiativeNumber;

        _hpBar.SetMonster(monster);

        _spriteElement.sprite = _monster.GetComponent<SpriteRenderer>().sprite;
        _initiativeNumberElement.text = _initiativeNumber.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MapHighlight highlight = _monster.CombatDependencies.Highlight;
        highlight.HighlightCells(_monster.CurrentCoords, Color.grey);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MapHighlight highlight = _monster.CombatDependencies.Highlight;
        highlight.ClearHighlight();
    }
}
