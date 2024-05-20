using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject credits;
    public GameObject controls;

    public Image fadeOut;
    public Image fadeOut2;
    public Text fadeOut3;

    public float fadeDuration = 2.0f; // Duration of the fade-out in seconds
    private float fadeStartTime;
    private Renderer[] renderers;
    private Color[] originalColors;
    private bool isFading = false;

    private float timeSince = 0f;

    // Start is called before the first frame update
    void Start()
    {
        credits.SetActive(false);
        controls.SetActive(false);

    }

    private bool round2 = false;
    private bool stop = false;
    private bool stop2 = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.CloseThisShit();
        }

        timeSince += Time.deltaTime;

        if (this.timeSince >= 5.0f && !stop)
        {
            stop = true;
            StartFadeOut2();
        }

        if (round2 && !stop2) {
            stop2 = true;
            StartFadeOut();
        }
    }

    private IEnumerator FadeOut()
    {
        // Get the current color of the image
        Color originalColor = this.fadeOut.color;

        // Loop over the duration to fade out
        for (float t = 0.01f; t < fadeDuration; t += Time.deltaTime)
        {
            // Calculate the new alpha
            float newAlpha = Mathf.Lerp(originalColor.a, 0, t / fadeDuration);

            // Apply the new color
            this.fadeOut.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            // Wait until the next frame
            yield return null;
        }

        // Ensure the image is completely transparent at the end
        fadeOut.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        fadeOut.enabled = false;
        fadeOut2.enabled = false;
        fadeOut3.enabled = false;
    }

    private IEnumerator FadeOut2()
    {
        // Get the current color of the image
        Color originalColor2 = this.fadeOut2.color;
        Color originalColor3 = this.fadeOut3.color;

        // Loop over the duration to fade out
        for (float t = 0.01f; t < fadeDuration; t += Time.deltaTime)
        {
            // Calculate the new alpha
            float newAlpha2 = Mathf.Lerp(originalColor2.a, 0, t / fadeDuration);
            float newAlpha3 = Mathf.Lerp(originalColor3.a, 0, t / fadeDuration);

            // Apply the new color
            this.fadeOut2.color = new Color(originalColor2.r, originalColor2.g, originalColor2.b, newAlpha2);
            this.fadeOut3.color = new Color(originalColor3.r, originalColor3.g, originalColor3.b, newAlpha3);

            // Wait until the next frame
            yield return null;
        }

        // Ensure the image is completely transparent at the end
        fadeOut2.color = new Color(originalColor2.r, originalColor2.g, originalColor2.b, 0);
        fadeOut3.color = new Color(originalColor3.r, originalColor3.g, originalColor3.b, 0);
        this.round2 = true;
    }

    public void StartFadeOut()
    {

        StartCoroutine(FadeOut());
    }

    public void StartFadeOut2()
    {

        StartCoroutine(FadeOut2());
    }

    public void LevelStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OpenCredits()
    {
        credits.SetActive(true);
    }
    public void OpenControls()
    {
        controls.SetActive(true);
    }
    public void CloseThisShit()
    {
        credits.SetActive(false);
        controls.SetActive(false);
    }
}
