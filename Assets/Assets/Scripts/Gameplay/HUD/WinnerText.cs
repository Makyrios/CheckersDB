using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerText : MonoBehaviour
{
    private TextMeshProUGUI winnerText;
    [SerializeField]
    public TMP_SpriteAsset whiteSprite;
    [SerializeField]
    public TMP_SpriteAsset blackSprite;
    // Start is called before the first frame update
    void Start()
    {
        winnerText = GetComponent<TextMeshProUGUI>();
        if (GameOver.winner == CheckerColor.White)
        {
            winnerText.spriteAsset = whiteSprite;
        }
        else
        {
            winnerText.spriteAsset = blackSprite;
        }
    }
}
