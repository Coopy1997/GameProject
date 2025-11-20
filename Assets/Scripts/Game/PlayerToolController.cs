using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerToolController : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCam;
    public GameObject foodPelletPrefab;
    public LayerMask fishLayer = ~0;

    public FishInfoUI fishInfoUI;

    Fish grabbedFish;
    Vector3 grabOffsetLocal;
    float grabbedZ;

    void Update()
    {
        if (!mainCam) mainCam = Camera.main;
        if (ToolManager.Instance == null) return;

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

    void HandleHand()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartGrab();

        if (grabbedFish)
        {
            var world = mainCam.ScreenToWorldPoint(Input.mousePosition);
            world.z = grabbedZ;
            var target = grabbedFish.transform.parent
                ? grabbedFish.transform.parent.TransformPoint(grabOffsetLocal)
                : grabbedFish.transform.TransformPoint(grabOffsetLocal);
            var delta = world - target;
            grabbedFish.transform.position += delta;

            if (Input.GetMouseButtonUp(0))
            {
                grabbedFish = null;
                if (fishInfoUI) fishInfoUI.Hide();   // NEW
            }

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

        var local = fish.transform.InverseTransformPoint(world);
        grabOffsetLocal = local;

        // NEW: tell UI to show
        if (fishInfoUI) fishInfoUI.Show(fish);
    }


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
