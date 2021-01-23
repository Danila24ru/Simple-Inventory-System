using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests : MonoBehaviour
{
    private const int timeoutRequestSeconds = 60;

    private const string authToken = "BMeHG5xqJeB4qCjpuJCTQLsqNGaqkfB6";

    private static WebRequests instance;

    public static WebRequests Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("[WebRequests_Instance]").AddComponent<WebRequests>();

            return instance;
        }
    }

    public void Post(string url, WWWForm form, Action<string> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(DoPostRequest(url, form, onSuccess, onError));
    }

    public IEnumerator DoPostRequest(string url, WWWForm form, Action<string> onSuccess, Action<string> onError = null)
    {
        var request = UnityWebRequest.Post(url, form);

        request.timeout = timeoutRequestSeconds;

        if (!string.IsNullOrEmpty(authToken))
            request.SetRequestHeader("auth", authToken);

        yield return request.SendWebRequest();

        if (!string.IsNullOrEmpty(request.error) || request.isNetworkError || request.isHttpError)
        {
            var error = $"{url}\n{request.responseCode} - {request.error} - {request.downloadHandler.text}";

            Debug.LogError(error);

            onError?.Invoke(error);
        }
        else
        {
            onSuccess?.Invoke(request.downloadHandler.text);
            request.Dispose();
        }
    }
}
