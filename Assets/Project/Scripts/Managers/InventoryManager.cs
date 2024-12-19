using ProjectModels;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    private Transform _items;
    private Transform _hoverSwipeUp;
    private Transform _hoverSwipeDown;
    private Transform _pages;
    private Transform _slots;
    private Vector3 slotsInitialPosition;
    private Vector3 slotsPosition;
    private SwipeDetector swipeDetector;
    public int pageCount = 1;
    private int currentPage = 1;
    private bool animatePageSwitch = false;
    public bool isDragging = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _items = transform.GetChild(0).Find("Items");
        _hoverSwipeDown = _items.Find("HoverSwipeDown");
        _hoverSwipeUp = _items.Find("HoverSwipeUp");
        _hoverSwipeDown.gameObject.SetActive(false);
        _hoverSwipeUp.gameObject.SetActive(false);
        _pages = _items.Find("Pages");
        _slots = _items.Find("Slots").Find("Content");
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
    }

    public void Start()
    {
        ArmorData armorData = new() { id = "123", name = "shapka", type = "Armor::Head" };
        _slots.GetChild(0).GetComponent<InventorySlot>().SpawnItem(armorData);
        ArmorData armorData2 = new() { id = "1233", name = "shapka2", type = "Armor::Head" };
        _slots.GetChild(3).GetComponent<InventorySlot>().SpawnItem(armorData2);
    }

    public void OnSwipeUp()
    {
        NextPage();
    }

    public void NextPage(bool force = false)
    {
        if ((currentPage == pageCount) || (!force && isDragging)) return;
        currentPage++;
        UpdatePageCounter();
        slotsPosition = new Vector3(_slots.localPosition.x, _slots.localPosition.y + 930f, _slots.localPosition.z);
        swipeDetector.enabled = false;
        animatePageSwitch = true;
    }

    public void OnSwipeDown()
    {
        PrevPage();
    }

    public void PrevPage(bool force = false)
    {
        if ((currentPage == 1) || (!force && isDragging)) return;
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

    public void EnableHoverSwipers(bool enable)
    {
      _hoverSwipeDown.gameObject.SetActive(enable);
      _hoverSwipeUp.gameObject.SetActive(enable);
    }
}

