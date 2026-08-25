using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerFallback : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoRT;

    void OnEnable()
    {
        StartCoroutine(StartPlayVideo());
    }

    private IEnumerator StartPlayVideo()
    {
        videoRT.SetActive(false);
        // while (!videoPlayer.isPrepared)
        // {
        //     yield return new WaitForEndOfFrame();
        // }
        try
        {
            videoPlayer.Play();
        }
        catch (Exception e)
        {
            videoRT.SetActive(false);
            videoPlayer.gameObject.SetActive(false);
            yield break;
        }
        while (!videoPlayer.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        while (videoPlayer.frame <= 0)
        {
            yield return new WaitForEndOfFrame();
        }
        videoRT.SetActive(true);
        // yield return new WaitForSecondsRealtime(0.5f);
        // if (!videoPlayer.isPlaying)
        // {
        //     videoRT.SetActive(false);
        //     videoPlayer.gameObject.SetActive(false);
        // }
    }

    void Update()
    {
        try
        {
            if (videoPlayer.GetDirectAudioMute(0) != !SaveDataUtils.SettingData.enableMusic)
            {
                videoPlayer.SetDirectAudioMute(0, !SaveDataUtils.SettingData.enableMusic);
            }
        }
        catch (Exception e)
        {
        }
    }
}
