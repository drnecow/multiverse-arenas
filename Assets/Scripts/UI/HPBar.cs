using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    private Image _hpBarImage;
    [SerializeField] Monster _monster;

    private int _maxHP;


    private void Awake()
    {
        _hpBarImage = gameObject.GetComponent<Image>();

        if (_monster != null)
        {
            _maxHP = _monster.Stats.MaxHP;
            ChangeHPBar(_monster.Stats.CurrentHP);
            _monster.OnMonsterHPChanged += ChangeHPBar;
        }
    }

    public void SetMonster(Monster monster)
    {
        _monster = monster;

        _maxHP = _monster.Stats.MaxHP;
        ChangeHPBar(_monster.Stats.CurrentHP);
        _monster.OnMonsterHPChanged += ChangeHPBar;
    }
    public void ChangeHPBar(int newCurrentHP)
    {
        _hpBarImage.fillAmount = (float)newCurrentHP / _maxHP;
    }

    private void OnDestroy()
    {
        _monster.OnMonsterHPChanged -= ChangeHPBar;
    }
}
