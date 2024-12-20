using System.Collections;
using ProjectModels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType { Head, Body, Legs, Primary, Secondary }

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot slot;
    public ArmorData armor;
    public ItemType type;
    private TextMeshProUGUI title;
    private Image image;
    private bool isDragging = false;

    void Awake()
    {
        image = transform.GetComponent<Image>();
        title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!isDragging)
        {
            slot = transform.parent.GetComponent<InventorySlot>();
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.3f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(slot.transform.parent.parent.parent.parent);
        transform.SetAsFirstSibling();
        image.raycastTarget = false;
        isDragging = true;
        InventoryManager.instance.isDragging = true;
        InventoryManager.instance.EnableHoverSwipers(true);

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Vector2.Lerp(transform.position, eventData.position, 0.3f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(OnEndDragEnum());
    }

    public IEnumerator OnEndDragEnum()
    {
        transform.SetParent(slot.transform);
        isDragging = false;
        yield return new WaitForSeconds(0.1f);
        image.raycastTarget = true;
        InventoryManager.instance.isDragging = false;
        InventoryManager.instance.EnableHoverSwipers(false);
    }

    public void SetData(InventorySlot slot, ArmorData data)
    {
        this.slot = slot;
        slot.item = this;
        armor = data;
        type = data.type == "Armor::Head" ? ItemType.Head : data.type == "Armor::Body" ? ItemType.Body : ItemType.Legs;
        title.text = data.name;
    }
}
