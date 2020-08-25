using UnityEngine;
using UnityEngine.Events;
using System;

public class SinputEventLauncher : MonoBehaviour
{
    [SerializeField]
    TimeMode checkTime = TimeMode.Update;
    [SerializeField]
    SButton[] buttons = null;
    [SerializeField]
    SAxis[] axes = null;
    [SerializeField]
    SVector[] joysticks = null;
    [SerializeField]
    SButtonsState[] buttonStates = null;

    void Update()
    {
        if (checkTime == TimeMode.Update) CheckInput();
    }

    void FixedUpdate()
    {
        if (checkTime == TimeMode.FixedUpdate) CheckInput();
    }

    void OnDisable()
    {
        for (int n = 0; n < buttons.Length; n++) if (buttons[n].isPressed)
            {
                buttons[n].SetState(false);
                buttons[n].events.Released?.Invoke();
            }

        for (int n = 0; n < axes.Length; n++)
        {
            if (axes[n].sendWhenZeroToo) axes[n].AxisValue?.Invoke(0f);
            if (axes[n].isPressed)
            {
                axes[n].SetState(false);
                axes[n].events.Released?.Invoke();
            }
        }

        for (int n = 0; n < joysticks.Length; n++)
        {
            if (joysticks[n].sendWhenZeroToo) joysticks[n].JoystickValue?.Invoke(Vector2.zero);
            if (joysticks[n].isPressed)
            {
                joysticks[n].SetState(false);
                joysticks[n].events.Released?.Invoke();
            }
        }

        for (int n = 0; n < buttonStates.Length; n++)
        {
            if (buttonStates[n].isPressed)
            {
                buttonStates[n].SetState(false);
                buttonStates[n].events.Released?.Invoke();
            }
        }
    }

    void CheckInput()
    {
        for (int n = 0; n < buttons.Length; n++)
        {
            if (Sinput.GetButtonDown(buttons[n].name))
            {
                buttons[n].SetState(true);
                buttons[n].events.Pressed?.Invoke();
            }

            if (Sinput.GetButtonUp(buttons[n].name))
            {
                buttons[n].SetState(false);
                buttons[n].events.Released?.Invoke();
            }
        }

        for (int n = 0; n < axes.Length; n++)
        {
            bool state = false;
            for (int i = 0; i < axes[n].names.Length; i++)
            {
                float value = Sinput.GetAxis(axes[n].names[i]);
                state = Mathf.Abs(value) > 0f;
                if (axes[n].sendWhenZeroToo || state)
                {
                    axes[n].AxisValue?.Invoke(value);
                    break;
                }
            }

            bool prevState = axes[n].isPressed;
            axes[n].SetState(state);

            if (state) { if (!prevState) axes[n].events.Pressed?.Invoke(); }
            else if (prevState) axes[n].events.Released?.Invoke();
        }
        
        for (int n = 0; n < joysticks.Length; n++)
        {
            bool state = false;
            for (int i = 0; i < joysticks[n].names.Length; i++)
            {
                Vector2 value = Sinput.GetVector(joysticks[n].names[i].x, joysticks[n].names[i].y,
                    joysticks[n].normalizationMode != NormalizationMode.NotNormalize);
                state = value.sqrMagnitude > 0f;

                if (state && (joysticks[n].normalizationMode == NormalizationMode.NormalizeWithoutPithagoras))
                {
                    if (Mathf.Abs(value.x) > Mathf.Abs(value.y)) value.x = Mathf.Sign(value.x);
                    else if (Mathf.Abs(value.x) < Mathf.Abs(value.y)) value.y = Mathf.Sign(value.y);
                    else { value.x = Mathf.Sign(value.x); value.y = Mathf.Sign(value.y); }
                }

                if (joysticks[n].sendWhenZeroToo || state)
                {
                    joysticks[n].JoystickValue?.Invoke(value);
                    joysticks[n].JoystickMagnitude?.Invoke(value.magnitude);
                    break;
                }
            }

            bool prevState = joysticks[n].isPressed;
            joysticks[n].SetState(state);

            if (state) { if (!prevState) joysticks[n].events.Pressed?.Invoke(); }
            else if (prevState) joysticks[n].events.Released?.Invoke();
        }

        for (int n = 0; n < buttonStates.Length; n++)
        {
            bool state = true;
            foreach (SButtonState button in buttonStates[n].buttonsState)
                if (Sinput.GetButton(button.name) != button.state)
                {
                    state = false;
                    break;
                }

            bool prevState = buttonStates[n].isPressed;
            buttonStates[n].SetState(state);

            if (state) { if (!prevState) buttonStates[n].events.Pressed?.Invoke(); }
            else if (prevState) buttonStates[n].events.Released?.Invoke();
        }
    }

    enum TimeMode { Update, Unscaled, FixedUpdate }

    enum NormalizationMode { Normalize, NotNormalize, NormalizeWithoutPithagoras }

    [Serializable]
    struct SButton
    {
        public string name;
        public ButtonEvents events;
        public bool isPressed { get; private set; }

        public SButton(string name)
        {
            this.name = name;
            events = new ButtonEvents();
            isPressed = false;
        }

        public void SetState(bool set)
        {
            isPressed = set;
        }
    }

    [Serializable]
    class FloatEvent : UnityEvent<float> { }

    [Serializable]
    struct SAxis
    {
        public string[] names;
        public ButtonEvents events;
        public bool sendWhenZeroToo;
        public FloatEvent AxisValue;
        public bool isPressed { get; private set; }

        public SAxis(string[] names, bool sendWhenZeroToo)
        {
            this.names = names;
            this.sendWhenZeroToo = sendWhenZeroToo;
            events = new ButtonEvents();
            AxisValue = null;
            isPressed = false;
        }

        public void SetState(bool set)
        {
            isPressed = set;
        }
    }

    [Serializable]
    class VectorEvent : UnityEvent<Vector2> { }

    [Serializable]
    struct SVector
    {
        public Joystick[] names;
        public ButtonEvents events;
        public NormalizationMode normalizationMode;
        public bool sendWhenZeroToo;
        public VectorEvent JoystickValue;
        public FloatEvent JoystickMagnitude;
        public bool isPressed { get; private set; }

        public SVector(Joystick[] names, NormalizationMode normalizationMode, bool sendWhenZeroToo)
        {
            this.names = names;
            this.normalizationMode = normalizationMode;
            this.sendWhenZeroToo = sendWhenZeroToo;
            events = new ButtonEvents();
            JoystickValue = null;
            JoystickMagnitude = null;
            isPressed = false;
        }

        public void SetState(bool set)
        {
            isPressed = set;
        }
    }

    [Serializable]
    struct Joystick
    {
        public string x;
        public string y;

        public Joystick(string x, string y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Serializable]
    struct SButtonsState
    {
        public SButtonState[] buttonsState;
        public ButtonEvents events;
        public bool isPressed { get; private set; }

        public SButtonsState(SButtonState[] buttonsState)
        {
            this.buttonsState = buttonsState;
            events = new ButtonEvents();
            isPressed = false;
        }

        public void SetState(bool set)
        {
            isPressed = set;
        }
    }

    [Serializable]
    struct SButtonState
    {
        public string name;
        public bool state;

        public SButtonState(string name, bool state)
        {
            this.name = name;
            this.state = state;
        }
    }

    [Serializable]
    struct ButtonEvents
    {
        public UnityEvent Pressed;
        public UnityEvent Released;

        public ButtonEvents(UnityEvent pressed, UnityEvent released)
        {
            Pressed = pressed;
            Released = released;
        }
    }
}
