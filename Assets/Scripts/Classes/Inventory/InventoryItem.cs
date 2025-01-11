using System.Collections;
using ProjectModels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType { Head, Body, Legs, Primary, Secondary, Quest }

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public InventorySlot slot;
    public ItemData item;
    public ItemType type;
    private TextMeshProUGUI title;
    private Image image;
    public bool dragPressed = false;
    public bool dragWasPressed = false;
    public bool isDragging = false;

    float clicked = 0;
    float clicktime = 0;
    readonly float clickdelay = 0.2f;

    void Awake()
    {
        image = transform.GetComponent<Image>();
        title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (clicktime + clickdelay < Time.time) clicked = 0;

        title.text = item.name;

        if (!isDragging)
        {
            slot = transform.parent.GetComponent<InventorySlot>();
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.5f);
        }
    }

    public void Equip()
    {
        if (item != null && item.type != ItemType.Quest.ToString())
        {
            InventorySlot equipSlot = InventoryManager.instance.GetEquipSlotByType(item.type);

            if (equipSlot == slot)
            {
                Unequip();
                return;
            };

            equipSlot.PlaceItem(this);
        }
    }

    public void Unequip()
    {
        if (item != null && item.type != ItemType.Quest.ToString())
        {
            InventorySlot firstEmptySlot = InventoryManager.instance.GetFirstEmptyBagSlot();

            if (firstEmptySlot == null) return;

            firstEmptySlot.PlaceItem(this);
        }
    }

    Coroutine onPointerClickCoroutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragPressed) return;

        clicked++;

        if (clicked == 1)
        {
            clicktime = Time.time;
            onPointerClickCoroutine = StartCoroutine(OnPointerClickEnum());
        }

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            StopCoroutine(onPointerClickCoroutine);
            clicked = 0;
            clicktime = 0;
            if (item != null) Equip();
        }
        else if (clicked > 2 || Time.time - clicktime > 1)
        {
            clicked = 0;
        }
    }

    IEnumerator OnPointerClickEnum()
    {
        yield return new WaitForSeconds(clickdelay);

        if (!dragWasPressed)
        {
            InventoryManager.instance.OpenItemInfo(this);
        }
    }

    public void DragPressed(bool enable)
    {
        transform.SetParent(slot.transform);
        transform.localScale = Vector3.one * (enable ? 1.1f : 1f);
        transform.GetComponent<CanvasGroup>().alpha = enable ? 0.8f : 1f;
        if (enable)
        {
            dragPressed = true;
        }
        else
        {
            StartCoroutine(DragPressedEnum());
        }
    }

    IEnumerator DragPressedEnum()
    {
        yield return new WaitForSeconds(0.1f);
        dragPressed = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragPressed) return;

        image.raycastTarget = false;
        dragPressed = false;
        isDragging = true;
        InventoryManager.instance.isDragging = true;
        InventoryManager.instance.EnableHoverSwipers(true);
        Transform root = GameObject.Find("Inventory").transform;
        transform.SetParent(root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        transform.position = Vector2.Lerp(transform.position, eventData.position, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        DragPressed(false);
        StartCoroutine(OnEndDragEnum());
    }

    IEnumerator OnEndDragEnum()
    {
        yield return new WaitForSeconds(0.1f);
        InventoryManager.instance.isDragging = false;
        InventoryManager.instance.EnableHoverSwipers(false);
        image.raycastTarget = true;
    }

    public void SetData(InventorySlot slot, ItemData data)
    {
        this.slot = slot;
        slot.item = this;
        item = data;
        switch (data.type)
        {
            case "Head": type = ItemType.Head; break;
            case "Body": type = ItemType.Body; break;
            case "Legs": type = ItemType.Legs; break;
            case "Primary": type = ItemType.Primary; break;
            case "Secondary": type = ItemType.Secondary; break;
        }
    }
}
