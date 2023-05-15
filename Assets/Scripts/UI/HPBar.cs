using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    private Image _hpBarImage;
    private TooltipTarget _tooltipTarget;
    [SerializeField] Monster _monster;

    private int _maxHP;


    private void Awake()
    {
        _hpBarImage = gameObject.GetComponent<Image>();
        _tooltipTarget = gameObject.GetComponent<TooltipTarget>();
        

        if (_monster != null)
        {
            _maxHP = _monster.Stats.MaxHP;
            ChangeHPBar(_monster.CurrentHP);

            _tooltipTarget.Enabled = true;
            _monster.OnMonsterHPChanged += ChangeHPBar;
        }
    }

    public void SetMonster(Monster monster)
    {
        _monster = monster;

        _maxHP = _monster.Stats.MaxHP;

        _tooltipTarget.Enabled = true;
        ChangeHPBar(_monster.CurrentHP);
        _monster.OnMonsterHPChanged += ChangeHPBar;
    }
    public void ChangeHPBar(int newCurrentHP)
    {
        _hpBarImage.fillAmount = (float)newCurrentHP / _maxHP;
        _tooltipTarget.SetText($"{newCurrentHP}/{_maxHP}");
    }

    private void OnDestroy()
    {
        _monster.OnMonsterHPChanged -= ChangeHPBar;
    }
}
