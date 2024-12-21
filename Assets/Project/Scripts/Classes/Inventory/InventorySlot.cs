using ProjectModels;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDropHandler
{
    public int index;
    public GameObject itemPrefub;
    public InventoryItem item;

    private bool longPressStarted = false;
    private float longPressTime = 0;
    private bool longPress = false;

    void Awake()
    {
        index = transform.GetSiblingIndex();
    }

    void Update()
    {
        if (transform.childCount > 0)
        {
            item = transform.GetChild(0).GetComponent<InventoryItem>();
        }
        else
        {
            item = null;
        }

        HandleLongPress();
    }

    private void HandleLongPress()
    {
        if (longPressStarted && transform.childCount > 0)
        {
            longPressTime += Time.deltaTime;
            if (longPressTime > 0.3f)
            {
                OnLongPress();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (longPress) return;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        longPressStarted = true;
        longPressTime = 0;
        longPress = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        longPressStarted = false;
        longPressTime = 0;

        if (item != null && item.dragPressed)
            item.DragPressed(false);
    }

    public void OnLongPress()
    {
        longPressStarted = false;
        longPress = true;

        if (item != null && !item.dragPressed && !item.isDragging)
            item.DragPressed(true);

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDragging) return;

        GameObject dropped = eventData.pointerDrag;
        InventoryItem droppedItem = dropped.GetComponent<InventoryItem>();

        if (item != null)
        {
            item.slot = droppedItem.slot;
            item.transform.SetParent(droppedItem.slot.transform);
        }

        droppedItem.slot = this;
        droppedItem.transform.SetParent(transform);
    }

    public void SpawnItem(ArmorData data)
    {
        GameObject armorItem = Instantiate(itemPrefub, transform.position, Quaternion.identity, transform);
        armorItem.transform.SetParent(transform);
        armorItem.GetComponent<InventoryItem>().SetData(this, data);
    }
}
