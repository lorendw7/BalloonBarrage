using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace BalloonBarrage.Foundation.Tests
{
    public sealed class ExhibitionControlsTests : InputTestFixture
    {
        [Test]
        public void PairedMapsKeepMouseAndTwoGamepadsIndependent()
        {
            var saved = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                "Assets/Settings/Gameplay/ExhibitionControls.inputactions");
            Assert.That(saved, Is.Not.Null);
            var p1 = Object.Instantiate(saved);
            var p2 = Object.Instantiate(saved);
            var mouse = InputSystem.AddDevice<Mouse>();
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var paired = InputSystem.AddDevice<Gamepad>();
            var spare = InputSystem.AddDevice<Gamepad>();
            try
            {
                p1.devices = new InputDevice[] { keyboard, mouse };
                p1.bindingMask = InputBinding.MaskByGroup("KeyboardMouse");
                p2.devices = new InputDevice[] { paired };
                p2.bindingMask = InputBinding.MaskByGroup("Gamepad");
                p1.Enable(); p2.Enable();
                InputSystem.Update();

                InputSystem.QueueStateEvent(spare, new GamepadState { rightTrigger = 1f, rightStick = Vector2.one });
                InputSystem.Update();
                Assert.That(p1.FindAction("Gameplay/Fire").IsPressed(), Is.False);
                Assert.That(p2.FindAction("Gameplay/Fire").IsPressed(), Is.False,
                    "The unpaired backup pad must not control P2");

                InputSystem.QueueStateEvent(paired, new GamepadState { rightTrigger = 1f, rightStick = new Vector2(.04f, 0f) });
                InputSystem.Update();
                Assert.That(p2.FindAction("Gameplay/Fire").IsPressed(), Is.True);
                Assert.That(p1.FindAction("Gameplay/Fire").IsPressed(), Is.False);
                Assert.That(p2.FindAction("Gameplay/AimStick").ReadValue<Vector2>(), Is.EqualTo(Vector2.zero),
                    "Small stick drift should not move the reticle");

                InputSystem.QueueStateEvent(paired, new GamepadState { rightStick = new Vector2(.8f, 0f) });
                InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(200f, 100f), buttons = 1 });
                InputSystem.Update();
                Assert.That(p2.FindAction("Gameplay/Fire").IsPressed(), Is.False);
                Assert.That(p1.FindAction("Gameplay/Fire").IsPressed(), Is.True);
                Assert.That(p1.FindAction("Gameplay/AimPointer").ReadValue<Vector2>(), Is.EqualTo(new Vector2(200f, 100f)));
                Assert.That(p2.FindAction("Gameplay/AimStick").ReadValue<Vector2>().x, Is.GreaterThan(.5f));
                Assert.That(p1.FindAction("Gameplay/AimStick").ReadValue<Vector2>(), Is.EqualTo(Vector2.zero));

                InputSystem.QueueStateEvent(paired, new GamepadState().WithButton(GamepadButton.Start));
                InputSystem.Update();
                Assert.That(p2.FindAction("Gameplay/Pause").IsPressed(), Is.True);
                Assert.That(p1.FindAction("Gameplay/Pause").IsPressed(), Is.False);
            }
            finally
            {
                p1.Disable(); p2.Disable();
                Object.DestroyImmediate(p1); Object.DestroyImmediate(p2);
                InputSystem.RemoveDevice(mouse); InputSystem.RemoveDevice(keyboard);
                InputSystem.RemoveDevice(paired); InputSystem.RemoveDevice(spare);
            }
        }
    }
}
