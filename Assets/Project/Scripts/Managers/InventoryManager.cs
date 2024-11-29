using System.Net.Sockets;
using ProjectModels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    private Transform _items;
    private Transform _pages;
    private Transform _slots;
    private Vector3 slotsInitialPosition;
    private Vector3 slotsPosition;
    private SwipeDetector swipeDetector;
    public int pageCount = 1;
    private int currentPage = 1;
    private bool animatePageSwitch = false;

    public bool isMoving = false;
    public bool iconsShown = false;
    public InventorySlot moveFrom;
    public InventorySlot moveTo;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _items = transform.GetChild(0).GetChild(1);
        _pages = _items.GetChild(0);
        _slots = _items.GetChild(1).GetChild(0);
        slotsInitialPosition = _slots.localPosition;
        slotsPosition = slotsInitialPosition;
        pageCount = _slots.childCount / 12;
        UpdatePageCounter();
        swipeDetector = _items.GetComponent<SwipeDetector>();
        swipeDetector.OnSwipeUpD += OnSwipeUp;
        swipeDetector.OnSwipeDownD += OnSwipeDown;

    }

    public void Update()
    {
        if (animatePageSwitch)
        {
            _slots.localPosition = Vector3.Lerp(_slots.localPosition, slotsPosition, 0.30f);
            if ((int)_slots.localPosition.y == (int)slotsPosition.y)
            {
                swipeDetector.enabled = true;
                animatePageSwitch = false;
            }
        };

        if (moveFrom)
        {
            isMoving = true;
            moveFrom.transform.localScale = Vector3.one * 0.9f;
            ShowMoveIcons(true);
        }

        if (moveFrom && moveTo)
        {
            MoveItem();
            isMoving = false;
        }
    }

    public void Start()
    {
        ArmorData armorData = new() { id = "123", name = "shapka" };
        InventoryItem invItem = new() { armorData = armorData, type = "armor" };
        _slots.GetChild(0).GetComponent<InventorySlot>().SetItem(invItem);
        ArmorData armorData2 = new() { id = "1233", name = "shapka2" };
        InventoryItem invItem2 = new() { armorData = armorData2, type = "armor" };
        _slots.GetChild(3).GetComponent<InventorySlot>().SetItem(invItem2);
    }

    public void OnSwipeUp()
    {
        if (currentPage == pageCount) return;
        currentPage++;
        UpdatePageCounter();
        slotsPosition = new Vector3(_slots.localPosition.x, _slots.localPosition.y + 930f, _slots.localPosition.z);
        swipeDetector.enabled = false;
        animatePageSwitch = true;
    }

    public void OnSwipeDown()
    {
        if (currentPage == 1) return;
        currentPage--;
        UpdatePageCounter();
        slotsPosition = new Vector3(_slots.localPosition.x, _slots.localPosition.y - 930f, _slots.localPosition.z);
        swipeDetector.enabled = false;
        animatePageSwitch = true;
    }

    public void UpdatePageCounter()
    {
        _pages.GetComponent<TextMeshProUGUI>().text = "" + currentPage + " / " + pageCount;
    }

    public void ResetPages()
    {
        _slots.localPosition = slotsInitialPosition;
        slotsPosition = slotsInitialPosition;
        currentPage = 1;
        UpdatePageCounter();
    }

    public void MoveItem()
    {
        moveFrom.transform.localScale = Vector3.one;
        ShowMoveIcons(false);

        if (moveFrom == moveTo)
        {
            ResetMovement();
            return;
        };

        if (moveFrom.content.childCount > 0 && moveTo.content.childCount == 0)
        {
            moveFrom.content.GetChild(0).SetParent(moveTo.content.transform);
        }
        else
        {
            moveFrom.content.GetChild(0).SetParent(moveTo.content.transform);
            moveTo.content.GetChild(0).SetParent(moveFrom.content.transform);
        }

        ResetMovement();
    }

    public void ResetMovement()
    {
        moveFrom = null;
        moveTo = null;
        isMoving = false;
        ShowMoveIcons(false);
    }

    public void ShowMoveIcons(bool show = true)
    {
        iconsShown = show;
        for (int i = 0; i < _slots.childCount; i++)
        {
            InventorySlot slot = _slots.GetChild(i).GetComponent<InventorySlot>();
            if (show)
            {
                if (slot.content.childCount > 0)
                {
                    if (slot != moveFrom) slot.moveIcon.gameObject.SetActive(true);
                }
                else
                {
                    if (slot != moveFrom) slot.replaceIcon.gameObject.SetActive(true);
                }
            }
            else
            {
                slot.moveIcon.gameObject.SetActive(false);
                slot.replaceIcon.gameObject.SetActive(false);
            }
        }
    }

}

