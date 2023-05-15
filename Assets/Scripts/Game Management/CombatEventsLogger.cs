using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Project.Constants;

public enum LogColor
{
    Neutral = 1,
    Hit = 2,
    Miss = 3,
    CriticalHit = 4,
    CriticalMiss = 5
}

public class CombatEventsLogger : MonoBehaviour
{
    [SerializeField] private GameObject _localInfoPrefab;
    [SerializeField] private GameObject _screenInfoPrefab;
    [SerializeField] private GameObject _damageNumberPrefab;

    [SerializeField] private float _localInfoDisappearanceStepInSeconds;
    [SerializeField] private float _screenInfoDisappearanceStepInSeconds;

    [SerializeField] private Color _neutralColor;
    [SerializeField] private Color _hitColor;
    [SerializeField] private Color _missColor;
    [SerializeField] private Color _criticalHitColor;
    [SerializeField] private Color _criticalMissColor;
    [SerializeField] private Color _normalDamageNumberColor;
    [SerializeField] private Color _criticalDamageNumberColor;

    HashSet<Monster> _busyLocalTargets;

    [field: SerializeField] public EventLogsChat Chat { get; private set; }


    private void Awake()
    {
        _busyLocalTargets = new HashSet<Monster>();
    }
    private Color GetLogColor(LogColor logColor)
    {
        return logColor switch
        {
            LogColor.Neutral => _neutralColor,
            LogColor.Hit => _hitColor,
            LogColor.Miss => _missColor,
            LogColor.CriticalHit => _criticalHitColor,
            LogColor.CriticalMiss => _criticalMissColor,
            _ => _neutralColor
        };
    }

    public void LogDamageNumber(Monster targetMonster, int damagePoints, bool isCrit)
    {
        StartCoroutine(AnimateDamageNumber(targetMonster, damagePoints, isCrit));
    }
    public void LogLocalInfo(Monster actor, string infoText, LogColor color=LogColor.Neutral)
    {
        StartCoroutine(AnimateLocalInfo(actor, infoText, color));
    }
    public void LogScreenInfo(string infoText, LogColor color=LogColor.Neutral)
    {
        StartCoroutine(AnimateScreenInfo(infoText, color));
    }

    public IEnumerator AnimateDamageNumber(Monster targetMonster, int damagePoints, bool isCrit)
    {
        while (_busyLocalTargets.Contains(targetMonster))
            yield return null;

        _busyLocalTargets.Add(targetMonster);

        GameObject pointsObject = Instantiate(_damageNumberPrefab);
        pointsObject.transform.SetParent(targetMonster.gameObject.transform);
        pointsObject.transform.localPosition = new Vector3(0, -1.5f);

        TextMeshProUGUI pointsText = pointsObject.GetComponentInChildren<TextMeshProUGUI>();
        pointsText.text = $"-{damagePoints}";
        pointsText.color = isCrit ? _criticalDamageNumberColor : _normalDamageNumberColor;
        CanvasGroup pointsTextCanvasGroup = pointsText.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(ConstantValues.ANIMATIONS_SWITCH_SPEED);

        for (int i = 0; i < 5; i++)
        {
            pointsObject.transform.localPosition = new Vector3(0, pointsObject.transform.localPosition.y + 0.6f);
            pointsTextCanvasGroup.alpha -= i * 0.2f;
            yield return new WaitForSeconds(0.12f);
        }

        Destroy(pointsObject);
        _busyLocalTargets.Remove(targetMonster);
    }

    public IEnumerator AnimateLocalInfo(Monster actor, string infoText, LogColor color)
    {
        GameObject infoObject = Instantiate(_localInfoPrefab);
        infoObject.transform.SetParent(actor.gameObject.transform);
        infoObject.transform.localPosition = new Vector3(0, 6.5f);

        TextMeshProUGUI localText = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        localText.text = infoText;
        localText.color = GetLogColor(color);

        CanvasGroup infoTextCanvasGroup = infoObject.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(ConstantValues.ANIMATIONS_SWITCH_SPEED);

        for (int i = 0; i < 5; i++)
        {
            infoObject.transform.localPosition = new Vector3(0, infoObject.transform.localPosition.y + 0.6f);
            infoTextCanvasGroup.alpha -= i * 0.2f;
            yield return new WaitForSeconds(_localInfoDisappearanceStepInSeconds);
        }

        Destroy(infoObject);
    }

    public IEnumerator AnimateScreenInfo(string infoText, LogColor color)
    {
        GameObject screenTextObject = Instantiate(_screenInfoPrefab);

        TextMeshProUGUI screenText = screenTextObject.GetComponentInChildren<TextMeshProUGUI>();
        screenText.text = infoText;
        screenText.color = GetLogColor(color);
        CanvasGroup infoTextCanvasGroup = screenText.GetComponent<CanvasGroup>();

        for (int i = 0; i < 5; i++)
        {
            screenTextObject.transform.localScale += new Vector3(0.06f, 0.06f, 0.06f);
            infoTextCanvasGroup.alpha -= i * 0.15f;
            yield return new WaitForSeconds(_screenInfoDisappearanceStepInSeconds);
        }

        Destroy(screenTextObject);
    }
}
