using UnityEngine;
using DG.Tweening;

public class GlowEffect : MonoBehaviour
{
    public Vector3 smallestScale = new Vector3(0.95f, 0.95f, 1f);
    public Vector3 largestScale = new Vector3(1f, 1f, 1f);
    private Sequence mySequence;

    // Logic
    public bool isRunningAnimation = false;

    private void OnEnable() 
    {
        if (!isRunningAnimation)
        {
            isRunningAnimation = true;

            // Reset scale
            transform.localScale = smallestScale;

            // Set animation
            mySequence = DOTween.Sequence();
            mySequence
            .Append(transform.DOScale(largestScale, 0.5f).SetEase(Ease.InOutSine))
            .Append(transform.DOScale(smallestScale, 0.6f).SetEase(Ease.InOutSine))
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
