using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CrossingSimulator.Networking
{
    public class ApiService : MonoBehaviour
    {
        static ApiService instance;
        const string JsonContentType = "application/json";

        public static ApiService Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                var existing = FindObjectOfType<ApiService>();
                if (existing != null)
                    return existing;

                var go = new GameObject("ApiService");
                instance = go.AddComponent<ApiService>();
                DontDestroyOnLoad(go);
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public Coroutine Get(string path, Action<ApiResponse> onCompleted, int timeoutSeconds = 30)
        {
            var request = UnityWebRequest.Get(ApiConfig.BuildUrl(path));
            request.timeout = timeoutSeconds;
            return StartCoroutine(SendRequest(request, onCompleted));
        }

        public Coroutine PostJson<TPayload>(string path, TPayload payload, Action<ApiResponse> onCompleted, int timeoutSeconds = 30)
        {
            var request = BuildJsonRequest(UnityWebRequest.kHttpVerbPOST, path, payload);
            request.timeout = timeoutSeconds;
            return StartCoroutine(SendRequest(request, onCompleted));
        }

        public Coroutine PutJson<TPayload>(string path, TPayload payload, Action<ApiResponse> onCompleted, int timeoutSeconds = 30)
        {
            var request = BuildJsonRequest(UnityWebRequest.kHttpVerbPUT, path, payload);
            request.timeout = timeoutSeconds;
            return StartCoroutine(SendRequest(request, onCompleted));
        }

        public Coroutine Delete(string path, Action<ApiResponse> onCompleted, int timeoutSeconds = 30)
        {
            var request = UnityWebRequest.Delete(ApiConfig.BuildUrl(path));
            request.timeout = timeoutSeconds;
            return StartCoroutine(SendRequest(request, onCompleted));
        }

        public Coroutine SendCustom(UnityWebRequest request, Action<ApiResponse> onCompleted)
        {
            return StartCoroutine(SendRequest(request, onCompleted));
        }

        UnityWebRequest BuildJsonRequest<TPayload>(string method, string path, TPayload payload)
        {
            var url = ApiConfig.BuildUrl(path);
            var request = new UnityWebRequest(url, method)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            var json = payload != null ? JsonUtility.ToJson(payload) : "{}";
            var body = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.SetRequestHeader("Content-Type", JsonContentType);
            request.SetRequestHeader("Accept", JsonContentType);

            return request;
        }

        IEnumerator SendRequest(UnityWebRequest request, Action<ApiResponse> onCompleted)
        {
            if (request.downloadHandler == null)
                request.downloadHandler = new DownloadHandlerBuffer();

            ApplyAuthHeaders(request);

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            var result = request.result;
            var error = request.error;
#else
            var result = request.isNetworkError ? UnityWebRequest.Result.ConnectionError :
                request.isHttpError ? UnityWebRequest.Result.ProtocolError : UnityWebRequest.Result.Success;
            var error = request.error;
#endif

            var response = new ApiResponse(
                request.url,
                request.responseCode,
                request.downloadHandler.text,
                error,
                result,
                request.GetResponseHeaders());

            onCompleted?.Invoke(response);
            request.Dispose();
        }

        void ApplyAuthHeaders(UnityWebRequest request)
        {
            var tokenStore = AuthTokenStore.Instance;
            if (tokenStore == null)
            {
                Debug.LogWarning("[ApiService] AuthTokenStore is null!");
                return;
            }

            var token = tokenStore.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
                Debug.Log($"[ApiService] Applied auth token to request: {request.url} (token length: {token.Length})");
            }
            else
            {
                Debug.LogWarning($"[ApiService] No auth token available for request: {request.url}");
            }
        }
    }

    public readonly struct ApiResponse
    {
        public string Url { get; }
        public long StatusCode { get; }
        public string Body { get; }
        public string Error { get; }
        public UnityWebRequest.Result Result { get; }
        public System.Collections.Generic.Dictionary<string, string> Headers { get; }

        public bool Success => Result == UnityWebRequest.Result.Success && StatusCode >= 200 && StatusCode < 300;
        public bool HasNetworkError => Result == UnityWebRequest.Result.ConnectionError || Result == UnityWebRequest.Result.DataProcessingError;

        public ApiResponse(
            string url,
            long statusCode,
            string body,
            string error,
            UnityWebRequest.Result result,
            System.Collections.Generic.Dictionary<string, string> headers)
        {
            Url = url;
            StatusCode = statusCode;
            Body = body;
            Error = error;
            Result = result;
            Headers = headers;
        }

        public bool TryGetEnvelope<TData>(out ApiResponseEnvelope<TData> envelope)
        {
            envelope = default;

            if (string.IsNullOrEmpty(Body))
            {
                Debug.LogWarning($"[ApiService] Empty response body from '{Url}'");
                return false;
            }

            try
            {
                Debug.Log($"[ApiService] Response from '{Url}': {Body}");
                envelope = JsonUtility.FromJson<ApiResponseEnvelope<TData>>(Body);
                return envelope != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ApiService] Failed to parse response envelope from '{Url}': {ex.Message}\nBody: {Body}");
                envelope = default;
                return false;
            }
        }

        public ApiResponseEnvelope<TData> GetEnvelopeOrDefault<TData>()
        {
            if (TryGetEnvelope<TData>(out var envelope))
                return envelope;

            int clampedStatus;
            if (StatusCode > int.MaxValue)
                clampedStatus = int.MaxValue;
            else if (StatusCode < int.MinValue)
                clampedStatus = int.MinValue;
            else
                clampedStatus = (int)StatusCode;

            return new ApiResponseEnvelope<TData>
            {
                status = clampedStatus,
                message = string.IsNullOrEmpty(Error) ? "Unable to parse response body." : Error,
                data = default
            };
        }
    }
}
