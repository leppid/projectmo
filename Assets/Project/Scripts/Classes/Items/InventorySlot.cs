using System;
using ProjectModels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public GameObject itemPrefub;
    public InventoryItem item;

    public Transform moveIcon;
    public Transform replaceIcon;

    public Transform content;

    private bool longPressStarted = false;
    private float longPressTime = 0;
    private bool longPress = false;

    void Awake()
    {
        content = transform.GetChild(0);
        moveIcon = transform.GetChild(1).GetChild(0);
        replaceIcon = transform.GetChild(1).GetChild(1);
        moveIcon.gameObject.SetActive(false);
        replaceIcon.gameObject.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {
        if (longPressStarted)
        {
            longPressTime += Time.deltaTime;
            if (longPressTime > 0.5f)
            {
                OnLongPress();
            }
        }

        if (content.childCount > 0)
        {
            item = content.GetChild(0).GetComponent<InventoryItem>();
        }
    }

    public void SetItem(InventoryItem newItem)
    {
        if (content.childCount > 0)
        {
            RemoveItem();
        }

        InstantiateItem(newItem);
    }

    private void InstantiateItem(InventoryItem newItem)
    {
        GameObject go = Instantiate(itemPrefub);
        go.transform.parent = content.transform;
        go.transform.localScale = Vector3.one;
        var rectTransform = go.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(transform.parent.GetComponent<GridLayoutGroup>().cellSize.x, transform.parent.GetComponent<GridLayoutGroup>().cellSize.y);
            rectTransform.position = transform.position;
        }
        go.GetComponent<InventoryItem>().SetItem(newItem);
    }

    public void RemoveItem()
    {
        if (item == null) return;

        Destroy(item.gameObject);
        item = null;
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (longPress) return;

        if (InventoryManager.instance.isMoving)
        {
            InventoryManager.instance.moveTo = this;
        }
    }

    public void OnLongPress()
    {
        longPressStarted = false;
        longPress = true;

        if (item)
        {
            InventoryManager.instance.moveFrom = this;
        }
    }
}
