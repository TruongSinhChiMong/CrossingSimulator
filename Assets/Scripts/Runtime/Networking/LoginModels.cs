using System;

namespace CrossingSimulator.Networking
{
    [Serializable]
    public class LoginRequest
    {
        public string email;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public string idToken;
        public string uid;
        public string email;
        public string displayName;
        public string refreshToken;
    }
}
