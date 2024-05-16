using Inventory;
using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryControllerChildren : InventoryController
{
    [SerializeField]
    private InventorySO secondaryInventoryData;

    protected override void Start()
    {
        base.Start();
        
    }
    private void MoveItemBetweenInventories(InventorySO InventoryData, int itemIndex)
    {
        bool success = inventoryData.MoveItemToInventory(InventoryData, itemIndex);
        if (success)
        {
            UpdateInventoryUI(inventoryData.GetCurrentInventoryState());
            // Optionally update the secondary inventory UI if it's displayed
        }
    }
    protected override void HandleItemActionRequest(int itemIndex)
    {
        base.HandleItemActionRequest(itemIndex);

        inventoryUI.AddAction("Move to I2", () => MoveItemBetweenInventories(secondaryInventoryData, itemIndex));


    }
}
