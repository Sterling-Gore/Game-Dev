using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : Loadable
{
    public float maxHealth = 30f;
    public float healthLevel = 30f;
    public Image healthBar;
    public float healSpeed = 5f; // Speed at which health regenerates
    private Coroutine healCoroutine;
    public GameObject gameOverScreen;
    public RectTransform healthBarRectTransform; // Assign this in the Inspector

    public AudioSource[] playerTakeDamageSounds;
    public CameraShakeGeneral cameraShake;
    public VHSPostProcessEffectCamera cameraVHS;
    public int randomIndex;

    bool HealthDelay;
    float HealthDelayTimer = 0f;

    float startWidth;
    float healthBarStartX;

    void Start()
    {
        HealthDelay = false;
        startWidth = healthBarRectTransform.sizeDelta.x;
        healthBarStartX = healthBar.rectTransform.position.x;
    }

    private void Update()
    {
        //var curWidth = healthLevel * startWidth / 100;
        //var pos = healthBar.rectTransform.position;
        //pos.x = healthBarStartX + ((startWidth - curWidth) / 4);
        //healthBar.rectTransform.position = pos;
        //healthBarRectTransform.sizeDelta = new Vector2(curWidth, healthBarRectTransform.sizeDelta.y);
        healthBar.fillAmount = healthLevel / maxHealth;

        if(HealthDelayTimer > 0)
        {
            HealthDelayTimer -= Time.deltaTime;
        }
        else
            HealthDelay = false;

        // Check if health level is zero
        if (healthLevel <= 0)
        {
            // Call animation here
            gameOverScreen.SetActive(true);
            healthLevel = maxHealth;
        }
    }

    private void OnEnable()
    {
        healCoroutine = StartCoroutine(HealOverTime());
    }

    private void OnDisable()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
    }

    private IEnumerator HealOverTime()
    {
        while (true)
        {
            if (healthLevel > 0 && healthLevel < maxHealth && !HealthDelay)
            {
                healthLevel += healSpeed * Time.deltaTime;
                healthLevel = Mathf.Clamp(healthLevel, 0, maxHealth); // Ensure health level stays within bounds
            }
            yield return null;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        HealthDelay = true;
        HealthDelayTimer = 2.5f;
        healthLevel -= damageAmount;
        healthLevel = Mathf.Clamp(healthLevel, 0, maxHealth); // Ensure health level stays within bounds
        Debug.Log("DAMAGED: " + damageAmount);

        int randomIndex = Random.Range(0, playerTakeDamageSounds.Length);
        AudioSource playerHurtSound = playerTakeDamageSounds[randomIndex];
        if( damageAmount != 10f) //this is when oxygen gone
            playerHurtSound.Play();

        if (cameraShake != null)
        {
            Debug.Log("Shaking");
            float randomDuration = Random.Range(0.35f, 0.75f);
            float randomMagnitude = Random.Range(0.50f, 0.75f);
            if(damageAmount == 10.0f){
                randomDuration = Random.Range(0.25f, 0.45f);
                randomMagnitude = Random.Range(0.05f, 0.1f);
            }
            cameraShake.StartShake(randomDuration, randomMagnitude);
        }

        if (cameraVHS != null)
        {
            cameraVHS.enabled = true;
            StartCoroutine(DisableScriptAfterDelay(cameraVHS, 1f)); // Enable VHS effect and disable it after 3 seconds
        }
    }

    private IEnumerator DisableScriptAfterDelay(MonoBehaviour script, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (script != null)
        {
            script.enabled = false; // Disable the script
        }
    }

    public override void Load(JObject state)
    {
        healthLevel = maxHealth;
    }

    public override void Save(ref JObject state)
    {
        state["health"] = healthLevel;
    }
}
