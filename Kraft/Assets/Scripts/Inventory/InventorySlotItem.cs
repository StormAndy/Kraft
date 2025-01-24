using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using static UnityEditor.Progress;

/// <summary> Manages the display of an individual inventory slot. </summary>
public class InventorySlotItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image itemImage; ///<summary> UI Image component to display the item icon. </summary>
    public TMP_Text itemNameText; ///<summary> UI Text component to display the item name. </summary>

    public ItemData itemData { get; private set; } ///<summary> Data associated with the item in this slot. </summary>
    public int stackSize;

    [HideInInspector] public Transform parentAfterDrag;

    public void AddToStack(int amount)
    {
        stackSize += amount;
        UpdateSlotUI();
    }


    /// <summary> Initializes the slot with the given item data. </summary>
    public void Initialize(ItemData item)
    {
        SetItemData(item);
    }

    public void SetItemData(ItemData item)
    {
        itemData = item;
        UpdateSlotUI();
    }

    /// <summary> Updates the UI elements to reflect the current item data. </summary>
    private void UpdateSlotUI()
    {
        //itemImage.sprite = itemData.icon;
        itemNameText.text = itemData.name;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = false;
        parentAfterDrag = transform.parent;

        // Empty old slot
        InventorySlot _parentSlot = this.GetComponentInParent<InventorySlot>();
        if (_parentSlot != null)
            _parentSlot.inventorySlotItem = null;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        InventorySlot _slot = parentAfterDrag.GetComponent<InventorySlot>();
        if(_slot != null)
            _slot.Assignitem(this);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
}
