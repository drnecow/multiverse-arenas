using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPBarAnimation : MonoBehaviour
{
    private Image _hpBarImage;
    [SerializeField] Monster _monster;

    private int _maxHP;


    private void Awake()
    {
        _hpBarImage = gameObject.GetComponent<Image>();

        _maxHP = _monster.Stats.MaxHP;
        _monster.OnMonsterAllegianceChanged += (isPlayerControlled) => { if (!isPlayerControlled) _hpBarImage.color = Color.red; };
        _monster.OnMonsterHPChanged += ChangeHPBar;
    }

    public void ChangeHPBar(int newCurrentHP)
    {
        _hpBarImage.fillAmount = (float)newCurrentHP / _maxHP;
    }
}
