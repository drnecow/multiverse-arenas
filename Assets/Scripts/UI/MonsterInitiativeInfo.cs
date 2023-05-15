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

    private Image _blockImage;
    private Color _blockColor;

    [SerializeField] private HPBar _hpBar;

    [SerializeField] private Image _spriteElement;
    [SerializeField] private TextMeshProUGUI _initiativeNumberElement;
    

    public void SetInfo(Monster monster)
    {
        _monster = monster;
        _initiativeNumber = _monster.InitiativeRoll;

        _blockImage = GetComponent<Image>();
        _blockColor = _blockImage.color;

        _hpBar.SetMonster(_monster);

        _spriteElement.sprite = _monster.MonsterAnimator.IdleSprite;
        _initiativeNumberElement.text = _initiativeNumber.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MapHighlight highlight = _monster.CombatDependencies.Highlight;
        highlight.HighlightCells(_monster.CurrentCoords, Color.yellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MapHighlight highlight = _monster.CombatDependencies.Highlight;
        highlight.ClearHighlight();
    }

    public void HighlightBlock()
    {
        _blockImage.color = Color.yellow;
    }
    public void ClearBlockHighlight()
    {
        _blockImage.color = _blockColor;
    }
}
