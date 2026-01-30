using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class BgSwapper : MonoBehaviour
{

    [SerializeField]
    private Sprite[] bgSprites;

    [SerializeField]
    private Image bgImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [YarnCommand("swapBG")]
    public void SwapBg(int bgIndex)
    {
        bgImage.sprite = bgSprites[bgIndex];
    }
}
