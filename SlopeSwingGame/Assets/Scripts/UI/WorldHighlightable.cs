using UnityEngine;

public abstract class WorldHighlightable : MonoBehaviour
{
    public bool Highlighted => highlighted;
    private bool highlighted;

    public virtual void SetHighlighted(bool isHighlighted)
    {
        highlighted = isHighlighted;

        if (highlighted)
        {
            OnHighlightStart();
        }
        else
        {
            OnHighlightStop();
        }
    }

    protected virtual void OnHighlightStart() { }
    protected virtual void OnHighlightStop() { }
    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnUpdate() { }

    private void Awake()
    {
        OnAwake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnStart();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
