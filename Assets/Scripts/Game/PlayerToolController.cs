using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerToolController : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCam;                  // assign or will use Camera.main
    public GameObject foodPelletPrefab;     // drag your Food prefab here
    public LayerMask fishLayer = ~0;        // optional: set to "Fish" layer in Inspector

    // drag state
    private Fish grabbedFish;
    private Vector3 grabOffsetLocal;        // offset from fish pivot to cursor (local space)
    private float grabbedZ;

    void Update()
    {
        if (!mainCam) mainCam = Camera.main;
        if (ToolManager.Instance == null) return;

        // Ignore clicks when cursor is over UI
        //if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        switch (ToolManager.Instance.currentTool)
        {
            case ToolType.Hand:
                HandleHand();
                break;

            case ToolType.Food:
                HandleFood();
                break;

            case ToolType.Medicine:
                HandleMedicine();
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // HAND: click a fish to grab + drag; release to drop
    void HandleHand()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartGrab();

        if (grabbedFish)
        {
            // While holding, move fish with cursor
            var world = mainCam.ScreenToWorldPoint(Input.mousePosition);
            world.z = grabbedZ;
            var target = grabbedFish.transform.parent
                ? grabbedFish.transform.parent.TransformPoint(grabOffsetLocal)
                : grabbedFish.transform.TransformPoint(grabOffsetLocal);
            // move so that the original offset tracks the cursor
            var delta = world - target;
            grabbedFish.transform.position += delta;

            if (Input.GetMouseButtonUp(0))
                grabbedFish = null;
        }
    }

    void TryStartGrab()
    {
        var world = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(world, fishLayer);
        if (!hit) return;

        var fish = hit.GetComponentInParent<Fish>();
        if (!fish) return;

        grabbedFish = fish;
        grabbedZ = fish.transform.position.z;

        // store local offset so the cursor “sticks” to the same place on the fish
        var local = fish.transform.InverseTransformPoint(world);
        grabOffsetLocal = local;
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // FOOD: left click drops one pellet at cursor
    void HandleFood()
    {
        if (!foodPelletPrefab) return;
        if (Input.GetMouseButtonDown(0))
        {
            var p = mainCam.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            Instantiate(foodPelletPrefab, p, Quaternion.identity);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // MEDICINE: left click uses medicine once (spends gold, heals water)
    void HandleMedicine()
    {
        if (Input.GetMouseButtonDown(0))
            UseMedicine();
    }

    void UseMedicine()
    {
        if (WaterHealthController.Instance)
            WaterHealthController.Instance.UseMedicine();
    }
}
