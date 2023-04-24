using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] private SimpleAnimationSO idleAnimation;
    private SpriteRenderer spriteRenderer;    
    //this is an SO for played animation
    private SimpleAnimationSO actualAnimation;
    private int currentFrame;
    private Coroutine animationCoroutine;
    private Action onAnimationEnd;
    private Action onFrameTriggered;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //method is just as description, can be directly in awake
        spriteRenderer = GetComponent<SpriteRenderer>();
        actualAnimation = idleAnimation;
    }
    private void Start()
    {
        StartAnimationCoroutine();
    }

    private void StartAnimationCoroutine()
    {
        animationCoroutine = StartCoroutine(SwitchFrameCoroutine(actualAnimation.framesPerSecond));
    }

    //??there is a way to get tick from gamemanager or similar superior class
    //??by subscribing event -> simple anim stop, anim speed change will be tricky 
    private IEnumerator SwitchFrameCoroutine(int fps)
    {
        //callculate wait only at coroutine start
        float changingTime = 1f / fps;
        var wait = new WaitForSeconds(changingTime);
        while (true)
        {
            SwitchFrame();
            yield return wait;
        }
        
    }
    //this looks odd, method do so much things
    //todo refactor
    private void SwitchFrame()
    {
        currentFrame = ++currentFrame;
        if(currentFrame < actualAnimation.frameArray.Length)
        {
            CheckFrameCallback();
        }
        else
        {
            if(actualAnimation.isLoop) currentFrame = 0;
            else
            {
                StopCoroutine(animationCoroutine);
                --currentFrame;
            }
            onAnimationEnd?.Invoke();
        }
        spriteRenderer.sprite = actualAnimation.frameArray[currentFrame];
    }

    private void CheckFrameCallback()
    {
        if(currentFrame == actualAnimation.FrameCallback) onFrameTriggered?.Invoke();
    }

    public void SetAnimation(SimpleAnimationSO simpleAnimation)
    {
        if(actualAnimation == simpleAnimation && animationCoroutine != null) return;
        //dont set animation if is only run coroutine is needed??
        actualAnimation = simpleAnimation;
        currentFrame = 0;
        //maybe start new coroutine anyway???
        if(animationCoroutine == null) StartAnimationCoroutine();
        else SwitchFrame();
    }
    
}
