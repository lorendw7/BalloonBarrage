using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShooter : MonoBehaviour
{

    [SerializeField] private Transform muzzle;

    [SerializeField] private PaintProjectile paintProjectilePrefab;

    [SerializeField] private ParticleSystem muzzleSpray;

    [SerializeField, Min(0.1f)]
    private float shotsPerSecond = 5f;

    private float nextShotTime;

    // Update is called once per frame
    void Update()
    {
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

        if (!Mouse.current.leftButton.isPressed)
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        if (muzzle == null || paintProjectilePrefab == null)
        {
            Debug.LogWarning("请连接 Muzzle 和 Paint Projectile Prefab。");
            return;
        }

        Vector3 direction = (aimPoint - muzzle.position).normalized;

        Instantiate(
            paintProjectilePrefab,
            muzzle.position,
            Quaternion.LookRotation(direction)
            );

        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);

        if (muzzleSpray != null)
        {
            muzzleSpray.Play();
        }
    }
}
