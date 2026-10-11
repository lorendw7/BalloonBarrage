using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundController : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float roundDuration = 90f;

    private float remainingTime;

    private int finalScore;

    public static bool IsPlaying { get; private set; }

    private void Start()
    {
        Score.ResetScore();
        remainingTime = roundDuration;
        IsPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying)
            return;

        remainingTime = Mathf.Max(
            0f, remainingTime - Time.deltaTime);

        if (remainingTime > 0f )
        {
            return;
        }

        EndRound();

    }

    private void EndRound()
    {
        IsPlaying = false;
        finalScore = Score.Value;

        foreach (PaintProjectile projectile in 
            FindObjectsByType<PaintProjectile>(FindObjectsSortMode.None))
        {
            projectile.gameObject.SetActive(false);
            Destroy(projectile.gameObject);
        }

        foreach (Balloon balloon in
    FindObjectsByType<Balloon>(FindObjectsSortMode.None))
        {
            balloon.gameObject.SetActive(false);
            Destroy(balloon.gameObject);
        }

        Debug.Log($"本局结束，最终分数：{finalScore}");
    }

    private void RestartRound()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(20, 55, 200, 40),
            $"Time: {Mathf.CeilToInt(remainingTime)}");

        if (IsPlaying)
        {
            return;
        }

        float left = (Screen.width - 260f) / 2f;
        float top = (Screen.height - 160f) / 2f;


    }
}
