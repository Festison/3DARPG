using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    private void Update()
    {
        LoadingText();
    }

    private void LoadingText()
    {
        loadingText.text = Mathf.FloorToInt(loadingBar.value*100) + "%";
    }

    IEnumerator LoadSceneProcess()
    {
        // 씬 로딩중에도 다른 행동을 할 수 있다.
        AsyncOperation asyncSceneload = SceneManager.LoadSceneAsync("Playground");
        // 팁을 보기위한 씬 속임수
        asyncSceneload.allowSceneActivation = false;

        float timer = 0f;

        // 페이크 로딩
        while (!asyncSceneload.isDone)
        {
            yield return null;

            if (asyncSceneload.progress < 0.9f)
            {
                loadingBar.value = asyncSceneload.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.value = Mathf.Lerp(0.9f, 1f, timer);

                if (true)
                {
                    asyncSceneload.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }
}
