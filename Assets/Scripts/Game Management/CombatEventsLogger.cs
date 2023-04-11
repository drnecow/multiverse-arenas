using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Project.Constants;

public class CombatEventsLogger : MonoBehaviour
{
    [SerializeField] private GameObject _localInfoPrefab;
    [SerializeField] private GameObject _screenInfoPrefab;
    [SerializeField] private GameObject _damageNumberPrefab;


    public void LogDamageNumber(Monster targetMonster, int damagePoints)
    {
        StartCoroutine(AnimateDamageNumber(targetMonster, damagePoints));
    }
    public void LogLocalInfo(Monster actor, string infoText)
    {
        StartCoroutine(AnimateLocalInfo(actor, infoText));
    }
    public void LogScreenInfo(string infoText)
    {
        StartCoroutine(AnimateScreenInfo(infoText));
    }

    public IEnumerator AnimateDamageNumber(Monster targetMonster, int damagePoints)
    {
        GameObject pointsObject = Instantiate(_damageNumberPrefab);
        pointsObject.transform.SetParent(targetMonster.gameObject.transform);
        pointsObject.transform.localPosition = new Vector3(0, -1.5f);

        TextMeshProUGUI pointsText = pointsObject.GetComponentInChildren<TextMeshProUGUI>();
        pointsText.text = $"-{damagePoints}";
        CanvasGroup pointsTextCanvasGroup = pointsText.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(ConstantValues.ANIMATIONS_SWITCH_SPEED);

        for (int i = 0; i < 5; i++)
        {
            pointsObject.transform.localPosition = new Vector3(0, pointsObject.transform.localPosition.y + 0.6f);
            pointsTextCanvasGroup.alpha -= i * 0.2f;
            yield return new WaitForSeconds(0.12f);
        }

        Destroy(pointsObject);
    }

    public IEnumerator AnimateLocalInfo(Monster actor, string infoText)
    {
        GameObject infoObject = Instantiate(_localInfoPrefab);
        infoObject.transform.SetParent(actor.gameObject.transform);
        infoObject.transform.localPosition = new Vector3(0, 6.5f);

        TextMeshProUGUI localText = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        localText.text = infoText;

        CanvasGroup infoTextCanvasGroup = infoObject.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(ConstantValues.ANIMATIONS_SWITCH_SPEED);

        for (int i = 0; i < 5; i++)
        {
            infoObject.transform.localPosition = new Vector3(0, infoObject.transform.localPosition.y + 0.6f);
            infoTextCanvasGroup.alpha -= i * 0.2f;
            yield return new WaitForSeconds(0.08f);
        }

        Destroy(infoObject);
    }

    public IEnumerator AnimateScreenInfo(string infoText)
    {
        GameObject screenTextObject = Instantiate(_screenInfoPrefab);

        TextMeshProUGUI screenText = screenTextObject.GetComponentInChildren<TextMeshProUGUI>();
        screenText.text = infoText;
        CanvasGroup infoTextCanvasGroup = screenText.GetComponent<CanvasGroup>();

        for (int i = 0; i < 5; i++)
        {
            screenTextObject.transform.localScale += new Vector3(0.06f, 0.06f, 0.06f);
            infoTextCanvasGroup.alpha -= i * 0.15f;
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(screenTextObject);
    }
}
