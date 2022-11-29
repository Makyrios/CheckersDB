using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentTurn : MonoBehaviour
{
    [SerializeField]
    private TMP_SpriteAsset WhiteCheckerAsset;
    [SerializeField]
    private TMP_SpriteAsset BlackCheckerAsset;

    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        text.spriteAsset = WhiteCheckerAsset;
    }

    public void ChangeTurn()
    {
        if (text.spriteAsset == WhiteCheckerAsset)
        {
            text.spriteAsset = BlackCheckerAsset;
        }
        else
        {
            text.spriteAsset = WhiteCheckerAsset;
        }
    }
}
