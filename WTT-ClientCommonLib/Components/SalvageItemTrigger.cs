using System;
using System.Collections.Generic;
using EFT.Interactive;
using UnityEngine;

public class SalvageItemTrigger : TriggerWithId
{
    [Serializable]
    public class SalvageReward
    {
        public string ItemTpl;
        public int Count;
        public bool ToQuestInventory;
    }

    [SerializeField]
    private string _requiredItemTpl;

    [SerializeField]
    private float _salvageTime = 10f;

    [SerializeField]
    private List<SalvageReward> _rewards = new();

    [SerializeField]
    private bool _consumeRequiredItem = true;

    public string RequiredItemTpl => _requiredItemTpl;
    public float SalvageTime => _salvageTime;
    public IReadOnlyList<SalvageReward> Rewards => _rewards;
    public bool ConsumeRequiredItem => _consumeRequiredItem;

    public void Configure(string requiredTpl, float time, IEnumerable<SalvageReward> rewards, bool consumeRequiredItem = true)
    {
        _requiredItemTpl = requiredTpl;
        _salvageTime = time;
        _rewards = new List<SalvageReward>(rewards);
        _consumeRequiredItem = consumeRequiredItem;
    }
}
