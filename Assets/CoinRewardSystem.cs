using UnityEngine;
using TMPro;

public class CoinRewardSystem : MonoBehaviour
{
    [Header("Reward Amounts")]
    public int tutorialReward = 100;
    public int levelCompleteReward = 250;
    public int enemyDefeatReward = 10;

    [Header("UI Reference (optional)")]
    public TMP_Text rewardText; // Assign your panel UI text here

    public static CoinRewardSystem Instance { get; private set; }

    private int tutorialEnemyCoins = 0;
    private int missionEnemyCoins = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Instantly shows the reward amount in the panel.
    /// </summary>
    public void ShowReward(int amount)
    {
        if (rewardText != null)
        {
            rewardText.gameObject.SetActive(true);
            rewardText.text = $"You earned {amount} coins!";
        }
    }

    // Call this when an enemy is defeated during the tutorial
    public void RewardEnemyDefeat(bool isTutorial = false)
    {
        if (isTutorial)
            tutorialEnemyCoins += enemyDefeatReward;
        else
            missionEnemyCoins += enemyDefeatReward;
    }

    // Call this when the tutorial is completed
    public void RewardTutorial()
    {
        int totalReward = tutorialReward + tutorialEnemyCoins;
        CoinSystem.AddCoins(totalReward);
        ShowReward(totalReward);
        UpdateUI();
        tutorialEnemyCoins = 0; // Reset tally for next tutorial
    }

    // Call this when the mission/level is completed
    public void RewardLevelComplete()
    {
        int totalReward = levelCompleteReward + missionEnemyCoins;
        CoinSystem.AddCoins(totalReward);
        ShowReward(totalReward);
        UpdateUI();
        missionEnemyCoins = 0; // Reset tally for next mission
    }

    private void UpdateUI()
    {
        CoinSystem coinSystem = Object.FindFirstObjectByType<CoinSystem>();
        if (coinSystem != null)
        {
            coinSystem.UpdateCoinsUI();
        }
    }
}