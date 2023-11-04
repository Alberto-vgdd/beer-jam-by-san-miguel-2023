using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputLetter : MonoBehaviour
{
    private const string AVAILABLE_LETTERS = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789 ";

    [SerializeField]
    private TextMeshProUGUI letterText;
    [SerializeField]
    private GameObject highlightedUnderscore;

    private int selectedLetter = 0;
    private char letter = AVAILABLE_LETTERS[0];

    public void ChangeLetter(int increment)
    {
        selectedLetter += increment;
        selectedLetter = (selectedLetter >= AVAILABLE_LETTERS.Length) ? 0 : selectedLetter;
        selectedLetter = (selectedLetter < 0) ? AVAILABLE_LETTERS.Length - 1 : selectedLetter;
        letter = AVAILABLE_LETTERS[selectedLetter];
        letterText.text = letter.ToString();
    }

    internal void Highlight(bool highlighted)
    {
        highlightedUnderscore.SetActive(highlighted);
    }

    internal char GetLetter()
    {
        return letter;
    }

    internal void Reset()
    {
        selectedLetter = 0;
        letter = AVAILABLE_LETTERS[selectedLetter];
        letterText.text = letter.ToString();
    }
}
