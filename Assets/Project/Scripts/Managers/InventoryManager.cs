using System;
using System.Collections;
using Newtonsoft.Json;
using ProjectModels;
using Proyecto26;
using TMPro;
using UnityEngine;

enum InventoryTab { Equip, Stats }

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public ItemData[] inventoryData;
    private Transform _inventory;
    private Transform _equipMover;
    private Transform _statsMover;
    private Transform _stats;
    private Transform _char;
    private Transform _equip;
    private Transform _bag;
    private Transform _equipSlots;
    private Transform _bagSlots;
    private Transform _bagPages;
    private Transform _bagHoverSwipeUp;
    private Transform _bagHoverSwipeDown;
    private Vector3 bagSlotsInitPos;
    private Vector3 bagSlotsPos;
    private SwipeDetector swipeDetector;
    private InventoryTab currentTab = InventoryTab.Equip;
    private int bagPageCount = 1;
    private int currentBagPage = 1;
    private bool animateBagPageSwitch = false;
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

        _inventory = GameObject.Find("InterfaceWorld").transform.Find("Canvas").Find("Inventory");
        _equipMover = _inventory.Find("EquipMover");
        _statsMover = _inventory.Find("StatsMover");
        _char = _equipMover.Find("Char");
        _equip = _equipMover.Find("Equip");
        _bag = _inventory.Find("Bag");
        _equipSlots = _equip.Find("Slots");
        _stats = _statsMover.Find("Stats");
        _bagSlots = _bag.Find("Slots").Find("Content");
        _bagPages = _bag.Find("Pages");
        _bagHoverSwipeDown = _bag.Find("HoverSwipeDown");
        _bagHoverSwipeUp = _bag.Find("HoverSwipeUp");
        _bagHoverSwipeDown.gameObject.SetActive(false);
        _bagHoverSwipeUp.gameObject.SetActive(false);
        bagSlotsInitPos = _bagSlots.localPosition;
        bagSlotsPos = bagSlotsInitPos;
        swipeDetector = _inventory.GetComponent<SwipeDetector>();
        swipeDetector.OnSwipeUpD += OnSwipeUp;
        swipeDetector.OnSwipeDownD += OnSwipeDown;
        swipeDetector.OnSwipeLeftD += OnSwipeLeft;
        swipeDetector.OnSwipeRightD += OnSwipeRight;
        SpawnInventory();
    }

    public void Start()
    {
        _stats.Find("Title").GetComponent<TextMeshProUGUI>().text = PlayerManager.instance.PlayerData.displayName + "'s Stats";
        UpdatePageCounter();
    }

    public void Update()
    {
        if (animateBagPageSwitch)
        {
            _bagSlots.localPosition = Vector3.Lerp(_bagSlots.localPosition, bagSlotsPos, 0.30f);
            if ((int)_bagSlots.localPosition.y == (int)bagSlotsPos.y)
            {
                swipeDetector.enabled = true;
                animateBagPageSwitch = false;
            }
        };

        _equipMover.localPosition = Vector3.Lerp(_equipMover.localPosition, new Vector3(currentTab == InventoryTab.Equip ? 0f : -1500f, _equipMover.localPosition.y, 0f), 0.2f);
        _statsMover.localPosition = Vector3.Lerp(_statsMover.localPosition, new Vector3(currentTab == InventoryTab.Stats ? 0f : 1500f, _statsMover.localPosition.y, 0f), 0.2f);
    }

    public void SpawnInventory()
    {
        for (int i = 0; i < _bagSlots.childCount; i++)
        {
            InventorySlot slot = _bagSlots.GetChild(i).GetComponent<InventorySlot>();
            slot.RemoveItem();
        }
        for (int i = 0; i < _equipSlots.childCount - 1; i++)
        {
            InventorySlot slot = _equipSlots.GetChild(i).GetComponent<InventorySlot>();
            slot.RemoveItem();
        }

        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        ApiManager.instance.GetArray<ItemData>("inventory")
        .Then(res =>
        {
            inventoryData = res;
            PlayerPrefs.SetString("inventoryJson", JsonConvert.SerializeObject(res));
            for (int i = 0; i < res.Length; i++)
            {
                int slotIndex = int.Parse(res[i].index);
                if (slotIndex >= 0)
                {
                    _bagSlots.GetChild(slotIndex).GetComponent<InventorySlot>().CreateItem(res[i]);
                }
                else
                {
                    _equipSlots.Find(res[i].type).GetComponent<InventorySlot>().CreateItem(res[i]);
                }
            }
        });
    }

    public void SyncInventory()
    {
        ApiManager.instance.Post<ResponseHelper>("sync_inventory", new InventorySyncParams { data = inventoryData }).Then(res =>
        {
            Debug.Log("Inventory Synced");
        }).Catch(err =>
        {
            Debug.Log("Inventory Sync Failed");
        });
    }

    public void OnSwipeLeft()
    {
        currentTab = InventoryTab.Stats;
    }

    public void OnSwipeRight()
    {
        currentTab = InventoryTab.Equip;
    }


    public void HandleEquipSlots()
    {
        bool enable = _equip.gameObject.activeSelf;
        _equip.gameObject.SetActive(!enable);
        _char.GetComponent<CanvasGroup>().alpha = !enable ? 0.1f : 1f;
    }

    public void OnSwipeUp()
    {
        NextPage();
    }

    public void NextPage(bool force = false)
    {
        if ((currentBagPage == bagPageCount) || (!force && isDragging)) return;
        currentBagPage++;
        UpdatePageCounter();
        bagSlotsPos = new Vector3(_bagSlots.localPosition.x, _bagSlots.localPosition.y + 930f, _bagSlots.localPosition.z);
        swipeDetector.enabled = false;
        animateBagPageSwitch = true;
    }

    public void OnSwipeDown()
    {
        PrevPage();
    }

    public void PrevPage(bool force = false)
    {
        if ((currentBagPage == 1) || (!force && isDragging)) return;
        currentBagPage--;
        UpdatePageCounter();
        bagSlotsPos = new Vector3(_bagSlots.localPosition.x, _bagSlots.localPosition.y - 930f, _bagSlots.localPosition.z);
        swipeDetector.enabled = false;
        animateBagPageSwitch = true;
    }

    public void UpdatePageCounter()
    {
        bagPageCount = PlayerManager.instance.PlayerData.bagPages;
        _bagPages.GetComponent<TextMeshProUGUI>().text = "" + currentBagPage + " / " + bagPageCount;
    }

    public void ResetPages()
    {
        _bagSlots.localPosition = bagSlotsInitPos;
        bagSlotsPos = bagSlotsInitPos;
        currentBagPage = 1;
        UpdatePageCounter();
    }

    public void EnableHoverSwipers(bool enable)
    {
        _bagHoverSwipeDown.gameObject.SetActive(enable);
        _bagHoverSwipeUp.gameObject.SetActive(enable);
        swipeDetector.enabled = !enable;
    }

    public InventorySlot GetEquipSlotByType(string type)
    {
        return _equipSlots.Find(type).GetComponent<InventorySlot>();
    }

    public InventorySlot GetFirstEmptyBagSlot()
    {
        for (int i = 0; i < _bagSlots.childCount; i++)
        {
            if (_bagSlots.GetChild(i).GetComponent<InventorySlot>().item == null) return _bagSlots.GetChild(i).GetComponent<InventorySlot>();
        }
        return null;
    }

    public void OnApplicationPause()
    {
        SyncInventory();
    }
}