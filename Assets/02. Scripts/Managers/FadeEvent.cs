using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

public class FadeEvent : MonoBehaviour
{
    // ���̵� �ƿ��� ����Ǵ� �ð�
    private float fadeDuration = 1;
    [Tooltip("���� ȭ��")]
    public CanvasGroup fadeImage;


    public UnityEvent OnFadeEvent;

    public void FadeIn() //���̵� �� ���
    {
        StartCoroutine(Fade(true));
    }

    public void FadeOut() //���̵� �ƿ� ���
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

