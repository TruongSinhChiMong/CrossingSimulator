using System;

namespace CrossingSimulator.Networking
{
    [Serializable]
    public class ApiResponseEnvelope<TData>
    {
        public int code;      // Server trả về "code" thay vì "status"
        public int status;    // Giữ lại để backward compatible
        public string message;
        public TData data;

        public bool IsSuccessStatus => code == 200 || code == 201 || status == 200 || status == 201;
    }
}
