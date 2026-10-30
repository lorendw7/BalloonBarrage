using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform muzzle;

    [SerializeField] private PaintProjectile paintProjectilePrefab;

    [SerializeField] private ParticleSystem muzzleSpray;

    [SerializeField]
    private ParticleSystem impactSprayPrefab;

    [SerializeField] private AudioSource shotAudioSource;

    [SerializeField, Min(0.1f)]
    private float shotsPerSecond = 5f;

    [SerializeField, Min(0f)]
    private float sprayRadius = 0.3f;

    [SerializeField, Min(0.1f)]
    private float sprayRange = 30f;

    [SerializeField, Min(1f)]
    private float sprayVisualSpeed = 32f;

    [SerializeField, Min(0f)]
    private float sprayVisualEndpointInset = 0.12f;

    [SerializeField]
    private int sprayVisualSortingOrder = 10000;

    [SerializeField, Min(0)]
    private int sprayPigmentIndex = 0;

    private ParticleSystem.Particle[] sprayParticleBuffer;

    private Camera aimCamera;

    private bool sprayVisualActive;

    private float lastSprayVisualLength = -1f;

    private float lastSprayLifetime = -1f;

    private bool reticleVisible;

    private bool cursorVisible;

    private float nextShotTime;

    private bool waitingForFireRelease = true;
    private bool hasInputFocus;
    private bool applicationSuspended;
    private bool aimValid;
    private Vector3 lastAimDirection;
    private LocalPlayerInput localPlayers;
    public int LocalPlayerIndex { get; private set; }
    public int ShotCount { get; private set; }
    public void ConfigureLocalPlayer(LocalPlayerInput owner, int index) { localPlayers = owner; LocalPlayerIndex = index; }
    public void SuspendLocalInput() { CancelFireInput(); }

    public PlayerShooter CreateSecondLocalPlayer(LocalPlayerInput owner)
    {
        var go = new GameObject("SecondLocalPlayerShooter");
        go.transform.SetParent(Camera.main.transform, false);
        go.transform.localPosition = new Vector3(-.5f, -.4f, .7f);
        var second = go.AddComponent<PlayerShooter>();
        second.ConfigureLocalPlayer(owner, 1);
        second.roundController = roundController;
        second.muzzle = go.transform;
        second.paintProjectilePrefab = paintProjectilePrefab;
        second.impactSprayPrefab = impactSprayPrefab;
        second.shotsPerSecond = shotsPerSecond; second.sprayRadius = sprayRadius; second.sprayRange = sprayRange;
        second.sprayPigmentIndex = 1;
        if (muzzleSpray != null)
        {
            second.muzzleSpray = Instantiate(muzzleSpray, go.transform);
            second.muzzleSpray.transform.localPosition = Vector3.zero;
            second.muzzleSpray.transform.localRotation = Quaternion.identity;
            second.ConfigureSprayPresentation();
        }
        if (shotAudioSource != null)
        {
            second.shotAudioSource = go.AddComponent<AudioSource>();
            second.shotAudioSource.clip = shotAudioSource.clip;
            second.shotAudioSource.volume = shotAudioSource.volume;
            second.shotAudioSource.playOnAwake = false;
            second.shotAudioSource.outputAudioMixerGroup = shotAudioSource.outputAudioMixerGroup;
        }
        if (aimReticle != null)
        {
            var label = new GameObject("LocalPlayerNumber", typeof(RectTransform), typeof(UnityEngine.UI.Text));
            label.transform.SetParent(aimReticle, false);
            var text = label.GetComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 18; text.text = "1"; text.color = Color.white; text.raycastTarget = false;
            text.rectTransform.anchoredPosition = new Vector2(24f, 20f);
            text.rectTransform.sizeDelta = new Vector2(28f, 28f);
            second.aimReticle = Instantiate(aimReticle, aimReticle.parent);
            second.aimReticle.name = "SecondPlayerReticle";
            second.aimReticle.GetComponentInChildren<UnityEngine.UI.Text>(true).text = "2";
            foreach (var image in second.aimReticle.GetComponentsInChildren<UnityEngine.UI.Image>(true))
                image.color = new Color(.3f, .9f, 1f, 1f);
        }
        second.ConfigureSprayPigment();
        return second;
    }


    public bool TryGetAimedBalloon(out Balloon target)
    {
        target = null;
        if (!aimValid || !RoundController.IsPlaying || RoundController.IsPaused || muzzle == null) return false;
        if (TryGetSprayHit(lastAimDirection, out RaycastHit hit))
            target = hit.collider.GetComponentInParent<Balloon>();
        return target != null;
    }

    [SerializeField]
    private RoundController roundController;

    [SerializeField]
    private RectTransform aimReticle;

    [SerializeField]
    private Transform gunVisual;

    [SerializeField]
    private Renderer paintCanisterRenderer;

    private Vector3 gunRestLocalPosition;

    private Quaternion gunRestLocalRotation;


    private void SetSprayVisual(
        bool shouldPlay,
        float hitDistance = 0f)
    {
        if (!shouldPlay) RoomPaintSystem.EndStroke(GetInstanceID());
        if (muzzleSpray == null)
        {
            return;
        }

        if (!shouldPlay)
        {
            if (sprayVisualActive)
            {
                muzzleSpray.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            sprayVisualActive = false;
            lastSprayVisualLength = -1f;
            lastSprayLifetime = -1f;
            return;
        }

        float visualLength = Mathf.Max(
            0.05f,
            hitDistance - sprayVisualEndpointInset);

        float lifetime = visualLength /
            Mathf.Max(1f, sprayVisualSpeed);

        if (Mathf.Abs(lifetime - lastSprayLifetime) > 0.0005f)
        {
            ParticleSystem.MainModule main = muzzleSpray.main;
            main.startLifetime = lifetime;
            lastSprayLifetime = lifetime;
        }

        if (!sprayVisualActive)
        {
            muzzleSpray.Play();
            sprayVisualActive = true;
        }

        if (lastSprayVisualLength >= 0f &&
            visualLength < lastSprayVisualLength - 0.01f)
        {
            TrimSprayParticles(visualLength);
        }

        lastSprayVisualLength = visualLength;
    }

    private void TrimSprayParticles(float visualLength)
    {
        if (sprayParticleBuffer == null ||
            sprayParticleBuffer.Length == 0)
        {
            return;
        }

        int particleCount = muzzleSpray.GetParticles(
            sprayParticleBuffer);

        bool changed = false;

        for (int index = 0; index < particleCount; index++)
        {
            if (sprayParticleBuffer[index].position.z <= visualLength)
            {
                continue;
            }

            sprayParticleBuffer[index].remainingLifetime = 0f;
            changed = true;
        }

        if (changed)
        {
            muzzleSpray.SetParticles(
                sprayParticleBuffer,
                particleCount);
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

        sprayPigmentIndex = Mathf.Clamp(
            sprayPigmentIndex,
            0,
            pigmentCount - 1);

        Color selectedColor = paintProjectilePrefab.GetPigmentColor(sprayPigmentIndex);

        if (muzzleSpray != null)
        {
            ParticleSystem.MainModule main = muzzleSpray.main;
            main.startColor = selectedColor;
        }

        if (paintCanisterRenderer != null)
        {
            Material canisterMaterial = paintCanisterRenderer.material;

            if (canisterMaterial.HasProperty("_BaseColor"))
            {
                canisterMaterial.SetColor("_BaseColor", selectedColor);
            }
        }
    }

    private void SelectNextPigment()
    {
        if (paintProjectilePrefab == null ||
            paintProjectilePrefab.PigmentCount <= 0)
        {
            return;
        }

        sprayPigmentIndex =
            (sprayPigmentIndex + 1) %
            paintProjectilePrefab.PigmentCount;

        ConfigureSprayPigment();
    }

    private void PlayImpactSpray(RaycastHit hit)
    {
        if(impactSprayPrefab == null)
        {
            return;
        }


        Color impactColor = Color.white;

        if (paintProjectilePrefab != null)
        {
            impactColor = paintProjectilePrefab.GetPigmentColor(sprayPigmentIndex);
        }

        PaintImpactPool.Play(impactSprayPrefab, hit.point, hit.normal, impactColor);
    }


    private void ConfigureSprayPresentation()
    {
        if (muzzleSpray == null)
        {
            return;
        }

        ParticleSystem.MainModule main = muzzleSpray.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startSpeed = sprayVisualSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(.12f, .23f);
        var shape = muzzleSpray.shape;
        if (shape.shapeType == ParticleSystemShapeType.Cone) shape.angle = 1.1f;

        ParticleSystem.CollisionModule collision =
            muzzleSpray.collision;
        collision.enabled = false;

        ParticleSystemRenderer sprayRenderer =
            muzzleSpray.GetComponent<ParticleSystemRenderer>();

        if (sprayRenderer != null)
        {
            sprayRenderer.sortingOrder = sprayVisualSortingOrder;
        }

        sprayParticleBuffer = new ParticleSystem.Particle[
            Mathf.Max(1, main.maxParticles)];
    }


    private void Awake()
    {
        hasInputFocus = Application.isFocused;
        aimCamera = Camera.main;
        cursorVisible = Cursor.visible;

        if (aimReticle != null)
        {
            reticleVisible = aimReticle.gameObject.activeSelf;
        }

        ConfigureSprayPresentation();
        ConfigureSprayPigment();
        PaintImpactPool.Warm(impactSprayPrefab);

        if (gunVisual == null)
        {
            return;
        }

        gunRestLocalPosition = gunVisual.localPosition;
        gunRestLocalRotation = gunVisual.localRotation;
    }

    private void RestorePointer()
    {
        if (LocalPlayerIndex == 0 && !cursorVisible)
        {
            Cursor.visible = true;
            cursorVisible = true;
        }

        if (aimReticle != null)
        {
            aimReticle.gameObject.SetActive(false);
            reticleVisible = false;
        }
    }

    private void OnEnable()
    {
        hasInputFocus = Application.isFocused;
        waitingForFireRelease = true;
    }

    private void CancelFireInput()
    {
        RoomPaintSystem.EndStroke(GetInstanceID());
        aimValid = false;
        waitingForFireRelease = true;
        SetSprayVisual(false);
        if (shotAudioSource != null) shotAudioSource.Stop();
        ResetGunPose();
        RestorePointer();
    }

    private void OnDisable()
    {
        CancelFireInput();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        hasInputFocus = hasFocus;
        waitingForFireRelease = true;
        if (!hasFocus) CancelFireInput();
    }

    private void OnApplicationPause(bool paused)
    {
        applicationSuspended = paused;
        waitingForFireRelease = true;
        if (paused) CancelFireInput();
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

        if (!RoundController.IsPlaying || !hasInputFocus || applicationSuspended)
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
        aimValid = false;
        if (!hasInputFocus || applicationSuspended)
        {
            CancelFireInput();
            return;
        }

        Mouse mouse = Mouse.current;
        Keyboard keyboard = Keyboard.current;
        bool routed = localPlayers != null && localPlayers.UsesGamepad(LocalPlayerIndex);
        Vector2 pointerPosition = mouse != null ? mouse.position.ReadValue() : default;
        bool fireHeld = mouse != null && mouse.leftButton.isPressed;
        bool cycle = keyboard != null && keyboard.cKey.wasPressedThisFrame;
        bool inputAvailable = mouse != null;
        if (routed) inputAvailable = localPlayers.ReadGamepad(LocalPlayerIndex, out pointerPosition, out fireHeld, out cycle);
        if (cycle && RoundController.IsPlaying && !RoundController.IsPaused) SelectNextPigment();
        bool showReticle = inputAvailable && RoundController.IsPlaying && !RoundController.IsPaused &&
            hasInputFocus && !applicationSuspended;

        bool shouldShowCursor = !showReticle;

        if (LocalPlayerIndex == 0 && cursorVisible != shouldShowCursor)
        {
            Cursor.visible = shouldShowCursor;
            cursorVisible = shouldShowCursor;
        }

        if (aimReticle != null)
        {
            if (reticleVisible != showReticle)
            {
                aimReticle.gameObject.SetActive(showReticle);
                reticleVisible = showReticle;
            }

            if (showReticle )
            {
                aimReticle.position =
                    pointerPosition;
            }
        }


        if (!inputAvailable)
        {
            CancelFireInput();
            return;
        }

        EventSystem eventSystem = EventSystem.current;

        bool pointerOverUi = !routed && eventSystem != null &&
            eventSystem.IsPointerOverGameObject();

        bool shouldSpray =
        RoundController.IsPlaying &&
        !RoundController.IsPaused &&
        !waitingForFireRelease &&
        fireHeld &&
        !pointerOverUi;

        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        if (aimCamera == null)
        {
            SetSprayVisual(false);
            return;
        }

        Ray aimRay = aimCamera.ScreenPointToRay(pointerPosition);

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
        if (muzzle != null)
        {
            lastAimDirection = (aimPoint - muzzle.position).normalized;
            aimValid = RoundController.IsPlaying && !RoundController.IsPaused && !pointerOverUi;
        }

        if (shouldSpray && muzzle != null)
        {
            float visualDistance = Mathf.Min(
                sprayRange,
                Vector3.Distance(muzzle.position, aimPoint));

            SetSprayVisual(true, visualDistance);
        }
        else
        {
            SetSprayVisual(false);
        }

        if (!RoundController.IsPlaying ||
            RoundController.IsPaused)
        {
            waitingForFireRelease = true;
            return;
        }

        if (pointerOverUi)
        {
            waitingForFireRelease = true;
            return;
        }

        if (waitingForFireRelease)
        {
            if (!fireHeld)
            {
                waitingForFireRelease = false;
            }

            return;
        }

        if (!fireHeld)
        {
            return;
        }

        if (LocalPlayerIndex == 1 ? !CoopLearnerCore.IsSecondShotReady(Time.time, nextShotTime) : Time.time < nextShotTime)
        {
            return;
        }

        if (roundController == null || muzzle == null)
        {
            Debug.LogWarning(
                "Connect RoundController and Muzzle.");
            return;
        }

        Vector3 direction =
            (aimPoint - muzzle.position).normalized;

        double shotTime = roundController.SongTimeSeconds;

        if (TryGetSprayHit(direction, out RaycastHit sprayHit))
        {
            Balloon sprayedBalloon = sprayHit.collider.GetComponentInParent<Balloon>();

            if (sprayedBalloon != null)
            {
                bool isOnBeat = sprayedBalloon.IsOnBeat(shotTime);

                int bonusPoints = LocalPlayerIndex == 1
                    ? Mathf.Clamp(CoopLearnerCore.SecondPlayerBeatBonus(isOnBeat), 0, 10)
                    : isOnBeat ? 10 : 0;

                RainbowChain chain = roundController.GetComponent<RainbowChain>();
                int burstsBefore = chain != null ? chain.BurstCount : 0;
                int hitsBefore = Score.SuccessfulHits;
                sprayedBalloon.HitAt(
                    sprayHit.point,
                    bonusPoints);
                if (Score.SuccessfulHits > hitsBefore)
                    localPlayers?.NotifyHit(LocalPlayerIndex, isOnBeat,
                        chain != null && chain.BurstCount > burstsBefore && chain.LastNeighborCount > 0);
            }
            else if (paintProjectilePrefab != null)
            {
                paintProjectilePrefab.SpawnPaintMark(
                    sprayHit,
                    sprayPigmentIndex, GetInstanceID());

                PlayImpactSpray(sprayHit);
            }
        }

        ShotCount++;
        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);

        KickGun();
        if (shotAudioSource != null)
        {
            shotAudioSource.Play();
        }
    }
}
