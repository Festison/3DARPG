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
        // �� �ε��߿��� �ٸ� �ൿ�� �� �� �ִ�.
        AsyncOperation asyncSceneload = SceneManager.LoadSceneAsync("Playground");
        // ���� �������� �� ���Ӽ�
        asyncSceneload.allowSceneActivation = false;

        float timer = 0f;

        // ����ũ �ε�
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
