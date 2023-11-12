using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/On-Screen Dpad")]
public class OnScreenDpad : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;

    private Vector2 currentInput = Vector2.zero;
    private Vector2 newInput;
    private Vector3 localPositionFromInputEvent;
    [SerializeField]
    private float deadzone = 50f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerUp(PointerEventData data)
    {
        currentInput = Vector2.zero;
        SendValueToControl(currentInput);
    }

    public void OnPointerDown(PointerEventData data)
    {
        UpdateNewInputFromEvent(data);

        if (newInput != currentInput)
        {
            HapticManager.Instance.PlaySelection();

            currentInput = newInput;
            SendValueToControl(newInput);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateNewInputFromEvent(eventData);

        if (newInput != currentInput)
        {
            HapticManager.Instance.PlaySelection();

            currentInput = newInput;
            SendValueToControl(newInput);
        }
    }

    private void UpdateNewInputFromEvent(PointerEventData data)
    {
        newInput.x = 0;
        newInput.y = 0;

        localPositionFromInputEvent = rectTransform.InverseTransformPoint(data.position);

        if (localPositionFromInputEvent.x > deadzone || localPositionFromInputEvent.x < -deadzone)
        {
            newInput.x = 1f * Mathf.Sign(localPositionFromInputEvent.x);
        }

        if (localPositionFromInputEvent.y > deadzone || localPositionFromInputEvent.y < -deadzone)
        {
            newInput.y = 1f * Mathf.Sign(localPositionFromInputEvent.y);
        }
    }

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}