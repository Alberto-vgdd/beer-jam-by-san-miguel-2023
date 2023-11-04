using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class EnterNewNameScreen : BaseScreen
{
    [Header("Parameters")]
    [SerializeField]
    private int numberOfLetters = 5;

    [Header("Components")]
    [SerializeField]
    private Transform inputLetterHolder;

    [SerializeField]
    private InputLetter inputLetterPrefab;

    [SerializeField]
    private InputLetter[] inputLetters;
    [SerializeField]
    private GameObject continueButton;
    private int selectedLetter = 0;



    void Awake()
    {
        inputLetters = new InputLetter[numberOfLetters];
        for (int i = 0; i < numberOfLetters; i++)
        {
            inputLetters[i] = Instantiate<InputLetter>(inputLetterPrefab, inputLetterHolder);
        }
    }

    private void Start()
    {
        Reset();
    }

    public void OnNavigate(CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.x != 0f)
        {
            inputLetters[selectedLetter].Highlight(false);
            selectedLetter += (int)Mathf.Sign(input.x);
            selectedLetter = Mathf.Clamp(selectedLetter, 0, numberOfLetters - 1);
            inputLetters[selectedLetter].Highlight(true);
        }
        if (input.y != 0f)
        {
            inputLetters[selectedLetter].ChangeLetter((int)Mathf.Sign(-input.y));
        }
    }

    private void OnEnable()
    {
        InputManager.Instance.WinnerPlayerControls.Gameplay.Movement.performed += OnNavigate;
        SelectGameObjectRequested?.Invoke(continueButton);
    }

    private void OnDisable()
    {
        InputManager.Instance.WinnerPlayerControls.Gameplay.Movement.performed -= OnNavigate;
    }

    public void OnContinue()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (InputLetter inputLetter in inputLetters)
        {
            stringBuilder.Append(inputLetter.GetLetter());
        }
        Reset();
        LeaderboardsManager.Instance.SetNewRecordPlayerName(stringBuilder.ToString());
        UIManager.Instance.ShowLeaderboardsScreen();
    }


    private void Reset()
    {
        selectedLetter = 0;

        foreach (InputLetter inputLetter in inputLetters)
        {
            inputLetter.Reset();
            inputLetter.Highlight(false);
        }

        inputLetters[selectedLetter].Highlight(true);
    }

}
