using ProjectModels;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private TextMeshProUGUI title;
    public ArmorData armorData;
    public string type;

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
        }

        if (transform.GetChild(0) != null)
        {
            title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            switch (type)
            {
                case "armor":
                    title.text = armorData.name;
                    break;
                default:
                    break;
            }
        }
    }

    public void SetItem(InventoryItem newItem)
    {
        switch (newItem.type)
        {
            case "armor":
                SetData(newItem.armorData);
                break;
        }
    }

    public void SetData(ArmorData data)
    {
        armorData = data;
        type = "armor";
    }
}
