using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Project.Utils;
using Project.Constants;
using Project.DictionaryStructs;

public class CharacterSheet : MonoBehaviour
{
    private Monster _pinnedMonster;
    private Monster _hoveredMonster;

    [SerializeField] private VisualAssets _visuals;
    [SerializeField] private Color _friendlyColor;
    [SerializeField] private Color _enemyColor;
    [SerializeField] private TextDescriptions _descriptions;

    [SerializeField] private List<Image> _coloredElements;

    [SerializeField] private Image _monsterImage;
    [SerializeField] private HPBar _hpBar;
    [SerializeField] private TextMeshProUGUI _monsterNameText;

    [SerializeField] private List<TextMeshProUGUI> _abilityValuesText;
    [SerializeField] private List<TextMeshProUGUI> _abilityModifiersText;
    [SerializeField] private List<TextMeshProUGUI> _abilitySavesText;

    [SerializeField] private TextMeshProUGUI _sizeText;
    [SerializeField] private TextMeshProUGUI _challengeRatingText;
    [SerializeField] private TextMeshProUGUI _proficiencyBonusText;
    [SerializeField] private TextMeshProUGUI _armorClassText;

    [SerializeField] private List<TextMeshProUGUI> _skillModifiersText;

    [SerializeField] private GameObject _resistancesObject;
    [SerializeField] private GameObject _vulnerabilitiesObject;
    [SerializeField] private GameObject _resistanceOrVulnerabilityIconPrefab;
    [SerializeField] private RectTransform _resistanceIconsParent;
    [SerializeField] private RectTransform _vulnerabilityIconsParent;

    [SerializeField] private GameObject _conditionsObject;
    [SerializeField] private GameObject _featuresObject;
    [SerializeField] private GameObject _conditionOrFeatureIconPrefab;
    [SerializeField] private RectTransform _conditionIconsParent;
    private Dictionary<Condition, GameObject> _currentConditionIcons;
    [SerializeField] private RectTransform _featureIconsParent;

    [SerializeField] private GameObject _speedOrSenseIconPrefab;
    [SerializeField] private RectTransform _senseIconsParent;
    [SerializeField] private RectTransform _speedIconsParent;

    [SerializeField] private GameObject _attackRowPrefab;
    [SerializeField] private RectTransform _attackRowsParent;


    private Vector3 _lastMouseCoords = new Vector3(-1000, -1000);


    public void SetPinnedMonsterAndItsInfo(Monster monster)
    {
        if (_pinnedMonster != monster && _pinnedMonster != null)
        {
            _pinnedMonster.OnActiveConditionAdded -= AddCondition;
            _pinnedMonster.OnActiveConditionRemoved -= RemoveCondition;
            _pinnedMonster.OnMonsterHPChanged -= _hpBar.ChangeHPBar;
        }

        _pinnedMonster = monster;
        SetDisplayedInfo(_pinnedMonster);
    }
    private void SetHoveredMonsterAndItsInfo(Monster monster)
    {
        if (_hoveredMonster != monster && _hoveredMonster != null)
        {
            _hoveredMonster.OnActiveConditionAdded -= AddCondition;
            _hoveredMonster.OnActiveConditionRemoved -= RemoveCondition;
            _hoveredMonster.OnMonsterHPChanged -= _hpBar.ChangeHPBar;
        }

        _hoveredMonster = monster;
        SetDisplayedInfo(_hoveredMonster);
    }
    private void SetDisplayedInfo(Monster monster)
    {
        Color elementsColor = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;
        foreach (Image image in _coloredElements)
        {
            image.color = elementsColor;
            Image[] imageChildren = image.GetComponentsInChildren<Image>();
            foreach (Image child in imageChildren)
                child.color = elementsColor;
        }

        _monsterImage.sprite = monster.MonsterAnimator.IdleSprite;
        _monsterImage.color = monster.GetComponent<SpriteRenderer>().color;
        _hpBar.SetMonster(monster);
        _monsterNameText.text = monster.Name;


        MonsterStats monsterStats = monster.Stats;

        _abilityValuesText[0].text = monsterStats.Abilities.Strength.ToString();
        _abilityValuesText[1].text = monsterStats.Abilities.Dexterity.ToString();
        _abilityValuesText[2].text = monsterStats.Abilities.Constitution.ToString();
        _abilityValuesText[3].text = monsterStats.Abilities.Intelligence.ToString();
        _abilityValuesText[4].text = monsterStats.Abilities.Wisdom.ToString();
        _abilityValuesText[5].text = monsterStats.Abilities.Charisma.ToString();

        _abilityModifiersText[0].text = monsterStats.Abilities.GetAbilityModifier(Ability.Strength).ToString();
        _abilityModifiersText[1].text = monsterStats.Abilities.GetAbilityModifier(Ability.Dexterity).ToString();
        _abilityModifiersText[2].text = monsterStats.Abilities.GetAbilityModifier(Ability.Constitution).ToString();
        _abilityModifiersText[3].text = monsterStats.Abilities.GetAbilityModifier(Ability.Intelligence).ToString();
        _abilityModifiersText[4].text = monsterStats.Abilities.GetAbilityModifier(Ability.Wisdom).ToString();
        _abilityModifiersText[5].text = monsterStats.Abilities.GetAbilityModifier(Ability.Charisma).ToString();

        _abilitySavesText[0].text = monsterStats.GetSavingThrowBonus(Ability.Strength).ToString();
        _abilitySavesText[1].text = monsterStats.GetSavingThrowBonus(Ability.Dexterity).ToString();
        _abilitySavesText[2].text = monsterStats.GetSavingThrowBonus(Ability.Constitution).ToString();
        _abilitySavesText[3].text = monsterStats.GetSavingThrowBonus(Ability.Intelligence).ToString();
        _abilitySavesText[4].text = monsterStats.GetSavingThrowBonus(Ability.Wisdom).ToString();
        _abilitySavesText[5].text = monsterStats.GetSavingThrowBonus(Ability.Charisma).ToString();


        _sizeText.text = monsterStats.Size.ToString();

        float CR = monster.Stats.ChallengeRating;
        string displayedCRText = "";
        if (CR < 1)
        {
            if (CR == 0)
                displayedCRText = "0";
            else if (CR == 0.125)
                displayedCRText = "1/8";
            else if (CR == 0.25)
                displayedCRText = "1/4";
            else if (CR == 0.5)
                displayedCRText = "1/2";
        }
        else
            displayedCRText = monster.Stats.ChallengeRating.ToString();

        _challengeRatingText.text = displayedCRText;
        _proficiencyBonusText.text = monsterStats.ProficiencyBonus.ToString();
        _armorClassText.text = monsterStats.ArmorClass.ToString();


        _skillModifiersText[0].text = monsterStats.GetSkillModifier(Skill.Athletics).ToString();
        _skillModifiersText[1].text = monsterStats.GetSkillModifier(Skill.Acrobatics).ToString();
        _skillModifiersText[2].text = monsterStats.GetSkillModifier(Skill.Stealth).ToString();
        _skillModifiersText[3].text = monsterStats.GetSkillModifier(Skill.Perception).ToString();
        _skillModifiersText[4].text = monsterStats.GetSkillModifier(Skill.Performance).ToString();

        SetResistances(monsterStats.DamageResistances);
        SetVulnerabilities(monsterStats.DamageVulnerabilities);

        _currentConditionIcons = new Dictionary<Condition, GameObject>();
        SetConditions(monster, monster.ActiveConditions);
        SetFeatures(monsterStats.SpecialAbilities);

        SetSenses(monster);
        SetSpeed(monster);

        SetAttacks(monster);
    }

    private void SetResistances(List<DamageType> monsterDamageResistances)
    {
        foreach (RectTransform child in _resistanceIconsParent)
            Destroy(child.gameObject);

        if (monsterDamageResistances.Count == 0)
        {
            _resistancesObject.SetActive(false);
            return;
        }
        else
            _resistancesObject.SetActive(true);

        foreach (DamageType resistance in monsterDamageResistances)
        {
            GameObject resistanceIcon = Instantiate(_resistanceOrVulnerabilityIconPrefab);
            Sprite resistanceIconSprite = _visuals.GetSpriteForDamageType(resistance);
            Image image = resistanceIcon.GetComponentInChildren<Image>();
            image.sprite = resistanceIconSprite;
            image.color = Color.white;
            resistanceIcon.transform.SetParent(_resistanceIconsParent);
        }
    }
    private void SetVulnerabilities(List<DamageType> monsterDamageVulnerabilities)
    {
        foreach (RectTransform child in _vulnerabilityIconsParent)
            Destroy(child.gameObject);

        if (monsterDamageVulnerabilities.Count == 0)
        {
            _vulnerabilitiesObject.SetActive(false);
            return;
        }
        else
            _vulnerabilitiesObject.SetActive(true);

        foreach (DamageType vulnerability in monsterDamageVulnerabilities)
        {
            GameObject vulnerabilityIcon = Instantiate(_resistanceOrVulnerabilityIconPrefab);
            Sprite vulnerabilityIconSprite = _visuals.GetSpriteForDamageType(vulnerability);
            Image image = vulnerabilityIcon.GetComponentInChildren<Image>();
            image.sprite = vulnerabilityIconSprite;
            image.color = Color.white;
            vulnerabilityIcon.transform.SetParent(_vulnerabilityIconsParent);
        }
    }
    private void SetConditions(Monster monster, HashSet<Condition> monsterActiveConditions)
    {
        foreach (RectTransform child in _conditionIconsParent)
            Destroy(child.gameObject);

        if (monsterActiveConditions.Count == 0)
        {
            _featuresObject.SetActive(false);

            monster.OnActiveConditionAdded -= AddCondition;
            monster.OnActiveConditionRemoved -= RemoveCondition;
            monster.OnActiveConditionAdded += AddCondition;
            monster.OnActiveConditionRemoved += RemoveCondition;

            return;
        }
        else
            _featuresObject.SetActive(true);

        foreach (Condition condition in monsterActiveConditions)
        {
            GameObject conditionIcon = Instantiate(_conditionOrFeatureIconPrefab);
            Sprite conditionIconSprite = _visuals.GetSpriteForCondition(condition);
            Image image = conditionIcon.GetComponentInChildren<Image>();
            image.sprite = conditionIconSprite;
            image.color = Color.white;

            TooltipTarget tooltipTarget = conditionIcon.GetComponent<TooltipTarget>();
            tooltipTarget.SetText(_descriptions.GetConditionDescription(condition));
            tooltipTarget.Enabled = true;

            conditionIcon.transform.SetParent(_conditionIconsParent);

            _currentConditionIcons.Add(condition, conditionIcon);
        }

        monster.OnActiveConditionAdded -= AddCondition;
        monster.OnActiveConditionRemoved -= RemoveCondition;
        monster.OnActiveConditionAdded += AddCondition;
        monster.OnActiveConditionRemoved += RemoveCondition;
    }
    private void AddCondition(Condition condition)
    {
        if (!_conditionsObject.activeSelf)
            _conditionsObject.SetActive(true);

        GameObject conditionIcon = Instantiate(_conditionOrFeatureIconPrefab);
        Sprite conditionIconSprite = _visuals.GetSpriteForCondition(condition);
        Image image = conditionIcon.GetComponentInChildren<Image>();
        image.sprite = conditionIconSprite;
        image.color = Color.white;

        TooltipTarget tooltipTarget = conditionIcon.GetComponent<TooltipTarget>();
        tooltipTarget.SetText(_descriptions.GetConditionDescription(condition));
        tooltipTarget.Enabled = true;

        conditionIcon.transform.SetParent(_conditionIconsParent);

        _currentConditionIcons.Add(condition, conditionIcon);
    }
    private void RemoveCondition(Condition condition)
    {
        if (_currentConditionIcons.ContainsKey(condition))
        {
            Destroy(_currentConditionIcons[condition]);
            _currentConditionIcons.Remove(condition);
        }
    }
    private void SetFeatures(List<SpecialAbility> monsterSpecialAbilities)
    {
        foreach (RectTransform child in _featureIconsParent)
            Destroy(child.gameObject);

        if (monsterSpecialAbilities.Count == 0)
            _featuresObject.SetActive(false);
        else
            _featuresObject.SetActive(true);

        foreach (SpecialAbility feature in monsterSpecialAbilities)
        {
            GameObject featureIcon = Instantiate(_conditionOrFeatureIconPrefab);
            Sprite featureIconSprite = _visuals.GetSpriteForSpecialAbility(feature);
            Image image = featureIcon.GetComponentInChildren<Image>();
            image.sprite = featureIconSprite;
            image.color = Color.white;

            TooltipTarget tooltipTarget = featureIcon.GetComponent<TooltipTarget>();
            tooltipTarget.SetText(_descriptions.GetSpecialAbilityDescription(feature));
            tooltipTarget.Enabled = true;

            featureIcon.transform.SetParent(_featureIconsParent);
        }
    }
    private void SetSenses(Monster monster)
    {
        foreach (RectTransform child in _senseIconsParent)
            Destroy(child.gameObject);

        Dictionary<Sense, int> senses = monster.Stats.Senses.GetAllSenses();

        foreach (Sense sense in senses.Keys)
        {
            if (senses[sense] > 0)
            {
                GameObject senseRow = Instantiate(_speedOrSenseIconPrefab);

                Image[] rowImages = senseRow.GetComponentsInChildren<Image>();
                Color imageColor = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;

                foreach (Image image in rowImages)
                    image.color = imageColor;

                ImageTextRowInfo rowInfo = senseRow.GetComponent<ImageTextRowInfo>();
                Image rowImage = rowInfo.Image;
                rowImage.sprite = _visuals.GetSpriteForSense(sense);
                rowImage.color = Color.white;

                TextMeshProUGUI rowText = rowInfo.Text;
                rowText.text = senses[sense].ToString();

                TooltipTarget tooltipTarget = senseRow.GetComponent<TooltipTarget>();
                tooltipTarget.SetText(_descriptions.GetSenseDescription(sense));
                tooltipTarget.Enabled = true;

                senseRow.transform.SetParent(_senseIconsParent.transform);
            }
        }
    }
    private void SetSpeed(Monster monster)
    {
        foreach (RectTransform child in _speedIconsParent)
            Destroy(child.gameObject);

        Dictionary<Speed, int> speedValues = monster.Stats.Speed.GetAllSpeedValues();

        foreach (Speed speed in speedValues.Keys)
        {
            if (speedValues[speed] > 0)
            {
                GameObject speedRow = Instantiate(_speedOrSenseIconPrefab);

                Image[] rowImages = speedRow.GetComponentsInChildren<Image>();
                Color imageColor = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;

                foreach (Image image in rowImages)
                    image.color = imageColor;

                ImageTextRowInfo rowInfo = speedRow.GetComponent<ImageTextRowInfo>();
                Image rowImage = rowInfo.Image;
                rowImage.sprite = _visuals.GetSpriteForSpeed(speed);
                rowImage.color = Color.white;

                TextMeshProUGUI rowText = rowInfo.Text;
                rowText.text = speedValues[speed].ToString();

                TooltipTarget tooltipTarget = speedRow.GetComponent<TooltipTarget>();
                tooltipTarget.SetText(_descriptions.GetSpeedDescription(speed));
                tooltipTarget.Enabled = true;

                speedRow.transform.SetParent(_speedIconsParent.transform);
            }
        }
    }
    private void SetAttacks(Monster monster)
    {
        foreach (RectTransform child in _attackRowsParent)
            if (child.GetSiblingIndex() != 0)
                Destroy(child.gameObject);

        Dictionary<MeleeAttack, int> meleeAttacks = monster.CombatActions.MeleeAttacks;
        Dictionary<RangedAttack, int> rangedAttacks = monster.CombatActions.RangedAttacks;

        Dictionary<Attack, int> combinedAttacks = new Dictionary<Attack, int>();
        foreach (KeyValuePair<MeleeAttack, int> meleeAttackIntPair in meleeAttacks)
            combinedAttacks.Add(meleeAttackIntPair.Key, meleeAttackIntPair.Value);
        foreach (KeyValuePair<RangedAttack, int> rangedAttackIntPair in rangedAttacks)
            combinedAttacks.Add(rangedAttackIntPair.Key, rangedAttackIntPair.Value);

        foreach (Attack attack in combinedAttacks.Keys)
        {
            GameObject attackRow = Instantiate(_attackRowPrefab);
            AttackRowInfo attackRowInfo = attackRow.GetComponent<AttackRowInfo>();

            attackRowInfo.AttackName.text = attack.Name;
            attackRowInfo.AttackDistance.text = (attack is MeleeAttack) ? (attack as MeleeAttack).Reach.ToString() + " ft." : 
                (attack as RangedAttack).NormalRange.ToString() + "/" + (attack as RangedAttack).DisadvantageRange.ToString() + " ft.";
            attackRowInfo.ToHitBonus.text = $"1d20 + {monster.Stats.Abilities.GetAbilityModifier(attack.UsedAbility)} + {monster.Stats.ProficiencyBonus}";


            string damageText = $"{attack.InitialDamageInfo.NumberOfDamageDice + Attack.GetDamageDiceBonusForSize(monster.Stats.Size)}d{(int)attack.InitialDamageInfo.DamageDie} ";
            if (attack.IsAbilityModifierAdded)
                damageText += $"+ {monster.Stats.Abilities.GetAbilityModifier(attack.UsedAbility)}";

            foreach (AttackDamageInfo additionalDamageInfo in attack.AdditionalDamageInfo)
                damageText += $"+ {additionalDamageInfo.NumberOfDamageDice}d{(int)additionalDamageInfo.DamageDie} ";
            damageText.TrimEnd();

            attackRowInfo.Damage.text = damageText;


            Image[] rowImages = attackRow.GetComponentsInChildren<Image>();
            Color imageColor = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;

            foreach (Image image in rowImages)
                image.color = imageColor;

            attackRow.transform.SetParent(_attackRowsParent);
        }
    }


    private void Update()
    {
        Vector3 mouseCoords = Utils.GetMouseWorldPosition();

        if (mouseCoords != _lastMouseCoords)
        {
            _lastMouseCoords = mouseCoords;
            GridNode targetNode = CombatDependencies.Instance.Map.GetGridObjectAtCoords(CombatDependencies.Instance.Map.WorldPositionToXY(mouseCoords));

            if (targetNode != null)
            {
                if (targetNode.HasMonster)
                    SetHoveredMonsterAndItsInfo(targetNode.Monster);
            }
            else
            {
                if (_pinnedMonster != null)
                    SetPinnedMonsterAndItsInfo(_pinnedMonster);
            }
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            GridNode targetNode = CombatDependencies.Instance.Map.GetGridObjectAtCoords(CombatDependencies.Instance.Map.WorldPositionToXY(mouseCoords));

            if (targetNode != null)
            {
                if (targetNode.HasMonster)
                    SetPinnedMonsterAndItsInfo(targetNode.Monster);
            }
        }
    }
}
