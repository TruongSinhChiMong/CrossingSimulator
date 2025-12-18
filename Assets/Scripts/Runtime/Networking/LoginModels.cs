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
    
    [Serializable]
    public class RegisterRequest
    {
        public string email;
        public string password;
        public string displayName;
    }

    [Serializable]
    public class RegisterResponse
    {
        public string idToken;
        public string uid;
        public string email;
        public string displayName;
        public string refreshToken;
    }
}
