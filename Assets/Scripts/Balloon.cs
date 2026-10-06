using UnityEngine;

public class Balloon : MonoBehaviour
{
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
    private Transform popPoint;

   public void Hit()
    {
        if (isPoping)
        {
            return;
        }

        Pop();
    }

    private void Pop()
    {
        isPoping = true;
        Score.Add(points);

        foreach(Collider balloonCollider 
            in GetComponentsInChildren<Collider>())
        {
            balloonCollider.enabled = false;
        }

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
}