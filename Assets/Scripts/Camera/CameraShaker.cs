using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    [SerializeField]
    private Transform offsetPivot;
    private Coroutine cameraShakeCoroutine;


    internal void OnShakeCameraTriggered(float duration, float magnitude, int numberOfBounces)
    {
        if (cameraShakeCoroutine != null)
        {
            StopCoroutine(cameraShakeCoroutine);
        }
        cameraShakeCoroutine = StartCoroutine(StartCameraShake(duration, magnitude, numberOfBounces));
    }

    private IEnumerator StartCameraShake(float duration, float magnitude, int numberOfBounces)
    {
        float timer = 0;
        float maxTime = duration / (float)(numberOfBounces + 1);
        Vector3 startPosition = offsetPivot.localPosition;
        Vector3 targetPosition;
        for (int i = 0; i < numberOfBounces; i++)
        {
            targetPosition = RandomPointWithMagnitudeGreateroOrEqualThan(magnitude);
            while (offsetPivot.localPosition != targetPosition)
            {
                offsetPivot.localPosition = Vector3.Lerp(startPosition, targetPosition, timer / maxTime);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            startPosition = targetPosition;
            timer -= maxTime;
        }

        while (offsetPivot.localPosition != Vector3.zero)
        {
            offsetPivot.localPosition = Vector3.Lerp(startPosition, Vector3.zero, timer / maxTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        cameraShakeCoroutine = null;
        yield return null;

    }

    private Vector3 RandomPointWithMagnitudeGreateroOrEqualThan(float strength)
    {
        Vector3 randomPoint = Vector3.zero;
        do
        {
            randomPoint.x = RandomFloat(strength);
            randomPoint.y = RandomFloat(strength);
            randomPoint.z = 0f;
        }
        while (randomPoint.magnitude < strength);

        return randomPoint;
    }

    private float RandomFloat(float absRange)
    {
        return Random.Range(-absRange, absRange);
    }
}

