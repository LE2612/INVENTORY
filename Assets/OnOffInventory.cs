using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffInventory : MonoBehaviour
{
    [SerializeField]
    protected UIInventoryPage inventoryUI1;

    [SerializeField]
    protected UIInventoryPage inventoryUI2;
    
    [SerializeField]
    protected InventorySO inventoryData1;

    [SerializeField]
    protected InventorySO inventoryData2;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI1.isActiveAndEnabled == false)
            {
                inventoryUI1.Show();
                foreach (var item in inventoryData1.GetCurrentInventoryState())
                {
                    inventoryUI1.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventoryUI1.Hide();
            }

        } if (Input.GetKeyDown(KeyCode.O))
        {
            if (inventoryUI2.isActiveAndEnabled == false)
            {
                inventoryUI2.Show();
                foreach (var item in inventoryData2.GetCurrentInventoryState())
                {
                    inventoryUI2.UpdateData(item.Key,
                        item.Value.item.ItemImage,
                        item.Value.quantity);
                }
            }
            else
            {
                inventoryUI2.Hide();
            }

        }

    }
}
