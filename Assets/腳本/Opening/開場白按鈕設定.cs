using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoButtonController : MonoBehaviour
{
    public Button Skip_Button;
    public Button myButton;        // 按钮对象
    public VideoPlayer videoPlayer; // 视频播放器对象

    void Start()
    {
        // 开始时隐藏按钮
        myButton.gameObject.SetActive(false);

        // 监听视频播放结束事件
        videoPlayer.loopPointReached += OnVideoEnd;

        // 監聽跳過按鈕的點擊事件
        Skip_Button.onClick.AddListener(SkipVideo);
    }

    // 当视频播放结束时调用
    void OnVideoEnd(VideoPlayer vp)
    {
        // 显示按钮
        myButton.gameObject.SetActive(true);
        Skip_Button.gameObject.SetActive(false);
    }

    void SkipVideo()
    {
        if (videoPlayer.isPlaying)
        {
            // 停止視頻並調用播放結束邏輯
            videoPlayer.Stop();
            OnVideoEnd(videoPlayer);
        }
    }

}
