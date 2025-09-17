using Animancer;
using CMF;
using SpiritGarden.Creature;
using SpiritGarden.Creature.Interaction.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimationControl : MonoBehaviour
{
    [HideInInspector] public CreatureReference creatureReference;

    [SerializeField] private LinearMixerTransition mixerMoveCycle;
    [Space(5)]
    [SerializeField] private AnimationClip jump;
    [SerializeField] private AnimationClip yes;
    [SerializeField] private AnimationClip no;
    [SerializeField] private AnimationClip eat;
    [SerializeField] private AnimationClip fire;
    [SerializeField] private AnimationClip roar;
    [SerializeField] private AnimationClip rest;
    [SerializeField] private AnimationClip sick;
    [Space(5)]
    [SerializeField] private float faceInterval = 0.75f;
    [SerializeField] private float faceFeedbackInterval = 1f;

    [SerializeField] private Material faceNeutral;
    [SerializeField] private Material[] faceChangesNeutral;
    [Space(5)]
    [SerializeField] private Material[] facesHappy;
    [SerializeField] private Material[] facesLove;
    [SerializeField] private Material[] facesUnhappy;
    [SerializeField] private Material[] facesHate;
    [Space(5)]
    [SerializeField] private List<LikeLevel> neutralReactions;
    [SerializeField] private List<LikeLevel> loveReactions;
    [SerializeField] private List<LikeLevel> happyReactions;
    [SerializeField] private List<LikeLevel> unhappyReactions;
    [SerializeField] private List<LikeLevel> hateReactions;

    private float faceChangeTimer = 0f;

    private bool activeFeedback;
    private Vector3 velocityHorizontalPrevious = Vector3.zero;
    private float smoothingFactor = 40f;
    private AnimancerComponent animancer;
    private IEnumerator faceFeedbackCoroutine;

    private void Start()
    {
        animancer = GetComponent<AnimancerComponent>();
        PlayMixerMoveCycle();
    }
    
    void Update()
    {
        if (activeFeedback == false)
        {
            AnimateNeutralFace();
            mixerMoveCycle.State.Parameter = VelocityHorizontalRelativeToMax();
        }
    }

    private bool faceIsDefault;
    private void AnimateNeutralFace()
    {
        faceChangeTimer += Time.deltaTime;

        if (faceChangeTimer > faceFeedbackInterval) 
        {
            faceChangeTimer = 0f;
            if (faceIsDefault)
                creatureReference.FaceSkinnedMeshRenderer.material = faceChangesNeutral[Random.Range(0, faceChangesNeutral.Length - 1)];
            else
                creatureReference.FaceSkinnedMeshRenderer.material = faceNeutral;

            faceIsDefault = !faceIsDefault;
        }
    }

    private void PlayMixerMoveCycle()
    {
        animancer.Play(mixerMoveCycle, 0.2f, FadeMode.FixedSpeed);
        mixerMoveCycle.State.Parameter = VelocityHorizontalRelativeToMax();
    }

    private Vector3 VelocityHorizontal()
    {
        Vector3 _velocity = creatureReference.Agent.velocity;
        Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, transform.up);
        Vector3 _verticalVelocity = _velocity - _horizontalVelocity;
        _horizontalVelocity = Vector3.Lerp(velocityHorizontalPrevious, _horizontalVelocity, smoothingFactor * Time.deltaTime);
        velocityHorizontalPrevious = _horizontalVelocity;

        return _horizontalVelocity;
    }

    private float VelocityHorizontalRelativeToMax()
    {
        return VelocityHorizontal().magnitude / creatureReference.Agent.speed;
    }

    public enum Emote
    {
        JUMP,
        YES,
        NO,
        EAT,
        FIRE,
        ROAR,
        REST,
        SICK
    }

    public void PlayEmote(Emote emote)
    {
        AnimationClip clip = fire;

        switch (emote)
        {
            case Emote.JUMP:
                clip = jump;
                break;
            case Emote.YES:
                clip = yes;
                break;
            case Emote.NO:
                clip = no;
                break;
            case Emote.EAT:
                clip = eat;
                break;
            case Emote.FIRE:
                clip = fire;
                break;
            case Emote.ROAR:
                clip = roar;
                break;
            case Emote.REST:
                clip = rest;
                break;
            case Emote.SICK:
                clip = sick;
                break;
        }

        var state = animancer.Play(clip, 0.2f, FadeMode.FixedSpeed);
        state.Time = 0;
        state.Events.OnEnd = PlayMixerMoveCycle;
    }

    public enum Expression
    {
        LOVE,
        HAPPY,
        NEUTRAL,
        UNHAPPY,
        HATE
    }

    public void SetExpression(Expression expression)
    {
        Material[] facesExpression = faceChangesNeutral;

        switch (expression)
        {
            case Expression.NEUTRAL:
                facesExpression = faceChangesNeutral;
                break;
            case Expression.HAPPY:
                facesExpression = facesHappy;
                break;
            case Expression.LOVE:
                facesExpression = facesLove;
                break;
            case Expression.UNHAPPY:
                facesExpression = facesUnhappy;
                break;
            case Expression.HATE:
                facesExpression = facesHate;
                break;
        }

        creatureReference.FaceSkinnedMeshRenderer.material = facesExpression[Random.Range(0, facesExpression.Length - 1)];
    }

    public void SetFeedbackExpressions(List<LikeLevel> likeLevels)
    {
        List<Expression> feedbackExpressions = new List<Expression>();

        foreach (LikeLevel likeLevel in likeLevels)
        {
            foreach (LikeLevel levelLove in loveReactions)
            {
                if (likeLevel == levelLove)
                    feedbackExpressions.Add(Expression.LOVE);
            }

            foreach (LikeLevel levelHappy in happyReactions)
            {
                if (likeLevel == levelHappy)
                    feedbackExpressions.Add(Expression.HAPPY);
            }

            foreach (LikeLevel levelNeutral in neutralReactions)
            {
                if (likeLevel == levelNeutral)
                    feedbackExpressions.Add(Expression.NEUTRAL);
            }

            foreach (LikeLevel levelUnhappy in unhappyReactions)
            {
                if (likeLevel == levelUnhappy)
                    feedbackExpressions.Add(Expression.UNHAPPY);
            }

            foreach (LikeLevel levelHate in hateReactions)
            {
                if (likeLevel == levelHate)
                    feedbackExpressions.Add(Expression.HATE);
            }
        }

        if (feedbackExpressions.Count == 0)
            feedbackExpressions.Add(Expression.HAPPY);

        faceFeedbackCoroutine = PlayFaceFeedback(feedbackExpressions);
        StartCoroutine(faceFeedbackCoroutine);
    }

    private IEnumerator PlayFaceFeedback(List<Expression> feedbackExpressions)
    {
        activeFeedback = true;
        int index = 0;
        while (index < feedbackExpressions.Count)
        {
            SetExpression(feedbackExpressions[index]);
            index++;
            yield return new WaitForSeconds(faceFeedbackInterval);
        }
        feedbackExpressions.Clear();
        activeFeedback = false;
        yield return null;
    }
}