using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRaycastDebugger : MonoBehaviour
{
    readonly List<RaycastResult> _hits = new List<RaycastResult>();
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current)
        {
            _hits.Clear();
            var ed = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(ed, _hits);
            if (_hits.Count == 0) Debug.Log("No UI under cursor");
            for (int i = 0; i < _hits.Count; i++)
                Debug.Log($"UI hit[{i}]: {_hits[i].gameObject.name}");
        }
    }
}
