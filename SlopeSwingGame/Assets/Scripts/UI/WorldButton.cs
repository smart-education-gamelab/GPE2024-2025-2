using UnityEngine;
using UnityEngine.Events;

public class WorldButton : WorldHighlightable
{
    public enum WorldButtonState
    {
        inactive = -1,
        active = 0,
        highlighted = 1,
        pressed = 2
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private UnityEvent OnClickEvent;
    private bool disabled = false;
    private Sprite normalSprite;

    private WorldButtonState state = WorldButtonState.active;

    public void Press()
    {
        if (disabled)
        {
            return;
        }

        SetState(WorldButtonState.pressed);
    }

    protected override void OnHighlightStart()
    {
        base.OnHighlightStart();

        if (disabled)
        {
            return;
        }

        if (state == WorldButtonState.inactive)
        {
            return;
        }

        SetState(WorldButtonState.highlighted);
    }

    protected override void OnHighlightStop()
    {
        base.OnHighlightStop();

        if (disabled)
        {
            return;
        }

        if (state == WorldButtonState.inactive)
        {
            return;
        }

        SetState(WorldButtonState.active);
    }

    public void Release()
    {
        if (disabled)
        {
            return;
        }

        if (state == WorldButtonState.pressed)
        {
            spriteRenderer.sprite = normalSprite;
            SetState(WorldButtonState.highlighted);
            OnClickEvent.Invoke();
        }
    }

    public void Click()
    {
        Press();
        Release();
    }

    public void SetInteract(bool interactable)
    {
        if (disabled)
        {
            return;
        }

        if (interactable)
        {
            if ((int)state >= 0)
            {
                // We were already active and doing things
                return;
            }
            else
            {
                SetState(WorldButtonState.active);
            }
        }
        else
        {
            SetState(WorldButtonState.inactive);
        }
    }

    public void SetDisabled(bool isDisabled) { disabled = isDisabled; }

    private void SetState(WorldButtonState newState)
    {
        state = newState;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch (state)
        {
            case (WorldButtonState.active):
                spriteRenderer.sprite = normalSprite;
                spriteRenderer.color = activeColor;
                break;
            case (WorldButtonState.inactive):
                spriteRenderer.color = inactiveColor;
                break;
            case (WorldButtonState.highlighted):
                spriteRenderer.color = highlightedColor;
                break;
            case (WorldButtonState.pressed):
                if (pressedSprite)
                {
                    spriteRenderer.sprite = pressedSprite;
                }
                spriteRenderer.color = pressedColor;
                break;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        if (OnClickEvent == null)
        {
            OnClickEvent = new UnityEvent();
        }

        normalSprite = spriteRenderer.sprite;

        UpdateSprite();
    }
}
