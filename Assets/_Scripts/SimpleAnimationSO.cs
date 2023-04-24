using UnityEngine;

[CreateAssetMenu(fileName = "New SimpleAnimationSO", menuName = "turn base 2D/Simple Animation", order = 0)]
public class SimpleAnimationSO : ScriptableObject {
    public Sprite[] frameArray;
    [Range(1, 20)]
    public int framesPerSecond = 10;
    public bool isLoop = true;
    //clamp it to frameArray lenght and -1
    [SerializeField]
    [Range(-1, 30)]
    private int frameCallback;    
    public int FrameCallback
    {
        get => frameCallback;
        private set
        {            
            frameCallback = value;
            OnValidate();
        }
    }
    private void OnValidate()
    {
        if(frameArray.Length == 0) frameCallback = -1;
        else frameCallback = Mathf.Clamp(frameCallback, -1, frameArray.Length - 1);
    }
}
