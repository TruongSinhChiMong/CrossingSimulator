using System;
using UnityEngine;

/// <summary>
/// Phát lệnh toàn cục cho học sinh: Z = Cross, X = Stop.
/// Đặt script này vào 1 GameObject trong scene (ví dụ "GameDirector").
/// </summary>
public class OrdersManager : MonoBehaviour
{
    public static OrdersManager Instance { get; private set; }

    public static event Action OnStopOrder;   // X
    public static event Action OnCrossOrder;  // Z

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void EmitStop() => OnStopOrder?.Invoke();
    public static void EmitCross() => OnCrossOrder?.Invoke();
}
