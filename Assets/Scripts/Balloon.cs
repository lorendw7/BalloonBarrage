using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Balloon : MonoBehaviour
{
    [SerializeField]
    public int points = 10;

    [SerializeField]
    private float popDuration = 0.12f;

    [SerializeField]
    private AudioClip popSound;

    [SerializeField, Range(0f, 1f)]
    private float popVolume = 0.5f;

    private bool isPoping;

    [SerializeField]
    private ParticleSystem popEffectPrefab;

    [SerializeField]
    private Transform popPoint;

    private void Update()
    {
        if (isPoping ||
            Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null )
        {
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            hit.collider.GetComponentInParent<Balloon>() == this)
        {
            StartCoroutine(Pop());
        }
    }

    private IEnumerator Pop()
    {
        isPoping = true;
        Score.Add(points);

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

        Collider boolloonCollider = GetComponent<Collider>();
        if (boolloonCollider != null)
        {
            boolloonCollider.enabled = false;
        }

        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;

            float progress = (elapsed / popDuration);

            transform.localScale = Vector3.Lerp(
                startScale, Vector3.zero, progress);

            yield return null;
        }

        Destroy(gameObject);
    }
}