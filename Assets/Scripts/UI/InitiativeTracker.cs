using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    private Dictionary<Monster, MonsterInitiativeInfo> _initiativeInfo;
    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private GameObject _initiativeInfoPrefab;

    public void SetInitiativeInfo(Dictionary<Monster, int> initativesList)
    {
        _initiativeInfo = new Dictionary<Monster, MonsterInitiativeInfo>();

        foreach (Monster monster in initativesList.Keys)
        {
            GameObject infoItem = Instantiate(_initiativeInfoPrefab);
            infoItem.transform.SetParent(_contentParent);

            MonsterInitiativeInfo info = infoItem.GetComponent<MonsterInitiativeInfo>();
            info.SetInfo(monster.gameObject.GetComponent<SpriteRenderer>().sprite, monster.Name, initativesList[monster]);

            _initiativeInfo.Add(monster, info);
        }
    }
}
