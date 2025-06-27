using UnityEngine;
using UnityEngine.InputSystem;

public class CardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private PlayerCardManager playerCardManager;

    private float value;
    [SerializeField] private TMPro.TextMeshProUGUI valueTMP;
    [SerializeField] private bool selectedable = true;
    [SerializeField] private bool draggable = true;
    
    private Vector3 targetPosition;
    private float positionLerpDuration = 0.3f;
    private Vector3 positionLerpVelocity = Vector3.zero;

    private float targetZAngle;
    private float rotationVelocity;
    private float rotationDuration = 0.3f;

    private PileManager lastPile;
    public PileManager LastPile => lastPile;
    public void SetLastPile(PileManager pile) { lastPile = pile; }
    private int pileIndex;
    public int PileIndex => pileIndex;
    public void SetPileIndex(int index) { pileIndex = index; }

    public float Value => value;
    public void SetValue(float newValue) { value = newValue; valueTMP.text = value.ToString(); }
    public bool Selectable => selectedable;
    public bool Draggable => draggable;
    public void SetTargetPosition(Vector3 newTarget) { targetPosition = newTarget; }
    public void SetTargetZRotation(float newTarget) { targetZAngle = newTarget; }
    public void SetAnchorPosition(Vector2 position) { rectTransform.anchoredPosition = position; }
    
    public void OnPointerDown()
    {
        if (!selectedable)
        {
            return;
        }

        //playerCardManager.SelectCard(this);
    }

    public void OnBeginDrag()
    {
        if (!draggable)
        {
            return;
        }

        //playerCardManager.BeginDragCard(this);
    }

    public void OnDrag()
    {
        if (!draggable)
        {
            return;
        }

        //playerCardManager.DragCard(this);
    }

    public void OnEndDrag()
    {
        if (!draggable)
        {
            return;
        }

        //playerCardManager.EndDragCard(this);
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        playerCardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerCardManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionLerpVelocity, positionLerpDuration);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.eulerAngles.z, targetZAngle, ref rotationVelocity, rotationDuration));
    }
}
