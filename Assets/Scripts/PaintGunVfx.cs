using UnityEngine;
using UnityEngine.InputSystem;

public class PaintGunVfx : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem muzzleSpray;

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null || 
            muzzleSpray == null) {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            muzzleSpray.Play();
        }
    }
}
