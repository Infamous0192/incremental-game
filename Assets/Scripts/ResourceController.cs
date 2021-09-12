using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button resourceButton;
    public Image resourceImage;

    public Text resourceDescription;
    public Text resourceUpgradeCost;
    public Text resourceUnlockCost;

    private ResourceConfig _config;

    public bool isUnlocked = false;
    private int _level = 1;

    private void Start()
    {
        resourceButton.onClick.AddListener(() =>
        {
            if (isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    public void SetConfig(ResourceConfig config)
    {
        _config = config;

        resourceDescription.text = $"{_config.name} Lv.{_level}\n+{GetOutput().ToString("0")}";
        resourceUnlockCost.text = $"Unlock Cost\n{GetUnlockCost()}";
        resourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        
        SetUnlocked(config.unlockCost == 0);
    }

    public double GetOutput()
    {
        return _config.output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.upgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.unlockCost;
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();

        if (GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }
        
        GameManager.Instance.AddGold(-unlockCost);
        GameManager.Instance.AddSpend(unlockCost);
        
        SetUnlocked(true);
        AchievementController.Instance.CheckAchievement(AchievementType.UnlockResource, _config.name);
        GameManager.Instance.ShowNextResource();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        resourceImage.color = isUnlocked ? Color.white : Color.grey;
        resourceUnlockCost.gameObject.SetActive(!unlocked);
        resourceUpgradeCost.gameObject.SetActive(unlocked);
    }
    
    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();

        if (GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }
        
        GameManager.Instance.AddGold(-upgradeCost);
        GameManager.Instance.AddSpend(upgradeCost);
        _level++;

        resourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        resourceDescription.text = $"{_config.name} Lv.{_level}\n+{GetOutput().ToString("0")}";
    }
}
