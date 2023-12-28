using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using TMPro;

namespace Festison
{
    public class LoadingSceneManager : SingleTon<LoadingSceneManager>
    {
        [Header("�ε� UI")]
        [Tooltip("�ε� ȭ��")]
        public GameObject Loading;
        [Tooltip("�ε� �ۼ�Ʈ")]
        public TextMeshProUGUI Loadingtext;
        [Tooltip("���� ȭ��")]
        public CanvasGroup fadeImage;

        private float fadeDuration = 2;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            fadeImage.DOFade(0, fadeDuration).OnStart(() => { Loading.SetActive(false); })
                .OnComplete(() => { fadeImage.blocksRaycasts = false; });
        }

        public void ChangeScene(string sceneName)
        {

            fadeImage.DOFade(1, fadeDuration).OnStart(() => { fadeImage.blocksRaycasts = true; })
                .OnComplete(() => { StartCoroutine("LoadScene", sceneName); });
        }

        IEnumerator LoadScene(string sceneName)
        {
            Loading.SetActive(true);

            // �� �ε� �߿��� ���� ����
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

            async.allowSceneActivation = false;

            float pastTime = 0;
            float percentage = 0;

            while (!(async.isDone))
            {
                yield return null;

                pastTime += Time.deltaTime;

                if (percentage >= 90)
                {
                    percentage = Mathf.Lerp(percentage, 100, pastTime);

                    if (percentage == 100)
                    {
                        async.allowSceneActivation = true; //�� ��ȯ �غ� �Ϸ�
                    }
                }
                else
                {
                    percentage = Mathf.Lerp(percentage, async.progress * 100f, pastTime);
                    if (percentage >= 90)
                        pastTime = 0;
                }
                Loadingtext.text = percentage.ToString("0") + "%"; //�ε� �ۼ�Ʈ ǥ��
            }
        }
    }
}

