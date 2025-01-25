using DeepGame.Quota;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRSim.Core;

public class Hud : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _quotaText;
    [SerializeField]
    private TMP_Text _days;
    [SerializeField]
    private TMP_Text _inventoryText;
    [SerializeField]
    private ShipInventory _inventory;

    private QuotaManager _quotaManager;

    private void Start()
    {
        _quotaManager = ServiceLocator.Get<QuotaManager>();   
    }

    void Update()
    {
        Quota quota = _quotaManager.GetQuotaData();
        _quotaText.text = _quotaManager.CurrentQuotaValue + "/" + quota.quotaValue;
        _days.text = _quotaManager.CurrentDay + "/" + quota.daysCount;
        _inventoryText.text = _inventory.Price.ToString();
    }
}
