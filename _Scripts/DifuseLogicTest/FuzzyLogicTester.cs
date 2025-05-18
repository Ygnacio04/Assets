using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuzzyLogicTester : MonoBehaviour
{
    public FuzzyLogicAI fuzzyAI;

    [Header("UI Elements")]
    public Slider healthSlider;
    public Slider distanceSlider;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI attackValueText;
    public TextMeshProUGUI retreatValueText;
    public TextMeshProUGUI patrolValueText;

    private void Start()
    {
        if (fuzzyAI == null)
            fuzzyAI = FindObjectOfType<FuzzyLogicAI>();

        // Initialize sliders
        if (healthSlider != null)
        {
            healthSlider.maxValue = fuzzyAI.maxHealth;
            healthSlider.value = fuzzyAI.health;
            healthSlider.onValueChanged.AddListener(OnHealthChanged);
        }

        if (distanceSlider != null)
        {
            distanceSlider.maxValue = fuzzyAI.maxDistance;
            distanceSlider.value = fuzzyAI.distanceToPlayer;
            distanceSlider.onValueChanged.AddListener(OnDistanceChanged);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    private void OnHealthChanged(float value)
    {
        if (fuzzyAI != null)
            fuzzyAI.health = value;
    }

    private void OnDistanceChanged(float value)
    {
        if (fuzzyAI != null)
            fuzzyAI.distanceToPlayer = value;
    }

    private void UpdateUI()
    {
        if (fuzzyAI == null) return;

        if (stateText != null)
        {
            string state = "Unknown";
            if (fuzzyAI.attackValue > fuzzyAI.retreatValue && fuzzyAI.attackValue > fuzzyAI.patrolValue)
                state = "Attack";
            else if (fuzzyAI.retreatValue > fuzzyAI.attackValue && fuzzyAI.retreatValue > fuzzyAI.patrolValue)
                state = "Retreat";
            else
                state = "Patrol";

            stateText.text = $"Current State: {state}";
        }

        if (attackValueText != null)
            attackValueText.text = $"Attack: {fuzzyAI.attackValue:F2}";

        if (retreatValueText != null)
            retreatValueText.text = $"Retreat: {fuzzyAI.retreatValue:F2}";

        if (patrolValueText != null)
            patrolValueText.text = $"Patrol: {fuzzyAI.patrolValue:F2}";
    }
}