using UnityEngine;
using GeneralLogic.Timer;
using Animancer;
using static CMF.SmoothPosition;

public class LevelUpGhost : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private AnimationClip flying;

    [Space(10)]
    [SerializeField] private float curveTimerMin;
    [SerializeField] private float curveTimerMax;
    [SerializeField] private float intensityTimerMin;
    [SerializeField] private float intensityTimerMax;

    [Space(10)]
    [SerializeField] private AnimationCurve xCurve;
    [SerializeField] private float xIntensityMin;
    [SerializeField] private float xIntensityMax;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private float yIntensityMin;
    [SerializeField] private float yIntensityMax;

    [Space(10)]
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private float radius = 2;
    [SerializeField] private float elevationOffset = 0;

    [Space(10)]
    [SerializeField] private float orbitTimerMax = 2f;
    [SerializeField] private float transitionTimerMax = 2f;
    [SerializeField] private int lifeTimeTransitions = 3;

    //private Vector3 positionOffset;
    private float angle;

    private Transform creatureTransform;
    private Transform playerTransform;

    private Transform current;
    private Transform other;
    private bool needsTransition;

    private TimerRandomized curveTimer;
    private TimerRandomized intensityTimer;

    private float intensityXValue = 0f;
    private float intensityYValue = 0f;

    private float transitionTimer = 0f;
    private float orbitTimer = 0f;
    private float transitionProgress = 0f;
    private int transitionCounter = 0;

    private void OnEnable()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        curveTimer = new TimerRandomized(Random.Range(curveTimerMin, curveTimerMax), curveTimerMin, curveTimerMax);
        curveTimer.Initialize();
        intensityTimer = new TimerRandomized(Random.Range(intensityTimerMin, intensityTimerMax), intensityTimerMin, intensityTimerMax);
        intensityTimer.Initialize();

        intensityTimer.OnTimerFinished += RandomizeIntensity;
    }

    private void OnDisable()
    {
        intensityTimer.OnTimerFinished -= RandomizeIntensity;

        curveTimer.Reset();
        intensityTimer.Reset();
    }

    private void Start()
    {
        creatureTransform = transform.parent;
        transform.parent = null;

        current = creatureTransform;
        other = playerTransform;

        animancer.Play(flying, 0.2f, FadeMode.FixedSpeed);
    }

    private void Update()
    {
        curveTimer.UpdateTimer();
        intensityTimer.UpdateTimer();

        Vector3 orbitPosCurrent = AddOrbit(current.position, radius);
        Vector3 orbitPosOther = AddOrbit(other.position, radius);

        transitionProgress = transitionTimer / transitionTimerMax;

        if (needsTransition)
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= transitionTimerMax)
            {
                transitionTimer = 0f;
                needsTransition = false;

                transitionCounter++;
                if (transitionCounter == lifeTimeTransitions)
                    Destroy(gameObject);

                Transform currentPrevious = current;
                current = other;
                other = currentPrevious;
            }
        }
        else
        {
            orbitTimer += Time.deltaTime;
            if (orbitTimer >= orbitTimerMax)
            {
                orbitTimer = 0f;
                needsTransition = true;
            }
        }

        Vector3 targetPos = Vector3.Lerp(orbitPosCurrent, orbitPosOther, transitionProgress);
        targetPos = AddVariance(targetPos);
        targetPos = CorrectHeight(targetPos);

        transform.LookAt(targetPos);
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed);
    }

    private Vector3 AddOrbit(Vector3 inputVector, float radius)
    {
        angle += Time.deltaTime * rotationSpeed;
        Vector3 positionOffset = Vector3.zero;
        positionOffset.Set(Mathf.Cos(angle) * radius, elevationOffset, Mathf.Sin(angle) * radius);
        return inputVector + positionOffset;
    }

    private Vector3 AddVariance(Vector3 inputVector)
    {
        return CalculateTargetPositionVariation(inputVector);
    }

    private Vector3 CorrectHeight(Vector3 inputVector)
    {
        if (inputVector.y <= Terrain.activeTerrain.SampleHeight(inputVector) + Terrain.activeTerrain.transform.position.y + 1)
        {
            inputVector = new Vector3(inputVector.x,
                (Terrain.activeTerrain.SampleHeight(inputVector) + Terrain.activeTerrain.transform.position.y + 1),
                inputVector.z);
        }

        return inputVector;
    }

    private void RandomizeIntensity()
    {
        intensityXValue = Random.Range(xIntensityMin, xIntensityMax);
        intensityYValue = Random.Range(yIntensityMin, yIntensityMax);
    }

    private Vector3 CalculateTargetPositionVariation(Vector3 targetPosition)
    {
        float timerRelative = Mathf.InverseLerp(0f, curveTimer.timerSetting, curveTimer.TimerCurrent);

        float variationX = xCurve.Evaluate(timerRelative) * intensityXValue;
        float variationY = yCurve.Evaluate(timerRelative) * intensityYValue;

        targetPosition = new Vector3(targetPosition.x + variationX,
                                     targetPosition.y + variationY,
                                     targetPosition.z);

        return targetPosition;
    }
}
