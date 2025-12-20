using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickDebugger : MonoBehaviour
{
    private PointerEventData _ped;
    private readonly List<RaycastResult> _results = new();

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (EventSystem.current == null)
        {
            Debug.LogError("[UIClickDebugger] No EventSystem in scene!");
            return;
        }

        _ped ??= new PointerEventData(EventSystem.current);
        _ped.position = Input.mousePosition;

        _results.Clear();
        EventSystem.current.RaycastAll(_ped, _results);

        if (_results.Count == 0)
        {
            Debug.Log("[UIClickDebugger] Click: NO UI HIT");
            return;
        }

        string msg = "[UIClickDebugger] Top hits:\n";
        int show = Mathf.Min(8, _results.Count);
        for (int i = 0; i < show; i++)
        {
            var r = _results[i];
            msg += $"{i + 1}) {r.gameObject.name}\n";
        }
        Debug.Log(msg);
    }
}
