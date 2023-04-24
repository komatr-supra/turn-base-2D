using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Action onAnimationTick;
    private float timer;
    private int actionIndexForAdd;
    private const float FRAMERATE = .05f;
    private void Awake() {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    private void Update() {
        //timing can be same for all SpriteAnimators but its optimalization and not needed right now
        timer += Time.deltaTime;

        if (timer >= FRAMERATE)
        {
            timer -= FRAMERATE;
            
            onAnimationTick?.Invoke();
        }
    }
    public void SubscribeAnimationEvent(Action action)
    {
        onAnimationTick += action;
    }
}
