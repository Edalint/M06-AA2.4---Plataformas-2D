using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoToSceneLoader : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "Playground";

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = FindFirstObjectByType<VideoPlayer>();
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}