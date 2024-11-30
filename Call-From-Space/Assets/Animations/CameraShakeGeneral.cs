using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeGeneral : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.05f; // Duration of the shake
    [SerializeField] private float shakeMagnitude = 0.1f; // Magnitude of the shake
    private Vector3 originalLocalPos; // Store the original local position of the camera
    public bool isShaking = false;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            // Apply random shake within a sphere of shakeMagnitude
            transform.localPosition = originalLocalPos + Random.insideUnitSphere * shakeMagnitude;
        }
    }

    public void StartShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        yield return new WaitForSeconds(shakeDuration);
        isShaking = false;
        transform.localPosition = originalLocalPos; // Reset position after shaking
    }
}
