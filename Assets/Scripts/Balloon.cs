using UnityEngine;

public class Balloon : MonoBehaviour
{
    private static readonly Vector3[] PaintDirections =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };

    public static event System.Action<int> OnBeatHit;
    public static event System.Action<int> OnResolved;
    public static event System.Action<int, Color> OnPigmentPopped;
    public static event System.Action<Balloon> OnDirectPop;


    [SerializeField]
    public int points = 10;

    [SerializeField]
    private AudioClip popSound;

    [SerializeField, Range(0f, 1f)]
    private float popVolume = 0.5f;

    private bool isPoping;
    private bool finaleAccent;
    public bool IsFinaleAccent => finaleAccent;
    public int FinaleEffectCount { get; private set; }
    private Renderer guideRenderer;
    public Vector3 GuidePosition
    {
        get
        {
            if (guideRenderer == null) guideRenderer = GetComponent<StudioBalloonAppearance>()?.latex;
            return guideRenderer != null ? guideRenderer.bounds.center : transform.position;
        }
    }

    [SerializeField]
    private ParticleSystem popEffectPrefab;

    [SerializeField]
    private ParticleSystem[] popEffectVariants;

    [SerializeField]
    private ParticleSystem onBeatGlowPrefab;

    [SerializeField]
    private Transform popPoint;

    public int EventId { get; private set; } = -1;

    public double EventTimeSeconds { get; private set; }

    public bool CanReceiveChain(double songTime) => EventId >= 0 && !isPoping &&
        gameObject.activeInHierarchy && songTime <= EventTimeSeconds + cleanupDelaySeconds;

    [SerializeField]
    private double cleanupDelaySeconds = 5d;

    [SerializeField]
    private double rhythmWindowsSeconds = 0.1d;

    [SerializeField]
    private GameObject[] wallPaintPrefabs; 


    public void InitializeRhythm(RhythmEventData eventData)
    {
        EventId = eventData.id;
        EventTimeSeconds = eventData.timeSeconds;
        finaleAccent = eventData.finaleAccent;
    }

    public void CheckExpiry(double songTime)
    {
        if (EventId < 0 || isPoping)
        {
            return;
        }

        if (songTime <= EventTimeSeconds + cleanupDelaySeconds)
        {
            return;
        }

        Score.RegisterExpiredTarget();

        isPoping = true;
        OnResolved?.Invoke(EventId);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public bool IsOnBeat(double shotTime)
    {
        if (EventId < 0 || isPoping)
        {
            return false;
        }

        double errorSeconds = 
            System.Math.Abs(shotTime - EventTimeSeconds);

        return errorSeconds <= rhythmWindowsSeconds;
    }

    public void Hit(int bonusPoints = 0)
    {
        if (isPoping)
        {
            return;
        }

        Pop(bonusPoints, false, default);
    }

    public void HitAt(Vector3 hitPoint, int bonusPoints = 0)
    {
        if (isPoping)
        {
            return;
        }

        Pop(bonusPoints, true, hitPoint);
    }

    public void HitFromChain()
    {
        if (isPoping || !RoundController.IsPlaying || RoundController.IsPaused) return;
        Pop(0, false, default, false);
    }

    private void Pop(
        int bonusPoints,
        bool hasHitPoint,
        Vector3 hitPoint,
        bool direct = true)
    {
        isPoping = true;
        Score.Add(points + bonusPoints);
        Score.RegisterSuccessfulHit(bonusPoints > 0);
        OnResolved?.Invoke(EventId);

        if (bonusPoints > 0)
        {
            OnBeatHit?.Invoke(points + bonusPoints);
        }

        foreach(Collider balloonCollider 
            in GetComponentsInChildren<Collider>())
        {
            balloonCollider.enabled = false;
        }

        if (direct)
        {
            // Spend only a previously banked charge. A relay completed by this pop
            // is granted afterward and belongs to the following direct hit.
            OnDirectPop?.Invoke(this);
            StudioBalloonAppearance pigment = GetComponent<StudioBalloonAppearance>();
            if (pigment != null && pigment.pigments.Length > 0)
                OnPigmentPopped?.Invoke(pigment.ColorIndex, pigment.Pigment);
        }

        LeaveWallPaint();

        ParticleSystem selectedPopEffect = SelectPopEffect();

        if (selectedPopEffect != null)
        {
            Vector3 effectPosition = hasHitPoint
                ? hitPoint
                : transform.position;

            if (!hasHitPoint && popPoint != null)
            {
                effectPosition = popPoint.position;
            }

            ParticleSystem effect = Instantiate(
                selectedPopEffect,
                effectPosition,
                Quaternion.identity);

            if (popEffectVariants == null ||
                popEffectVariants.Length == 0)
            {
                GetComponent<StudioBalloonAppearance>()?
                    .ColorizeBurst(effect.gameObject);
            }

            if (bonusPoints > 0)
            {
                effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                foreach( ParticleSystem part in effect.GetComponentsInChildren<ParticleSystem>() )
                {
                    var main = part.main;
                    main.startSizeMultiplier *= 1.35f;
                }

                if (onBeatGlowPrefab != null)
                {
                    ParticleSystem glow = Instantiate(
                        onBeatGlowPrefab,
                        effectPosition,
                        Quaternion.identity
                        );

                    Destroy( glow.gameObject, 1f );
                }
            }

            effect.Play();

            Destroy(effect.gameObject, 2f);
        }

        Camera mainCamera = Camera.main;

        if (finaleAccent && popEffectVariants != null && popEffectVariants.Length > 0)
        {
            Vector3 center = hasHitPoint ? hitPoint : (popPoint != null ? popPoint.position : transform.position);
            for (int index = 0; index < 3; index++)
            {
                ParticleSystem prefab = popEffectVariants[(index * 2) % popEffectVariants.Length];
                if (prefab == null) continue;
                Vector3 offset = new Vector3((index - 1) * .24f, index == 1 ? .22f : 0f, 0f);
                ParticleSystem accent = Instantiate(prefab, center + offset, Quaternion.identity);
                accent.transform.localScale *= .65f;
                accent.Play(true);
                Destroy(accent.gameObject, 2f);
                FinaleEffectCount++;
            }
        }

        if (popSound != null && mainCamera != null)
        {
            AudioSource.PlayClipAtPoint(
                popSound,
                mainCamera.transform.position,
                popVolume);
        }

        Destroy(gameObject);
    }

    private ParticleSystem SelectPopEffect()
    {
        if (popEffectVariants == null ||
            popEffectVariants.Length == 0)
        {
            return popEffectPrefab;
        }

        StudioBalloonAppearance appearance =
            GetComponent<StudioBalloonAppearance>();

        int colorIndex = appearance != null
            ? appearance.ColorIndex
            : 0;

        int requestedIndex =
            KyudaisaiLearnerCore.SelectExplosionVariant(
                colorIndex,
                popEffectVariants.Length);

        int safeIndex = Mathf.Clamp(
            requestedIndex,
            0,
            popEffectVariants.Length - 1);

        ParticleSystem selected = popEffectVariants[safeIndex];
        return selected != null ? selected : popEffectPrefab;
    }

    private void LeaveWallPaint()
    {
        StudioBalloonAppearance appearance = GetComponent<StudioBalloonAppearance>();

        if (appearance == null || wallPaintPrefabs == null)
        {
            return;
        }

        int index = appearance.ColorIndex;


        if (index < 0 || index >= wallPaintPrefabs.Length ||
            wallPaintPrefabs[index] == null)
        {
            return;
        }
        foreach(Vector3 direction in PaintDirections)
        {
            bool hasHit = Physics.Raycast(
                transform.position,
                direction,
                out RaycastHit hit,
                3f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
             );

            if (!hasHit)
            {
                continue;
            }

            if (!PaintSurface.CanReceivePaint(hit.collider))
            {
                continue;
            }

            Vector3 position = hit.point + hit.normal * 0.005f;


            Quaternion rotation =
                 Quaternion.FromToRotation(
                     Vector3.forward,
                     hit.normal);

            Instantiate(wallPaintPrefabs[index], position, rotation);
        }

    }
}
