using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerShooter : MonoBehaviour
{
    // Cosmetic color selection must not alter Unity's gameplay random stream.
    private static readonly System.Random sprayPigmentRandom =
        new System.Random();

    [SerializeField] private Transform muzzle;

    [SerializeField] private PaintProjectile paintProjectilePrefab;

    [SerializeField] private ParticleSystem muzzleSpray;

    [SerializeField] private AudioSource shotAudioSource;

    [SerializeField, Min(0.1f)]
    private float shotsPerSecond = 5f;

    [SerializeField, Min(0f)]
    private float sprayRadius = 0.3f;

    [SerializeField, Min(0.1f)]
    private float sprayRange = 30f;

    private int sprayPigmentIndex;

    private float nextShotTime;

    private bool waitingForFireRelease = true;

    [SerializeField]
    private RoundController roundController;

    [SerializeField]
    private RectTransform aimReticle;

    [SerializeField]
    private Transform gunVisual;

    private Vector3 gunRestLocalPosition;

    private Quaternion gunRestLocalRotation;

    private void SetSprayVisual(bool shouldPlay)
    {
        if (muzzleSpray == null)
        {
            return;
        }


        if(shouldPlay && !muzzleSpray.isPlaying)
        {
            muzzleSpray.Play();
        }
        else if (!shouldPlay && muzzleSpray.isPlaying)
        {
            muzzleSpray.Stop(
                true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private bool TryGetSprayHit(
        Vector3 direction,
        out RaycastHit sprayHit)
    {
        return Physics.SphereCast(
            muzzle.position,
            sprayRadius,
            direction,
            out sprayHit,
            sprayRange,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void ConfigureSprayPigment()
    {
        if (paintProjectilePrefab == null)
        {
            return;
        }

        int pigmentCount = paintProjectilePrefab.PigmentCount;

        if (pigmentCount <= 0)
        {
            return;
        }

        sprayPigmentIndex = sprayPigmentRandom.Next(pigmentCount);

        if (muzzleSpray != null)
        {
            ParticleSystem.MainModule main = muzzleSpray.main;
            main.startColor = paintProjectilePrefab.GetPigmentColor(
                sprayPigmentIndex);
        }
    }


    private void Awake()
    {
        ConfigureSprayPigment();

        if (gunVisual == null)
        {
            return;
        }

        gunRestLocalPosition = gunVisual.localPosition;
        gunRestLocalRotation = gunVisual.localRotation;
    }

    private void RestorePointer()
    {
        Cursor.visible = true;

        if (aimReticle != null)
        {
            aimReticle.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        SetSprayVisual(false);
        ResetGunPose();
        RestorePointer();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus)
        {
            RestorePointer() ;
        }
    }

    private void KickGun()
    {
        if (gunVisual == null)
        {
            return;
        }

        gunVisual.localPosition = gunRestLocalPosition + Vector3.back * 0.035f;

        gunVisual.localRotation = gunRestLocalRotation * Quaternion.Euler(-6f, 0f, 0f);

    
    }

    private void ResetGunPose()
    {
        if(gunVisual == null)
        {  return; }

        gunVisual.localPosition = gunRestLocalPosition;
        gunVisual.localRotation = gunRestLocalRotation;
    }

    private void LateUpdate()
    {

        if (!RoundController.IsPlaying)
        {
            ResetGunPose();
            return;
        }

        if (gunVisual == null ||
            RoundController.IsPaused) {
            return;
        }

        float t = 1f - Mathf.Exp(-18f * Time.deltaTime);

        gunVisual.localPosition = Vector3.Lerp(
            gunVisual.localPosition,
            gunRestLocalPosition,
            t);

        gunVisual.localRotation = Quaternion.Slerp(
            gunVisual.localRotation,
            gunRestLocalRotation, t);

    }
    // Update is called once per frame
    void Update()
    {
        bool showReticle = Mouse.current != null &&
            RoundController.IsPlaying &&
            !RoundController.IsPaused &&
            Application.isFocused;

        Cursor.visible = !showReticle;

        if (aimReticle != null)
        {
            
            aimReticle.gameObject.SetActive(showReticle);

            if (showReticle )
            {
                aimReticle.position =
                    Mouse.current.position.ReadValue();
            }
        }


        if (Mouse.current == null)
        {
            SetSprayVisual(false);
            return;
        }

        bool shouldSpray =
        RoundController.IsPlaying &&
        !RoundController.IsPaused &&
        !waitingForFireRelease &&
        Mouse.current.leftButton.isPressed &&
        (EventSystem.current == null ||
         !EventSystem.current.IsPointerOverGameObject());

        SetSprayVisual(shouldSpray);

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Ray aimRay = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue());

        Vector3 aimPoint = aimRay.GetPoint(20f);

        if (Physics.Raycast(
            aimRay,
            out RaycastHit hit,
            100f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
        }

        transform.LookAt(aimPoint);

        if (!RoundController.IsPlaying ||
            RoundController.IsPaused)
        {
            waitingForFireRelease = true;
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            waitingForFireRelease = true;
            return;
        }

        if (waitingForFireRelease)
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                waitingForFireRelease = false;
            }

            return;
        }

        if (!Mouse.current.leftButton.isPressed)
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        if (roundController == null || muzzle == null)
        {
            Debug.LogWarning(
                "Connect RoundController and Muzzle.");
            return;
        }

        Vector3 direction = (aimPoint - muzzle.position).normalized;

        double shotTime = roundController.SongTimeSeconds;

        if (TryGetSprayHit(direction, out RaycastHit sprayHit))
        {
            Balloon sprayedBalloon = sprayHit.collider.GetComponentInParent<Balloon>();

            if (sprayedBalloon != null)
            {
                bool isOnBeat = sprayedBalloon.IsOnBeat(shotTime);

                int bonusPoints = isOnBeat ? 10 : 0;

                sprayedBalloon.Hit(bonusPoints);
            }
            else if (paintProjectilePrefab != null)
            {
                paintProjectilePrefab.SpawnPaintMark(
                    sprayHit,
                    sprayPigmentIndex);
            }
        }

        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);

        KickGun();
        if (shotAudioSource != null)
        {
            shotAudioSource.Play();
        }
    }
}
