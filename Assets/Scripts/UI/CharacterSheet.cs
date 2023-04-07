using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Project.Utils;

public class CharacterSheet : MonoBehaviour
{
    private Monster _monster;

    [SerializeField] private VisualAssets _visuals;
    [SerializeField] private Color _friendlyColor;
    [SerializeField] private Color _enemyColor;
    [SerializeField] private List<Image> _coloredElements;

    [SerializeField] private Image _monsterImage;
    [SerializeField] private TextMeshProUGUI _monsterNameText;

    [SerializeField] private List<TextMeshProUGUI> _abilityValuesText;
    [SerializeField] private List<TextMeshProUGUI> _abilityModifiersText;
    [SerializeField] private List<TextMeshProUGUI> _abilitySavesText;

    [SerializeField] private TextMeshProUGUI _sizeText;
    [SerializeField] private TextMeshProUGUI _challengeRatingText;
    [SerializeField] private TextMeshProUGUI _proficiencyBonusText;
    [SerializeField] private TextMeshProUGUI _armorClassText;


    public void SetDisplayedInfo(Monster monster)
    {
        _monster = monster;

        _monsterImage.sprite = monster.MonsterAnimator.IdleSprite;
        _monsterImage.color = monster.GetComponent<SpriteRenderer>().color;
        _monsterNameText.text = monster.Name;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mouseCoords = Utils.GetMouseWorldPosition();
            GridNode targetNode = CombatDependencies.Instance.Map.GetGridObjectAtCoords(CombatDependencies.Instance.Map.WorldPositionToXY(mouseCoords));

            if (targetNode != null)
            {
                if (targetNode.HasMonster)
                    SetDisplayedInfo(targetNode.Monster);
            }
        }
    }
}
