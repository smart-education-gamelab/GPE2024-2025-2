using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCardManager : MonoBehaviour
{
    [SerializeField] private GameObject standardCardPrefab;
    [SerializeField] private GameObject substituteCardPrefab;
    [SerializeField] private GameObject playerInteracables;
    [SerializeField] private GameObject debugCanvasElements;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private CourseScript courseManager;
    [SerializeField] private Camera interactablesCamera;
    private PileManager handPile;
    private FormulaManager formulaManager;

    private Card grabbedCard;
    private PileManager grabbedPile;
    private Card substituteCard;

    [SerializeField] private LayerMask cardLayer;
    [SerializeField] private LayerMask pileLayer;
    [SerializeField] private LayerMask highlightableLayers;
    [SerializeField] private LayerMask worldButtonLayer;
    private WorldHighlightable worldHighlightable;

    private bool grabbing = false;
    private float minDragDistance = 1f; // Minimum distance the cursor must move to count as "dragging"
    private float currentDragDistance = 0f;
    private Vector2 startDragPosition;
    private Vector2 currentMousePosition;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip hoverOverCard;
    [SerializeField] private AudioClip selectCard;
    [SerializeField] private AudioClip buttonClick;

    private bool playerTakingShot;
    private bool cameraFullUI;
    private bool isPaused;
    private bool isGameFinished = false;
    private CursorLockMode mouseLockMode = CursorLockMode.None;

    public void SetPlayerTakingShot(bool takingShot) { playerTakingShot = takingShot; CalculateShowUIMatrix(); }
    public void SetCameraFullUI(bool cameraUI) { cameraFullUI = cameraUI; CalculateShowUIMatrix(); }
    
    public void SetPaused(bool paused) { isPaused = paused; CalculateShowUIMatrix(); MouseLock();
        if (leaderboard.activeSelf) leaderboard.SetActive(false);
    }
    public void SetPlayerInteractables(bool show) { playerInteracables.SetActive(show); debugCanvasElements.SetActive(show);}

    public void UpdateAllCardPositions()
    {
        handPile.CalculateCardTransforms();
    }
    public void MouseMove(InputAction.CallbackContext context)
    {
        currentMousePosition = context.ReadValue<Vector2>();

        HighlightWorldObject();
        DragCard();
    }

    public void ClickAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Grab();
            PressWorldObject();
        }
        else if (context.canceled)
        {
            Release();
            ReleaseWorldObject();
        }

        currentDragDistance = 0f;
    }

    public void SetLeaderboardState(bool gameFinished = false)
    {
        if (isGameFinished) return;
        if (gameFinished)
        {
            isGameFinished = true;
            leaderboard.SetActive(true);
            CalculateShowUIMatrix();
            return;
        }
        leaderboard.SetActive(!leaderboard.activeSelf); CalculateShowUIMatrix();
    }

    public void DeleteFormulaCards()
    {
        formulaManager.DeleteFormulaCards();
    }

    public void RecycleHand()
    {
        FlushHand();
        DeleteFormulaCards();
        AddCardsToHand(10);
        courseManager.MadeShot(0, true);
        soundFXManager.PlaySoundFXClip(buttonClick, transform, 1f);
    }

    public bool IsDragging()
    {
        currentDragDistance = Mathf.Max(Vector2.Distance(startDragPosition, currentMousePosition), currentDragDistance);
        return grabbing && currentDragDistance > minDragDistance;
    }

    public Card CreateNewCard(int value)
    {
        GameObject newCardGO = Instantiate(standardCardPrefab, transform);
        Card newCard = newCardGO.GetComponent<Card>();

        newCard.SetValue(value);

        return newCard;
    }

    public void MoveCardToHand(Card card) { handPile.AddCard(card); }

    private void CalculateShowUIMatrix()
    {
        if (playerTakingShot)
        {
            SetPlayerInteractables(false);
        }
        else if (!cameraFullUI)
        {
            SetPlayerInteractables(false);
        }
        else if (isPaused)
        {
            SetPlayerInteractables(false);
        }
        else if (leaderboard.activeSelf)
        {
            SetPlayerInteractables(false);
        }
        else
        {
            SetPlayerInteractables(true);
        }
    }

    private void MouseLock()
    {
        var cameraMode = FindFirstObjectByType<CameraManager>().GetCurrentCameraMode();
        switch (isPaused)
        {
            case true:
                mouseLockMode = cameraMode.CursorLockMode;
                cameraMode.SetCursorLockMode(CursorLockMode.None);
                break;
            case false:
                cameraMode.SetCursorLockMode(mouseLockMode);
                break;
        }
    }

    private WorldHighlightable GetHighlightableViaRay(Ray ray)
    {

        if (Physics.Raycast(ray, out RaycastHit higlightableHit, Mathf.Infinity, highlightableLayers))
        {
            return higlightableHit.transform.GetComponent<WorldHighlightable>();
        }
        else
        {
            return null;
        }
    }

    private WorldButton GetWorldButtonViaRay(Ray ray)
    {

        if (Physics.Raycast(ray, out RaycastHit worldButtonHit, Mathf.Infinity, worldButtonLayer))
        {
            return worldButtonHit.transform.GetComponent<WorldButton>();
        }
        else
        {
            return null;
        }
    }

    private Card GetCardViaRay(Ray ray)
    {

        if (Physics.Raycast(ray, out RaycastHit cardHit, Mathf.Infinity, cardLayer))
        {
            // We hit the card
            return cardHit.transform.GetComponent<Card>();
        }
        else
        {
            // We hit no cards
            return null;
        }
    }

    private PileManager GetPileViaRay(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit pileHit, Mathf.Infinity, pileLayer))
        {
            // We hit a pile
            return pileHit.transform.GetComponent<PileManager>();
        }
        else
        {
            // We hit no pile
            return null;
        }
    }

    private void HighlightWorldObject()
    {
        // We don't care about highlighting when we are already focused on something else
        if (grabbing)
        {
            return;
        }

        Ray ray = interactablesCamera.ScreenPointToRay(currentMousePosition);

        WorldHighlightable highlightable = GetHighlightableViaRay(ray);

        if (highlightable)
        {
            if (highlightable != worldHighlightable)
            {
                // We moved to a new card
                if (worldHighlightable)
                {
                    worldHighlightable.SetHighlighted(false);
                }

                worldHighlightable = highlightable;
                worldHighlightable.SetHighlighted(true);
                soundFXManager.PlaySoundFXClip(hoverOverCard, transform, 1f);
            }
        }
        else
        {
            if (worldHighlightable)
            {
                worldHighlightable.SetHighlighted(false);
                worldHighlightable = null;
            }
        }
    }

    private void Grab()
    {
        Ray ray = interactablesCamera.ScreenPointToRay(currentMousePosition);

        // Find a card to grab
        Card card = GetCardViaRay(ray);
        // Find a pile
        PileManager pile = GetPileViaRay(ray);

        if (card)
        {
            // Actually grab the card
            grabbedCard = card;
            // Don't forget to get the pile as well
            grabbedPile = pile;

            grabbing = true;
            
            startDragPosition = currentMousePosition;
            
            soundFXManager.PlaySoundFXClip(selectCard, transform, 1f);
        }
    }

    private void Release()
    {
        // Check if we were dragging
        if (IsDragging())
        {
            Ray ray = interactablesCamera.ScreenPointToRay(currentMousePosition);
            PileManager newPile = GetPileViaRay(ray);

            // Try to add the card to the new pile
            if (newPile)
            {
                if (newPile.AcceptCard(grabbedCard, grabbedPile))
                {
                    // If the pile accepts then take the substitute from the old pile
                    grabbedPile.RemoveCard(substituteCard);

                }
                else
                {
                    // If the pile does not accept, replace the substitute with the original card
                    grabbedPile.SubstituteCard(substituteCard, grabbedCard);
                }
            }
            else
            {
                // Put the card back silly
                grabbedPile.SubstituteCard(substituteCard, grabbedCard);
            }
        }


        // We will let go of everything regardless
        grabbedCard = null;
        grabbedPile = null;
        substituteCard.gameObject.SetActive(false);
        grabbing = false;
    }

    private void DragCard()
    {
        if (!IsDragging())
        {
            return;
        }

        if (!grabbedCard)
        {
            return;
        }

        if (!substituteCard.gameObject.activeSelf)
        {
            substituteCard.gameObject.SetActive(true);
            substituteCard.SetValue(grabbedCard.TrueValue);
            grabbedPile.SubstituteCard(grabbedCard, substituteCard);
            grabbedCard.SetTargetYRotation(0);
        }
        else
        {
            // Mouse to world position
            Vector3 cardTargetPosition = interactablesCamera.ScreenToWorldPoint(currentMousePosition) - grabbedPile.transform.position;
            cardTargetPosition.y = 0;

            grabbedCard.SetTarget(grabbedPile.transform, cardTargetPosition);
        }

    }

    private void PressWorldObject()
    {
        if (grabbing)
        {
            return;
        }

        Ray ray = interactablesCamera.ScreenPointToRay(currentMousePosition);
        WorldButton worldButton = GetWorldButtonViaRay(ray);

        if (worldButton)
        {
            worldButton.Press();
        }
    }

    private void ReleaseWorldObject()
    {
        Ray ray = interactablesCamera.ScreenPointToRay(currentMousePosition);
        WorldButton worldButton = GetWorldButtonViaRay(ray);

        if (worldButton)
        {
            worldButton.Release();
        }
    }

    private int RandomTieredDecimal()
    {
        int tierSeed = Random.Range(1, GlobalGameSettings.SimplifiedTiers.Length);

        int randomInt = Random.Range(GlobalGameSettings.SimplifiedTiers[tierSeed - 1] / 100, GlobalGameSettings.SimplifiedTiers[tierSeed] / 100);
        int signSeed = Random.Range(0, 2);
        int sign = 1;


        if (signSeed == 1)
        {
            sign = -1;
        }

        return sign * randomInt * 100;
    }

    private int RandomDecimal()
    {
        int randomInt = Random.Range(1, 101);
        int signSeed = Random.Range(0, 2);
        int sign = 1;

        if (signSeed == 1)
        {
            sign = -1;
        }

        return sign * randomInt * 100;
    }

    private Card CreateNewCard(int value, GameObject parentPile)
    {
        Card newCard = CreateNewCard(value);

        newCard.transform.parent = parentPile.transform;

        return newCard;
    }

    private void Awake()
    {
        formulaManager = GetComponent<FormulaManager>();
        substituteCard = Instantiate(substituteCardPrefab).GetComponent<Card>();
        substituteCard.gameObject.SetActive(false);

        handPile = GameObject.FindGameObjectWithTag("Hand").GetComponent<PileManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button("Add Card")]
    private void DebugAddCardsToHand()
    {
        handPile.AddCard(CreateNewCard(RandomTieredDecimal(), handPile.gameObject));
    }

    [Button("Toggle Simplified Values")]
    private void ToggleSimplifiedValueText()
    {
        GlobalGameSettings.SetSimplifiedValueText(!GlobalGameSettings.SimplifiedValues);

        Card[] allCards = Resources.FindObjectsOfTypeAll<Card>();

        for (int i = 0; i < allCards.Length; i++)
        {
            allCards[i].UpdateValueText();
        }
    }
    
    // Test Functions

    public void AddCardsToHand(int cardAmount)
    {
        for (int i = 0; i < cardAmount; i++)
            handPile.AddCard(CreateNewCard(RandomTieredDecimal(), handPile.gameObject));
    }

    public void FlushHand()
    {
        handPile.DeleteAllCards();
    }
}
