using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AchievementData
{
    public string title;
    public AchievementType type;
    public string value;
    public bool isUnlocked;
}

public enum AchievementType
{
    UnlockResource,
    AccumulateGold,
    SpendGold
}

public class AchievementController : MonoBehaviour
{
    private static AchievementController _instance = null;

    public static AchievementController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AchievementController>();
            }

            return _instance;
        }
    }

    [SerializeField] private Transform popUpTransform;
    [SerializeField] private Text popUpText;
    [SerializeField] private float popUpDuration = 3f;

    [SerializeField] private List<AchievementData> achievementList;
    [SerializeField] private List<AchievementData> unlockingAchievements;

    private float _popUpDurationCounter;

    private void Start()
    {
        popUpTransform.localScale = Vector3.right;
    }

    private void Update()
    {
        if (unlockingAchievements.Count != 0)
        {
            StartCoroutine(ShowAchievementPopUp());
        }
    }

    public void CheckAchievement(AchievementType type, string value)
    {
        AchievementData achievementData = achievementList.Find(a => a.type == type && a.value == value && !a.isUnlocked);

        UnlockAchievement(achievementData);
    }

    public void CheckAchievement(AchievementType type, double value)
    {
        AchievementData achievementData = achievementList.Find(a => a.type == type && double.Parse(a.value) <= value && !a.isUnlocked);

        UnlockAchievement(achievementData);
    }

    public void UnlockAchievement(AchievementData achievementData)
    {
        if (achievementData != null)
        {
            Debug.Log(achievementData.title);
            achievementData.isUnlocked = true;
            unlockingAchievements.Add(achievementData);
        }
    }

    private IEnumerator ShowAchievementPopUp()
    {
        while (unlockingAchievements.Count != 0)
        {
            AchievementData achievementData = unlockingAchievements[0];
            popUpText.text = achievementData.title;
            _popUpDurationCounter = popUpDuration;
            Vector3 initialScale = Vector3.right;
            popUpTransform.localScale = initialScale;
            while (_popUpDurationCounter > 0)
            {
                _popUpDurationCounter -= Time.unscaledDeltaTime;
                popUpTransform.localScale = Vector3.LerpUnclamped(popUpTransform.localScale, Vector3.one, 0.5f);
                yield return 0;
            }
            while (popUpTransform.localScale != initialScale)
            {
                popUpTransform.localScale = Vector2.LerpUnclamped(popUpTransform.localScale, initialScale, 0.5f);
                yield return 0;
            }
            unlockingAchievements.Remove(achievementData);
            yield return new WaitForSeconds(0f);
        }
        yield return new WaitForSeconds(1f);
    }
}