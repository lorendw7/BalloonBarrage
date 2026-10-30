using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public enum LocalPlayerLayout { MouseSolo, GamepadSolo, MouseAndGamepad, TwoGamepads }

/// <summary>Local device ownership and cursor transport; shared song/score remain in RoundController.</summary>
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class LocalPlayerInput : MonoBehaviour
{
    public LocalPlayerLayout Layout { get; private set; }
    public bool IsCoop => (int)Layout >= (int)LocalPlayerLayout.MouseAndGamepad;
    public PlayerShooter Primary { get; private set; }
    public PlayerShooter Secondary { get; private set; }
    public int LastAimingPlayer { get; private set; }
    private RoundController round;
    private readonly Gamepad[] assigned = new Gamepad[2];
    private readonly Vector2[] aims = { new Vector2(.4f, .5f), new Vector2(.6f, .5f) };
    private readonly List<Gamepad> available = new List<Gamepad>(4);
    private bool prepared;
    private bool hasFocus;
    private bool suspended;
    private bool choosing;
    private GameObject reconnectRoot;
    private Text reconnectText;
    private LocalizedText fontTemplate;

    public void Configure(RoundController owner, PlayerShooter primary)
    {
        round = owner; Primary = primary; hasFocus = Application.isFocused;
        Primary.ConfigureLocalPlayer(this, 0);
        Secondary = Primary.CreateSecondLocalPlayer(this);
        Secondary.gameObject.SetActive(false);
        EventSystem system = EventSystem.current;
        if (system != null && system.GetComponent<InputSystemUIInputModule>() == null)
        {
            BaseInputModule old = system.GetComponent<BaseInputModule>();
            if (old != null) old.enabled = false;
            system.gameObject.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }
    }

    public bool UsesGamepad(int player) => player == 1 || Layout == LocalPlayerLayout.GamepadSolo || Layout == LocalPlayerLayout.TwoGamepads;
    public int RequiredGamepads => Layout == LocalPlayerLayout.TwoGamepads ? 2 : Layout == LocalPlayerLayout.MouseSolo ? 0 : 1;
    public int AvailableGamepads
    {
        get { RefreshAvailable(); return available.Count; }
    }
    public bool CanStart => AvailableGamepads >= RequiredGamepads;
    public bool DevicesReady
    {
        get
        {
            if (!prepared) return CanStart;
            for (int player = 0; player < (IsCoop ? 2 : 1); player++)
                if (UsesGamepad(player) && !Connected(assigned[player])) return false;
            return true;
        }
    }
    public int AssignedDeviceId(int player) => assigned[player]?.deviceId ?? -1;
    public Vector2 ViewportAim(int player) => aims[player];
    public void SelectLayout(int value)
    {
        if (RoundController.IsPlaying) return;
        Layout = (LocalPlayerLayout)Mathf.Clamp(value, 0, 3);
        prepared = false; assigned[0] = assigned[1] = null;
        Primary?.SuspendLocalInput(); Secondary?.SuspendLocalInput();
        if (Secondary != null) Secondary.gameObject.SetActive(false);
    }
    public bool PrepareRound()
    {
        RefreshAvailable();
        if (available.Count < RequiredGamepads) return false;
        assigned[0] = UsesGamepad(0) ? available[0] : null;
        assigned[1] = IsCoop ? available[Layout == LocalPlayerLayout.TwoGamepads ? 1 : 0] : null;
        aims[0] = new Vector2(.4f, .5f); aims[1] = new Vector2(.6f, .5f);
        LastAimingPlayer = 0; prepared = true;
        Primary.SuspendLocalInput(); Secondary.SuspendLocalInput();
        Secondary.gameObject.SetActive(IsCoop);
        return true;
    }
    private static bool Connected(Gamepad pad) => pad != null && pad.added && pad.enabled;
    private void RefreshAvailable()
    {
        available.Clear();
        foreach (Gamepad pad in Gamepad.all) if (Connected(pad)) available.Add(pad);
        available.Sort((a, b) => a.deviceId.CompareTo(b.deviceId));
    }
    public bool ReadGamepad(int player, out Vector2 screenPosition, out bool fire, out bool cycle)
    {
        screenPosition = new Vector2(aims[player].x * Screen.width, aims[player].y * Screen.height);
        fire = cycle = false;
        Gamepad pad = assigned[player];
        if (!prepared || !Connected(pad) || !hasFocus || suspended || (player == 1 && !IsCoop)) return false;
        fire = pad.rightTrigger.isPressed;
        cycle = pad.buttonWest.wasPressedThisFrame;
        return true;
    }
    private void OnEnable() { InputSystem.onDeviceChange += DeviceChanged; hasFocus = Application.isFocused; }
    private void OnDisable() { InputSystem.onDeviceChange -= DeviceChanged; }
    private void OnApplicationFocus(bool focused)
    {
        hasFocus = focused;
        Primary?.SuspendLocalInput(); Secondary?.SuspendLocalInput();
    }
    private void OnApplicationPause(bool paused)
    {
        suspended = paused;
        Primary?.SuspendLocalInput(); Secondary?.SuspendLocalInput();
    }
    private void DeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (!prepared || (device != assigned[0] && device != assigned[1])) return;
        if (!DevicesReady)
        {
            Primary?.SuspendLocalInput(); Secondary?.SuspendLocalInput();
            round?.PauseForControllerLoss();
        }
    }
    private void Update()
    {
        if (round == null || !hasFocus || suspended) return;
        if (!RoundController.IsPlaying)
        {
            foreach (Gamepad pad in Gamepad.all)
                if (Connected(pad) && pad.buttonEast.wasPressedThisFrame && round.IsChoosingSong)
                {
                    GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
                    bool expanded = selected != null && selected.GetComponent<Toggle>() != null;
                    if (!expanded) round.CancelSongSelection();
                }
            // Default UI actions provide d-pad/stick navigation, South submit and East cancel.
            bool gamepadNavigation = false;
            foreach (Gamepad pad in Gamepad.all)
                gamepadNavigation |= Connected(pad) && (pad.buttonSouth.wasPressedThisFrame || pad.dpad.ReadValue().sqrMagnitude > .1f);
            if (gamepadNavigation && EventSystem.current != null &&
                (EventSystem.current.currentSelectedGameObject == null || choosing != round.IsChoosingSong))
                EventSystem.current.SetSelectedGameObject(GameObject.Find(round.IsChoosingSong ? "ConfirmSongButton" : "StartButton"));
            choosing = round.IsChoosingSong;
            return;
        }
        if (!DevicesReady) { round.PauseForControllerLoss(); return; }
        bool pausePressed = false;
        foreach (Gamepad pad in assigned) pausePressed |= Connected(pad) && pad.startButton.wasPressedThisFrame;
        if (pausePressed) round.TogglePauseFromController();
        if (RoundController.IsPaused) return;
        for (int player = 0; player < (IsCoop ? 2 : 1); player++)
        {
            Gamepad pad = assigned[player];
            if (!UsesGamepad(player) || !Connected(pad)) continue;
            Vector2 stick = pad.rightStick.ReadValue();
            if (stick.sqrMagnitude < .02f) stick = pad.leftStick.ReadValue();
            if (stick.sqrMagnitude < .02f) continue;
            Vector2 pixels = Vector2.ClampMagnitude(stick, 1f) * (800f * Time.unscaledDeltaTime);
            aims[player] += new Vector2(pixels.x / Mathf.Max(1, Screen.width), pixels.y / Mathf.Max(1, Screen.height));
            aims[player] = new Vector2(Mathf.Clamp(aims[player].x, .04f, .96f), Mathf.Clamp(aims[player].y, .04f, .96f));
            LastAimingPlayer = player;
        }
        if (!UsesGamepad(0) && Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f) LastAimingPlayer = 0;
    }
    private void LateUpdate()
    {
        bool show = RoundController.IsPlaying && RoundController.IsPaused && !DevicesReady;
        if (show && reconnectRoot == null)
        {
            reconnectRoot = new GameObject("ControllerReconnectOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            reconnectRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            reconnectRoot.GetComponent<Canvas>().sortingOrder = 40;
            var scaler = reconnectRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = .5f;
            var label = new GameObject("ReconnectHint", typeof(RectTransform), typeof(Text), typeof(Shadow));
            label.transform.SetParent(reconnectRoot.transform, false);
            reconnectText = label.GetComponent<Text>();
            fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
            reconnectText.fontSize = 22; reconnectText.alignment = TextAnchor.MiddleCenter;
            reconnectText.color = new Color(1f, .95f, .75f); reconnectText.raycastTarget = false;
            reconnectText.rectTransform.anchorMin = reconnectText.rectTransform.anchorMax = new Vector2(.5f, .76f);
            reconnectText.rectTransform.sizeDelta = new Vector2(1000f, 64f);
        }
        if (reconnectRoot == null) return;
        reconnectRoot.SetActive(show);
        if (show)
        {
            reconnectText.font = fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            reconnectText.text = LocalizationManager.GetText("players.reconnect");
        }
    }
    private void OnDestroy() { if (reconnectRoot != null) Destroy(reconnectRoot); }
}
