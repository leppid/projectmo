using System;
using ProjectModels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { Any, Head, Body, Legs, Primary, Secondary }

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDropHandler
{
    public string label;
    private TextMeshProUGUI labelTextMesh;
    public SlotType type;
    public GameObject itemPrefub;
    public int index;
    public InventoryItem item;

    private bool longPressStarted = false;
    private float longPressTime = 0;
    private bool longPress = false;

    void Awake()
    {
        switch (type)
        {
            case SlotType.Any:
                index = transform.GetSiblingIndex();
                break;
            case SlotType.Head:
                index = -1;
                label = "Head";
                break;
            case SlotType.Body:
                index = -2;
                label = "Body";
                break;
            case SlotType.Legs:
                index = -3;
                label = "Legs";
                break;
            case SlotType.Primary:
                index = -4;
                label = "Primary";
                break;
            case SlotType.Secondary:
                index = -5;
                label = "Secondary";
                break;
        }

        labelTextMesh = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        if (label != null) labelTextMesh.text = label;
    }

    void Update()
    {
        if (transform.childCount > 1)
        {
            item = transform.GetChild(1).GetComponent<InventoryItem>();
            labelTextMesh.text = null;
        }
        else
        {
            if (item != null)
            {
                item = item.isDragReady ? item : null;
            }
            labelTextMesh.text = label;
        }

        HandleLongPress();
    }

    private void HandleLongPress()
    {
        if (longPressStarted && transform.childCount > 1)
        {
            longPressTime += Time.deltaTime;
            if (longPressTime > 0.18f)
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

        if (item != null && item.isDragReady && !item.isDrag)
            item.SetDragReady(false);
    }

    public void OnLongPress()
    {
        longPressStarted = false;
        longPress = true;

        if (item != null && !item.isDragReady && !item.isDrag)
            item.SetDragReady(true);

    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem droppedItem = dropped.GetComponent<InventoryItem>();

        if (droppedItem != null && !droppedItem.isDragReady) return;

        PlaceItem(droppedItem);
    }

    public void PlaceItem(InventoryItem itemToPlace)
    {
        if (itemToPlace.slot.index == index) return;

        if (type != SlotType.Any && type.ToString() != itemToPlace.type.ToString()) return;

        if (item != null)
        {
            item.slot = itemToPlace.slot;
            item.transform.SetParent(itemToPlace.slot.transform);
            InventoryUtils.UpdateInventoryIndex(item.item.id, itemToPlace.slot.index);
        }

        itemToPlace.slot = this;
        itemToPlace.transform.SetParent(transform);
        InventoryUtils.UpdateInventoryIndex(itemToPlace.item.id, index);
    }

    public void CreateItem(ItemData data)
    {
        GameObject newItem = Instantiate(itemPrefub, transform.position, Quaternion.identity, transform);
        newItem.transform.SetParent(transform);
        newItem.GetComponent<InventoryItem>().SetData(this, data);
    }

    public void RemoveItem()
    {
        if (transform.childCount > 1)
        {
            item = null;
            Destroy(transform.GetChild(1).gameObject);
        }
    }
}
