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
    Fish selectedFish;
    Vector3 grabOffsetLocal;
    float grabbedZ;

    Vector3 mouseDownScreenPos;
    bool isDragging;
    public float dragThreshold = 10f;

    void Update()
    {
        if (!mainCam) mainCam = Camera.main;
        if (ToolManager.Instance == null) return;

        // optionally block when over UI
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

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
        {
            mouseDownScreenPos = Input.mousePosition;
            StartSelectOrGrab();
        }

        if (Input.GetMouseButton(0) && grabbedFish != null)
        {
            if (!isDragging)
            {
                var d = Input.mousePosition - mouseDownScreenPos;
                if (d.sqrMagnitude > dragThreshold * dragThreshold)
                    isDragging = true;
            }

            if (isDragging)
            {
                var world = mainCam.ScreenToWorldPoint(Input.mousePosition);
                world.z = grabbedZ;

                var target = grabbedFish.transform.parent
                    ? grabbedFish.transform.parent.TransformPoint(grabOffsetLocal)
                    : grabbedFish.transform.TransformPoint(grabOffsetLocal);

                var delta = world - target;
                grabbedFish.transform.position += delta;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            grabbedFish = null;
            isDragging = false;
        }
    }

    void StartSelectOrGrab()
    {
        var world = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(world, fishLayer);

        if (hit)
        {
            var fish = hit.GetComponentInParent<Fish>();
            if (fish != null)
            {
                selectedFish = fish;
                grabbedFish = fish;
                grabbedZ = fish.transform.position.z;
                grabOffsetLocal = fish.transform.InverseTransformPoint(world);

                if (fishInfoUI != null)
                    fishInfoUI.Show(fish);

                return;
            }
        }

        // clicked empty water -> clear selection
        selectedFish = null;
        grabbedFish = null;
        isDragging = false;

        if (fishInfoUI != null)
            fishInfoUI.Hide();
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
