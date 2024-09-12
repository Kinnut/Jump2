using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject settingsMenu;
    public Animation settingsMenuAnimation;
    private bool isPaused = false;
    public float animationSpeed = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (isPaused && settingsMenuAnimation.isPlaying)
        {
            settingsMenuAnimation["Setting_Open"].time += Time.unscaledDeltaTime * animationSpeed;
            settingsMenuAnimation.Sample();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        settingsMenu.SetActive(true);
        settingsMenuAnimation.Play("Setting_Open");
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        settingsMenu.SetActive(false);
        isPaused = false;
    }
}
