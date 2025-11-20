using System;

namespace CrossingSimulator.Networking
{
    [Serializable]
    public class ApiResponseEnvelope<TData>
    {
        public int status;
        public string message;
        public TData data;

        public bool IsSuccessStatus => status == 200 || status == 201;
    }
}
