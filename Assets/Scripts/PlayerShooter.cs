using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerShooter : MonoBehaviour
{

    [SerializeField] private Transform muzzle;

    [SerializeField] private PaintProjectile paintProjectilePrefab;

    [SerializeField] private ParticleSystem muzzleSpray;

    [SerializeField] private AudioSource shotAudioSource;

    [SerializeField, Min(0.1f)]
    private float shotsPerSecond = 5f;

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

    private void Awake()
    {
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

    private void LateUpdate()
    {
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
            return;
        }

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

        if (roundController == null || muzzle == null || paintProjectilePrefab == null)
        {
            Debug.LogWarning("Connect RoundController, Muzzle and projectile prefab.");
            return;
        }

        Vector3 direction = (aimPoint - muzzle.position).normalized;

        double shotTime = roundController.SongTimeSeconds;

        Balloon targetBalloon = null;

        if (hit.collider != null)
        {
            targetBalloon = hit.collider.GetComponentInParent<Balloon>();
        }

        bool isOnBeat = targetBalloon != null &&
            targetBalloon.IsOnBeat(shotTime);
        Debug.Log($"Shot at {shotTime:F3}s; on beat: {isOnBeat}.");

        PaintProjectile projectile = Instantiate(
            paintProjectilePrefab,
            muzzle.position,
            Quaternion.LookRotation(direction)
            );

        int targetEventId = -1;

        if (targetBalloon != null)
        {
            targetEventId = targetBalloon.EventId;
        }

        projectile.InitializeShot(isOnBeat, targetEventId);

        GetComponent<StudioGunAppearance>()?.ColorizeShot(projectile.gameObject);

        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);

        KickGun();
        if (shotAudioSource != null)
        {
            shotAudioSource.Play();
        }
        if (muzzleSpray != null)
        {
            muzzleSpray.Play();
        }
    }
}
