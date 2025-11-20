using System;
using UnityEngine;

namespace CrossingSimulator.Networking
{
    [CreateAssetMenu(fileName = "AuthTokenStore", menuName = "CrossingSimulator/Auth Token Store", order = 0)]
    public class AuthTokenStore : ScriptableObject
    {
        static AuthTokenStore runtimeInstance;

        [SerializeField] string accessToken;
        [SerializeField] string refreshToken;
        [SerializeField] string userId;
        [SerializeField] string email;
        [SerializeField] string displayName;

        public static AuthTokenStore Instance
        {
            get
            {
                if (runtimeInstance != null)
                    return runtimeInstance;

                runtimeInstance = Resources.Load<AuthTokenStore>("AuthTokenStore");
                if (runtimeInstance == null)
                {
                    runtimeInstance = CreateInstance<AuthTokenStore>();
                    runtimeInstance.name = "AuthTokenStore (Runtime)";
                }

                return runtimeInstance;
            }
        }

        public event Action TokensChanged;

        public string AccessToken
        {
            get => accessToken;
            set
            {
                if (accessToken == value)
                    return;
                accessToken = value;
                TokensChanged?.Invoke();
            }
        }

        public string RefreshToken
        {
            get => refreshToken;
            set
            {
                if (refreshToken == value)
                    return;
                refreshToken = value;
                TokensChanged?.Invoke();
            }
        }

        public string UserId
        {
            get => userId;
            set => userId = value;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        public void ApplyLoginResponse(LoginResponse response)
        {
            if (response == null)
                return;

            AccessToken = response.idToken;
            RefreshToken = response.refreshToken;
            UserId = response.uid;
            Email = response.email;
            DisplayName = response.displayName;
        }

        public void Clear()
        {
            accessToken = null;
            refreshToken = null;
            userId = null;
            email = null;
            displayName = null;
            TokensChanged?.Invoke();
        }
    }
}
