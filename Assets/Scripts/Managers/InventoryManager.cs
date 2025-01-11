using System;
using Newtonsoft.Json;
using ProjectModels;
using Proyecto26;
using TMPro;
using UnityEngine;

enum InventoryTab { Equip, Stats }

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public ItemData[] inventoryData = new ItemData[0];
    private Transform _inventory;
    private Transform _equipMover;
    private Transform _statsMover;
    private Transform _item;
    private Transform _itemInfo;
    private Transform _itemHeader;
    private Transform _itemStats;
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
    public bool isDrag = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _inventory = transform.root.Find("InterfaceWorld").Find("Canvas").Find("Inventory");
        _equipMover = _inventory.Find("EquipMover");
        _statsMover = _inventory.Find("StatsMover");
        _item = _inventory.Find("Item");
        _itemInfo = _item.Find("Info");
        _itemHeader = _itemInfo.Find("Header");
        _itemStats = _itemInfo.Find("Stats");
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
    }

    public void Start()
    {
        SpawnInventory();
        UpdatePageCounter();
        _stats.Find("Title").GetComponent<TextMeshProUGUI>().text = PlayerManager.instance.PlayerData.displayName + "'s Stats";
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

        string inventoryString = PlayerPrefs.GetString("inventoryJson", "null");

        if (PlayerPrefs.GetString("authToken", "null") == "null")
        {
            ItemData item1 = new() { id = "1", name = "Helmet", type = "Head", index = "0" };
            ItemData item2 = new() { id = "2", name = "Armor", type = "Body", index = "1" };
            ItemData item3 = new() { id = "3", name = "Boots", type = "Legs", index = "2" };
            ItemData item4 = new() { id = "4", name = "Shield", type = "Secondary", index = "3" };
            ItemData item5 = new() { id = "5", name = "Sword", type = "Primary", index = "4" };
            ItemData[] testItems = new ItemData[] { item1, item2, item3, item4, item5 };
            inventoryString = JsonConvert.SerializeObject(testItems);
        }

        if (inventoryString == "null") return;

        inventoryData = JsonConvert.DeserializeObject<ItemData[]>(inventoryString);

        for (int i = 0; i < inventoryData.Length; i++)
        {
            int slotIndex = int.Parse(inventoryData[i].index);
            if (slotIndex >= 0)
            {
                _bagSlots.GetChild(slotIndex).GetComponent<InventorySlot>().CreateItem(inventoryData[i]);
            }
            else
            {
                _equipSlots.Find(inventoryData[i].type).GetComponent<InventorySlot>().CreateItem(inventoryData[i]);
            }
        }
    }

    public void SyncInventory(Action callback = null)
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null")
        {
            callback?.Invoke();
            return;
        };

        ApiManager.instance.Post<ResponseHelper>("player/sync", new PlayerParams { inventory = inventoryData }).Then(res =>
        {
            ApiManager.instance.GetArray<ItemData>("player/inventory").Then(res =>
            {
                PlayerPrefs.SetString("inventoryJson", JsonConvert.SerializeObject(res));
                callback?.Invoke();

            }).Catch(err =>
            {
                callback?.Invoke();
            });
        }).Catch(err =>
        {
            RequestException error = (RequestException)err.GetBaseException();
            if (error.IsNetworkError)
            {
                PlayerPrefs.SetString("errorMessage", "Connection lost.");
                PlayerPrefs.SetString("isLogout", "true");
                GameManager.instance.LoadLevel("Login", false);
                return;
            }
            callback?.Invoke();
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
        if ((currentBagPage == bagPageCount) || (!force && isDrag)) return;
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
        if ((currentBagPage == 1) || (!force && isDrag)) return;
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

    public void OpenItemInfo(InventoryItem item)
    {
        _itemHeader.Find("name").GetComponent<TextMeshProUGUI>().text = item.item.name;

        TextMeshProUGUI mindmg = _itemStats.Find("mindmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI maxdmg = _itemStats.Find("maxdmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI crtdmg = _itemStats.Find("crtdmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI crtcnc = _itemStats.Find("crtcnc").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hp = _itemStats.Find("hp").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ep = _itemStats.Find("ep").GetComponent<TextMeshProUGUI>();

        mindmg.text = mindmg.text.Replace("{{mindmg}}", "0");
        maxdmg.text = maxdmg.text.Replace("{{maxdmg}}", "0");
        crtdmg.text = crtdmg.text.Replace("{{crtdmg}}", "0");
        crtcnc.text = crtcnc.text.Replace("{{crtcnc}}", "0");
        hp.text = hp.text.Replace("{{hp}}", "0");
        ep.text = ep.text.Replace("{{ep}}", "0");

        _item.gameObject.SetActive(true);
    }

    public void CloseItemInfo()
    {
        _item.gameObject.SetActive(false);

        TextMeshProUGUI mindmg = _itemStats.Find("mindmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI maxdmg = _itemStats.Find("maxdmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI crtdmg = _itemStats.Find("crtdmg").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI crtcnc = _itemStats.Find("crtcnc").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI hp = _itemStats.Find("hp").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ep = _itemStats.Find("ep").GetComponent<TextMeshProUGUI>();

        mindmg.text = "+ {{mindmg}} to <color=orange>Min Damage";
        maxdmg.text = "+ {{maxdmg}} to <color=orange>Max Damage";
        crtdmg.text = "+ {{crtdmg}} % to <color=orange>Crit Damage";
        crtcnc.text = "+ {{crtcnc}} % to <color=orange>Crit Chance";
        hp.text = "+ {{hp}} to <color=orange>Health";
        ep.text = "+ {{ep}} to <color=orange>Evasion";
    }
}