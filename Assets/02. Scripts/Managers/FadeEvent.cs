using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

public class FadeEvent : MonoBehaviour
{
    // 페이드 아웃이 진행되는 시간
    private float fadeDuration = 1;
    [Tooltip("가릴 화면")]
    public CanvasGroup fadeImage;


    public UnityEvent OnFadeEvent;

    public void FadeIn() //페이드 인 사용
    {
        StartCoroutine(Fade(true));
    }

    public void FadeOut() //페이드 아웃 사용
    {
        StartCoroutine(Fade(false));
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        if (isFadeIn)
        {
            fadeImage.alpha = 1;
            Tween tween = fadeImage.DOFade(0f, fadeDuration);
            yield return tween.WaitForCompletion();
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            fadeImage.alpha = 0;
            fadeImage.gameObject.SetActive(true);
            Tween tween = fadeImage.DOFade(1f, fadeDuration);
            yield return tween.WaitForCompletion();
            StartCoroutine(Fade(true));
        }
    }
}

