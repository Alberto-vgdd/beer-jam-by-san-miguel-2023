using DG.Tweening;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    public static float BOTTLE_WIDTH = 0.3f;
    public static float BOTTLE_HALF_WIDTH = BOTTLE_WIDTH / 2f;

    [Header("Components")]
    [SerializeField]
    private BoxCollider boxCollider;
    [SerializeField]
    private Transform bottleVisualsParent;
    [SerializeField]
    private GameObject bottleShadow;

    private BottleVisuals bottleVisuals;

    private Tween moveToBoxAnimation;

    internal Vector3 GetPosition()
    {
        return transform.position;
    }

    internal void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    internal Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }

    internal void SetLocalPosition(Vector3 newLocalPosition)
    {
        transform.localPosition = newLocalPosition;
    }

    internal void EnableBoxCollider(bool enabled)
    {
        boxCollider.enabled = enabled;
    }

    internal void MoveToBox(Vector3 newPosition, float time)
    {
        moveToBoxAnimation = DOTween.Sequence()
            .Append(transform.DOMove(newPosition, time))
            .Join(bottleShadow.transform.DOScale(Vector3.zero, time))
            .Join(bottleVisualsParent.DOPunchScale(new Vector3(-0.5f, 0.5f, -0.5f), 0.7f, 7, 0.6f))
            .OnComplete(() => bottleShadow.SetActive(false));
    }

    internal Tween GetMoveToBoxAnimation()
    {
        return moveToBoxAnimation;
    }

    internal void SetBottleVisuals(BottleVisuals newBottleVisuals)
    {
        bottleVisuals = newBottleVisuals;
        bottleVisuals.SetParent(bottleVisualsParent);
    }

    internal Sprite GetBottleVisualsSprite()
    {
        return bottleVisuals.GetSprite();
    }
}
