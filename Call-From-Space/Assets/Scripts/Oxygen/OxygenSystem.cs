namespace GameDev.Scripts.Oxygen
{
    using UnityEngine;
    using UnityEngine.UI;

    public class OxygenSystem : MonoBehaviour
    {
        public float oxygenLevel = 100f;

        public void DecreaseOxygen(float amount)
        {
            oxygenLevel -= amount;
            oxygenLevel = Mathf.Clamp(oxygenLevel, 0f, 100f);
        }

        public void IncreaseOxygen(float amount)
        {
            oxygenLevel += amount;
            oxygenLevel = Mathf.Clamp(oxygenLevel, 0f, 100f);
        }
    }
}