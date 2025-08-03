using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FlagpoleScript : MonoBehaviour
{
    [Header("Flagpole Settings")]
    [Tooltip("Name of the scene to load after flagpole sequence.")]
    public string nextSceneName;
    [Tooltip("Reference to the black UI Image for fade out.")]
    public Image blackFadeImage;
    [Tooltip("How long to freeze the player before sliding down.")]
    public float freezeDuration = 0.3f;
    [Tooltip("How long to slide down the pole.")]
    public float slideDuration = 1.0f;
    [Tooltip("How long the player walks after reaching the bottom.")]
    public float walkDuration = 1.2f;
    [Tooltip("How long the fade to black takes.")]
    public float fadeDuration = 1.0f;

    private bool sequenceStarted = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!sequenceStarted && other.CompareTag("Player"))
        {
            sequenceStarted = true;
            StartCoroutine(FlagpoleSequence(other.gameObject));
        }
    }

    IEnumerator FlagpoleSequence(GameObject player)
    {
        // Freeze player
        var playerController = player.GetComponent<PlayerController>();
        var rb = player.GetComponent<Rigidbody2D>();
        if (playerController != null && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            playerController.enabled = false;
        }

        // Wait a moment
        yield return new WaitForSeconds(freezeDuration);

        // Slide player down to bottom of flagpole (bottom of this collider)
        float startY = player.transform.position.y;
        float endY = GetComponent<Collider2D>().bounds.min.y + player.GetComponent<Collider2D>().bounds.extents.y;
        float elapsed = 0f;
        Vector3 startPos = player.transform.position;
        Vector3 endPos = new Vector3(startPos.x, endY, startPos.z);
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = endPos;

        // Unfreeze, walk right, set isRunning true
        if (playerController != null && rb != null)
        {
            rb.isKinematic = false;
            playerController.enabled = true;
            // Force walk right
            StartCoroutine(WalkOff(playerController, walkDuration));
        }

        // Fade to black
        if (blackFadeImage != null)
        {
            Color c = blackFadeImage.color;
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, fadeElapsed / fadeDuration);
                blackFadeImage.color = new Color(c.r, c.g, c.b, alpha);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
            blackFadeImage.color = new Color(c.r, c.g, c.b, 1f);
        }

        // Wait a moment, then load next scene
        yield return new WaitForSeconds(0.3f);
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator WalkOff(PlayerController playerController, float duration)
    {
        float elapsed = 0f;
        // Set running animation
        playerController.SetHurt(false); // Ensure not hurt
        var animator = playerController.GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("isRunning", true);

        // Move player right at a fixed speed
        var rb = playerController.GetComponent<Rigidbody2D>();
        float walkSpeed = 4f;
        while (elapsed < duration)
        {
            if (rb != null)
                rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
