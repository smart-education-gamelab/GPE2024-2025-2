using UnityEngine;
using UnityEngine.Splines;

public enum FormulaConfiguration
{
    None,
    Linear,
    XSquared,
    YSquared
}

public class SwingManager : MonoBehaviour
{
    private MathBall mathBall;
    private PlayerCardManager playerCardManager;
    private Camera mainCamera;
    [SerializeField] private int curvePointsPerUnit;

    [SerializeField] private FormulaConfiguration formulaConfiguration = FormulaConfiguration.XSquared;
    [SerializeField] private float mElementValue = 0f;
    [SerializeField] private float xyElementValue = 0f;
    private float multiplierForce = 1f;
    private Vector3[] linePoints;
    private LineRenderer shotVectorLineRenderer;
    private CourseScript courseScript;

    [SerializeField] private BetterGraphManager betterGraphManager;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip[] shootingClips;

    private bool canShoot = true;

    private int shotCounter = 0;

    public Vector3 LastShotPosition => lastShotPosition;
    private Vector3 lastShotPosition;
    
    public void SetShotMultiplierForce(float newMultiplier) { multiplierForce = newMultiplier; }

    public void SetLastShotPosition(Vector3 newPosition)
    {
        lastShotPosition = newPosition;
    }
    
    public void SetFormulaConfiguration(FormulaConfiguration newConfiguration)
    {
        formulaConfiguration = newConfiguration;
        FullCalculateAndUpdate();
    }

    public void SetMElement(float value)
    {
        mElementValue = value;
        FullCalculateAndUpdate();
    }

    public void SetXYElement(float value)
    {
        xyElementValue = value;
        FullCalculateAndUpdate();
    }

    public void SetFormula(FormulaConfiguration newConfiguration, float mValue, float xyValue)
    {
        formulaConfiguration = newConfiguration;
        mElementValue = mValue;
        xyElementValue = xyValue;
        FullCalculateAndUpdate();
    }

    public void Shoot()
    {
        if (courseScript == null)
        {
            courseScript = FindFirstObjectByType<CourseScript>();
        }

        if (!canShoot)
        {
            return;
        }

        if (xyElementValue == 0 && mElementValue == 0)
        {
            return;
        }
        mathBall.SwitchGravity(true);
        canShoot = false;
        courseScript.MadeShot(0);
        shotCounter++;
        lastShotPosition = mathBall.transform.position;
        mathBall.Shoot(linePoints, multiplierForce, formulaConfiguration != FormulaConfiguration.Linear);
        playerCardManager.DeleteFormulaCards();
        shotVectorLineRenderer.enabled = false;
        playerCardManager.SetPlayerTakingShot(true);
        xyElementValue = 0;
        mElementValue = 0;

        soundFXManager.PlayRandomSoundFXClip(shootingClips, mathBall.transform, 1f);
    }

    public void SetBall(MathBall ball)
    {
        mathBall = ball;
        mainCamera.GetComponent<CameraManager>().SetTarget(mathBall.transform);
        FullCalculateAndUpdate();
        betterGraphManager.SetLastShotPosition(mathBall.transform.position);
    }

    public void BallStoppedMoving()
    {
        shotVectorLineRenderer.enabled = true;
        playerCardManager.UpdateAllCardPositions();
        lastShotPosition = mathBall.transform.position;
        courseScript.BallStoppedMoving();
        playerCardManager.SetPlayerTakingShot(false);
        canShoot = true;
        mathBall.SwitchGravity(false);
        betterGraphManager.SetLastShotPosition(mathBall.transform.position);
    }

    private float CalculateResult()
    {
        switch (formulaConfiguration)
        {
            case (FormulaConfiguration.Linear):
                return mElementValue * xyElementValue;
            case (FormulaConfiguration.XSquared): // hehe all sneaky like
            case (FormulaConfiguration.YSquared):
                return mElementValue * xyElementValue * xyElementValue;
            default:
                return mElementValue * xyElementValue;
        }
    }

    private void CalculateLinearLinePoints()
    {
        // Set the line renderer to 2 points
        Vector3[] newPositions = new Vector3[2];

        newPositions[0] = mathBall.transform.position;
        newPositions[1] = mathBall.transform.position;

        newPositions[1].x = xyElementValue + mathBall.transform.position.x;
        newPositions[1].z = CalculateResult() + mathBall.transform.position.z;

        linePoints = newPositions;
    }

    private void CalculateSquaredLinePoints()
    {
        // How many points do we need?
        // Depends how far we are travelling and
        // how many points per line we want

        int totalPoints =Mathf.Abs((int)CalculateResult()) * curvePointsPerUnit;
        float individualStep = CalculateResult() / (float)totalPoints;

        Vector3[] newPositions = new Vector3[totalPoints];

        // We need to plot points in the line
        // for each of our total points

        // We need to account for negative xy numbers
        float xySign = Mathf.Sign(xyElementValue);

        for (int i = 0; i < newPositions.Length; i++)
        {
            float xyStep = individualStep * i;
            float resultStep = mElementValue * xyStep * xyStep * xySign;

            switch (formulaConfiguration)
            {
                case (FormulaConfiguration.XSquared):
                    newPositions[i] = new Vector3(xyStep, 0, resultStep);
                    break;
                case (FormulaConfiguration.YSquared):
                    newPositions[i] = new Vector3(resultStep, 0, xyStep);
                    break;
                default:
                    break;
            }

            newPositions[i] += mathBall.transform.position;
        }

        linePoints = newPositions;
    }

    private void CalculateLinePoints()
    {
        if (!mathBall)
        {
            return;
        }

        switch (formulaConfiguration)
        {
            case (FormulaConfiguration.Linear):
                CalculateLinearLinePoints();
                return;
            case (FormulaConfiguration.XSquared):
            case (FormulaConfiguration.YSquared):
                CalculateSquaredLinePoints();
                return;
            default:
                return;
        }
    }

    private void UpdateShotUI()
    {
        UpdateFormulaDisplay();
        UpdateShotLineRenderer();
    }

    private void UpdateFormulaDisplay()
    {
        
    }

    private void UpdateShotLineRenderer()
    {
        shotVectorLineRenderer.positionCount = linePoints.Length;
        shotVectorLineRenderer.SetPositions(linePoints);
    }

    private void FullCalculateAndUpdate()
    {
        CalculateLinePoints();
        UpdateShotUI();
    }

    private void Awake()
    {
        shotVectorLineRenderer = GetComponent<LineRenderer>();
        playerCardManager = GetComponent<PlayerCardManager>();
        linePoints = new Vector3[0];
        mainCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Increase M
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetMElement(mElementValue + 0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SetMElement(mElementValue - 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SetXYElement(xyElementValue + 0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            SetXYElement(xyElementValue - 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
}
