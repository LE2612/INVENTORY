using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        protected UIInventoryPage inventoryUI;

        [SerializeField]
        protected InventorySO inventoryData;

        public List<InventoryItem> ListInventoryItems;

        [SerializeField]
        protected AudioClip dropClip;

        [SerializeField]
        protected AudioSource audioSource;

        protected virtual void Start()
        {
            ListInventoryItems = inventoryData.inventoryItems;
            PrepareUI();
           
        }
       
        protected virtual void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, 
                    item.Value.quantity);
            }
        }

        protected virtual void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        }

        protected virtual void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if(itemAction != null)
            {
                
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }

        }

        protected void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }

        protected void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        protected void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItem item1 = inventoryData.GetItemAt(itemIndex_1);
            InventoryItem item2 = inventoryData.GetItemAt(itemIndex_2);           
            if (itemIndex_1 >= 0 && itemIndex_1 < inventoryData.inventoryItems.Count &&
                itemIndex_2 >= 0 && itemIndex_2 < inventoryData.inventoryItems.Count)
            {                
                if (!item1.IsEmpty && !item2.IsEmpty && item1.item == item2.item)
                {                   
                    int totalQuantity = item1.quantity + item2.quantity;                 
                    int maxStackSize = item1.item.MaxStackSize;                  
                    if (totalQuantity > maxStackSize)
                    {
                        item1.quantity = maxStackSize;
                        item2.quantity = totalQuantity - maxStackSize;
                        inventoryData.inventoryItems[itemIndex_1] = item1;
                        inventoryData.inventoryItems[itemIndex_2] = item2;
                    }
                    else
                    {                        
                        item1.quantity = totalQuantity;
                        inventoryData.inventoryItems[itemIndex_1] = item1;
                        inventoryData.inventoryItems[itemIndex_2] = InventoryItem.GetEmptyItem();
                    }
                }
                    inventoryData.SwapItems(itemIndex_1, itemIndex_2);
            }
            else
            {
                Debug.Log("Invalid item indices for swapping.");
            }
        }

        protected void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
        }

        protected string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        
    }
}