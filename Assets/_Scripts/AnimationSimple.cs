using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSimple : MonoBehaviour
{
    [SerializeField] Sprite[] effect;
    int index;
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SubscribeAnimationEvent(Anim);
        spriteRenderer.sprite = effect[index];
    }
    private void Anim()
    {
        index = ++index < effect.Length ? index : 0;
        spriteRenderer.sprite = effect[index];
    }
}
