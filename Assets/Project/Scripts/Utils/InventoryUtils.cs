using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;

public class InventoryUtils
{
   public static  void UpdateInventoryIndex(string id, int index)
   {
       ItemData[] data = InventoryManager.instance.inventoryData;
       for (int i = 0; i < data.Length; i++)
       {
           if (data[i].id == id)
           {
               data[i].index = index.ToString();
           }
       }
       InventoryManager.instance.inventoryData = data;
       PlayerPrefs.SetString("inventoryJson", JsonConvert.SerializeObject(data));
   }
}
