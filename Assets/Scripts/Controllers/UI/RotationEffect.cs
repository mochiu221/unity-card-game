using UnityEngine;
using DG.Tweening;

public class RotationEffect : MonoBehaviour
{
    public float rotationTime = 3f;
    private Sequence mySequence;

    // Logic
    public bool isRunningAnimation = false;

    private void OnEnable() 
    {
        if (!isRunningAnimation)
        {
            isRunningAnimation = true;

            // Reset scale
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            // Set animation
            mySequence = DOTween.Sequence();
            mySequence
            .Append(transform.DORotate(new Vector3(0, 0, -360f), rotationTime, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear))
            .SetLoops(-1);
        }
    }

    private void OnDisable() 
    {
        if (isRunningAnimation)
        {
            isRunningAnimation = false;

            // Stop animation
            mySequence.Kill();
        }
    }

}
