using System;
using UnityEngine;

public class OrdersManager : MonoBehaviour
{
    public static OrdersManager Instance { get; private set; }

    public static event Action OnCross;
    public static event Action OnStop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // nếu muốn giữ qua scene
    }

    public static void EmitCross() => OnCross?.Invoke();
    public static void EmitStop() => OnStop?.Invoke();
}
