using System;
using UnityEngine;

public enum CardState
{
    None,
    Disabled,
    Active,         // Default
    Highlighted,
    Selected
}

public class Card : WorldHighlightable
{
    public bool Selectable => selectable;
    public bool Draggable => draggable;
    public float Value => value * 0.001f;
    public int TrueValue => value;
    public bool Valid => valid;
    public Vector3 TargetPosition => targetPosition;
    public CardState CardState => cardState;

    [SerializeField] private Animator animator;
    [SerializeField] private TMPro.TextMeshPro valueTMP;
    [SerializeField] private bool selectable = true;
    [SerializeField] private bool draggable = true;
    private int value = 1;
    [SerializeField] private bool valid = true;
    private CardState cardState;
    private BoxCollider boxCollider;

    [SerializeField] private Transform fillTransform;
    private float fillImageSize = 0;

    [SerializeField] private SpriteRenderer fillSprite;

    // Position
    private Transform targetTransform;
    private Vector3 targetPosition;
    private float positionLerpDuration = 0.01f;
    private Vector3 positionLerpVelocity = Vector3.zero;
    // Rotation
    private float targetYAngle;
    private float rotationVelocity;
    private float rotationDuration = 0.3f;
    public void SetValue(int newValue) { value = newValue; UpdateValueText(); }
    public void SetTarget(Transform transform, Vector3 offset) { targetTransform = transform; targetPosition = offset; }
    public void SetTargetPosition(Vector3 newTarget) { targetPosition = newTarget; }
    public void SetTargetYRotation(float newTarget) { targetYAngle = newTarget; }
    public void SetTransformPosition(Vector3 position) { transform.position = position; }
    public float GetColliderWidth() { return boxCollider.size.x; }
    public float GetColliderHeight() { return boxCollider.size.z; }

    public bool Select()
    {
        if (selectable)
        {
            cardState = CardState.Selected;
            animator.SetBool("Selected", true);
            return true;
        }

        return false;
    }

    public void Deselect()
    {
        if (cardState == CardState.Selected)
        {
            animator.SetBool("Selected", false);
            cardState = CardState.Active;
        }
    }

    public override void SetHighlighted(bool isHighlighted)
    {
        base.SetHighlighted(isHighlighted);
        animator.SetBool("Highlighted", Highlighted);
    }

    public void UpdateValueText()
    {
        valueTMP.text = GetValueText();
        fillSprite.color = GlobalGameSettings.GetTierColor(TrueValue);
    }

    public string GetValueText()
    {
        if (GlobalGameSettings.SimplifiedValues)
        {
            // Enable the fill
            fillTransform.gameObject.SetActive(true);

            // How much fill do we have?
            float fillAmount = GlobalGameSettings.GetTierPercentage(value);
            // Set the scale
            fillTransform.localScale = new Vector3(1, fillAmount, 1);

            // 0.7 is the max
            // 0.35 is the center
            // * 1 - the percentage
            // * -1 to be at the bottom

            float yPositionOffset = (fillImageSize * 0.5f) * (1 - fillAmount) * -1f;

            fillTransform.localPosition = new Vector3(0, yPositionOffset, 0);


            return GlobalGameSettings.ConvertToSimplifiedText(value);
        }
        else
        {
            fillTransform.gameObject.SetActive(false);
            return (TrueValue / 1000f).ToString();
        }
    }

    protected override void OnAwake()
    {
        boxCollider = GetComponent<BoxCollider>();
        fillImageSize = fillTransform.GetComponent<SpriteRenderer>().size.y;
    }

    protected override void OnFixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position + targetPosition, ref positionLerpVelocity, positionLerpDuration);
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYAngle, ref rotationVelocity, rotationDuration), 0);
    }
}
