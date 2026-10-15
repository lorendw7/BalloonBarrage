# Controller preparation

Updated: October 13, 2026. The creator selected P1 keyboard/mouse plus P2 one gamepad and owns one Xbox and one Beitong controller. Gameplay support is **not connected yet**. This guide supplies configuration and a hardware-check sequence; [game design](GAME_DESIGN.md#co-op-and-localization) defines behavior and [roadmap](ROADMAP.md) defines delivery gates.

## Prepared input asset

`Assets/Settings/Gameplay/ExhibitionControls.inputactions` contains an optional Gameplay map and two explicit control schemes. It is not assigned to the current shooter, global project actions or EventSystem. Existing solo controls and UI actions are preserved.

| Action | KeyboardMouse | Gamepad | Value |
|---|---|---|---|
| AimPointer | Mouse position | None | Screen coordinates, Vector2 |
| AimStick | None | Right stick | Direction/speed request, Vector2 |
| Fire | Left mouse button | Right trigger (RT) | Button/held state |
| Pause | Esc | Menu/Start | Button press |

KeyboardMouse requires both a keyboard and a mouse; Gamepad requires one Gamepad device. The right stick retains the Input System's built-in deadzone; no second binding deadzone is stacked on it. Stick input is a velocity request, not an absolute screen coordinate. Integrating/clamping the reticle is learner gameplay work. The asset alone cannot move a reticle, spawn a gun or fire.

For integration, each PlayerInput must use its own actions copy and explicitly paired devices. P1 pairs the keyboard/mouse; P2 pairs the pad selected for that round. Read that player's actions, not Gamepad.current or the global InputSystem.actions. Disable automatic scheme switching for fixed roles. A connected spare pad must not silently take over P2. Do not create two copies of the shared camera, RoundController, score or music.

Keep the current Input System UI Input Module and UI actions while learning gamepad gameplay. Gameplay input and menu navigation are separate. Later accept a designated menu owner's navigation/Submit/Cancel and release guards before enabling pad-driven menus; do not replace the scene UI asset with the Gameplay-only asset.

## First hardware check: no code changes

Reported status, October 13: the creator says the Beitong check has no issue. The exact model, Unity device/layout name and connection mode were not supplied. Treat this as user-reported hardware preparation, not accepted in-game aiming/firing or co-op. Xbox checking and simultaneous-device gameplay acceptance have not been reported.

1. Save the current scene. Connect only the Xbox pad first, preferably by USB for the first check.
2. Open **Window > Analysis > Input Debugger** in Unity. Expand **Devices**, then double-click the Xbox/XInput device.
3. Move the right stick: confirm rightStick/x and rightStick/y change and settle close to zero. Squeeze RT: confirm rightTrigger changes from near zero to near one. Press Menu: confirm start changes.
4. Record the exact layout/device name and whether these three controls respond. This checks recognition only; the current game still reads the mouse.
5. Disconnect Xbox and repeat with Beitong. If the model offers a Windows/XInput mode, choose it using that model's manufacturer instructions; no model-specific shortcut is assumed. Generic Joystick/HID recognition alone does not prove the Gamepad bindings match.
6. Connect both and confirm two distinct device entries. Do not pair based solely on arrival order or whichever pad was used most recently.

The installed Input System is 1.19.0. Its local package documentation is the primary version reference: `Library/PackageCache/com.unity.inputsystem@21a28c3a6c83/Documentation~/Gamepad.md`, `PlayerInput.md`, `Debugging.md` and `Testing.md`. Online references: [Unity Input Debugger](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/the-input-debugger-window.md), [Unity device support](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/supported-devices-reference.md) and [PlayerInput](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/PlayerInput.html).

## Integration and acceptance sequence

1. Complete the solo rhythm checkpoint. Teach one-player pad reticle movement and firing with the supplied actions; verify neutral drift, clamped aiming, cadence and release-after-menu.
2. Add P2 in the shared scene with its own reticle/shooter state and explicit player identity on shots. Preserve P1 mouse behavior and share existing rhythm/round state.
3. Accept mouse and pad simultaneously; the spare pad affects neither player. Verify different-distance chart-event judgement and one bonus per player per event.
4. Both players hit the same balloon: score resolves once. Pause freezes timer, audio, motion and shots. Resuming while either fire control is held requires that player's release.
5. Disconnect the paired pad during play: pause with a readable reason; reconnect without a ghost shot, or return to title and choose solo. Retry clears player/beat/effect state.
6. Repeat with the alternate controller and on the exhibition Windows PC. Test actual audio, focus, menus and a complete song round; synthetic input checks cannot accept physical controller compatibility.

The Editor test `ExhibitionControlsTests` uses isolated synthetic devices to check action filtering, trigger release, independent mouse/stick values, small-stick drift and the spare pad. See [Validation](../Project/VALIDATION.md) for the executed result and remaining checks. The tests do not implement co-op gameplay.
