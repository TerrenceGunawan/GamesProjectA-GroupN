using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIVideoPlayer : MonoBehaviour
{
    private RawImage rawImage;     // Assign the RawImage in Inspector
    private VideoPlayer videoPlayer; // Assign your VideoPlayer

    void Start()
    {
        // If not assigned, try to find them automatically
        if (!videoPlayer) videoPlayer = GetComponent<VideoPlayer>();
        if (!rawImage) rawImage = GetComponent<RawImage>();

        // Set up video to render to texture
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        // Create a RenderTexture dynamically that matches screen size
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;

        // Play the video
        videoPlayer.isLooping = false;
    }

    void Update()
    {
        if (gameObject.activeInHierarchy && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }
}
