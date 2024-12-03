using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenSystem : Loadable
{
    public float oxygenLevel = 100f;
    public Text oxygenLevelText;
    public Image oxygenBar;
    public Image oxygenRadial;
    public float refillSpeed = 10f;
    public ParticleSystem breathEffect; // Reference to the particle system

    public AudioSource LowOxygen;
    public AudioSource CriticalOxygen;


    public bool LosingOxygen;


    [Header("Health Damage")]
    public HealthSystem healthSystem;
    public float damageAmount = 15f;
    public float damageCooldown = 1.5f;
    float timeOfDamage = 0;

    void Start()
    {
        LosingOxygen = false;
    }

    private void Update()
    {
        // Update the UI text with the current oxygen level
        if (oxygenLevelText != null)
        {
            oxygenLevelText.text = "Oxygen Level: " + Mathf.RoundToInt(oxygenLevel).ToString();
        }
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;

        // Adjust the particle effect based on the oxygen level
        if (breathEffect != null)
        {
            var emission = breathEffect.emission;
            float threshold = 30f; // Start the particle effect when oxygen level is below this threshold
            if (oxygenLevel < threshold)
            {
                emission.rateOverTime = Mathf.Lerp(0, 50, 1 - (oxygenLevel / threshold)); // Increase emission rate as oxygen level decreases below the threshold
            }
            else
            {
                emission.rateOverTime = 0; // No emission when oxygen level is above the threshold
            }
        }
        if (oxygenLevel == 0 && Time.time - timeOfDamage > damageCooldown)
        {
            healthSystem.TakeDamage(damageAmount);
            timeOfDamage = Time.time;
        }



        LowOxygen.enabled = (oxygenLevel < 40f); //enabled when oxygen is less than 40
        CriticalOxygen.enabled = (oxygenLevel < 20f); //enabled when oxygen is less than 20



    }

    public void DecreaseOxygen(float amount)
    {
        oxygenLevel = Mathf.Clamp(oxygenLevel - (amount * Time.deltaTime), 0, 100);
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;
    }

    public void IncreaseOxygen()
    {
        oxygenLevel = Mathf.Clamp(oxygenLevel + (refillSpeed * Time.deltaTime), 0, 100);
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;
    }

    public void RefillToFull()
    {
        LosingOxygen = false;
        StartCoroutine(RefillAll());
    }

    IEnumerator RefillAll()
    {
        while (oxygenLevel != 100)
        {
            IncreaseOxygen();
            yield return null;
        }
    }

    public override void Load(JObject state)
    {
        oxygenLevel = 100;
        LosingOxygen = (bool)state[fullName]["isLosingOxygen"];
    }

    public override void Save(ref JObject state)
    {
        state[fullName] = new JObject()
        {
            ["oxygenLevel"] = oxygenLevel,
            ["isLosingOxygen"] = LosingOxygen
        };
    }
}