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


    [SerializeField]
    public int points = 10;

    [SerializeField]
    private AudioClip popSound;

    [SerializeField, Range(0f, 1f)]
    private float popVolume = 0.5f;

    private bool isPoping;

    [SerializeField]
    private ParticleSystem popEffectPrefab;

    [SerializeField]
    private ParticleSystem onBeatGlowPrefab;

    [SerializeField]
    private Transform popPoint;

    public int EventId { get; private set; } = -1;

    public double EventTimeSeconds { get; private set; }

    [SerializeField]
    private double cleanupDelaySeconds = 3.1d;

    [SerializeField]
    private double rhythmWindowsSeconds = 0.1d;

    [SerializeField]
    private GameObject[] wallPaintPrefabs; 


    public void InitializeRhythm(RhythmEventData eventData)
    {
        EventId = eventData.id;
        EventTimeSeconds = eventData.timeSeconds;
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

        Pop(bonusPoints);
    }

    private void Pop(int bonusPoints)
    {
        isPoping = true;
        Score.Add(points + bonusPoints);
        Score.RegisterSuccessfulHit(bonusPoints > 0);

        if (bonusPoints > 0)
        {
            OnBeatHit?.Invoke(points + bonusPoints);
        }

        foreach(Collider balloonCollider 
            in GetComponentsInChildren<Collider>())
        {
            balloonCollider.enabled = false;
        }

        LeaveWallPaint();

        if (popEffectPrefab != null)
        {
            Vector3 effectPosition = transform.position;

            if (popPoint != null)
            {
                effectPosition = popPoint.position;
            }

            ParticleSystem effect = Instantiate(
                popEffectPrefab,
                effectPosition,
                Quaternion.identity);

            GetComponent<StudioBalloonAppearance>()?.ColorizeBurst(effect.gameObject);

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

        if (popSound != null && mainCamera != null)
        {
            AudioSource.PlayClipAtPoint(
                popSound,
                mainCamera.transform.position,
                popVolume);
        }

        Destroy(gameObject);
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
