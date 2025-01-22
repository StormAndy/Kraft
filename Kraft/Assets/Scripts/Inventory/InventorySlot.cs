using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotItem inventorySlotItem;

    public Image image;
    public Color selectedColor, defaultColor;


    private void Awake()
    {
        
        if (image == null)
            image = GetComponent<Image>();

        image.color = defaultColor;
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = defaultColor;
    }

    /// <summary> On drop draggable check for Slot Item and child it to this slot if empty, then assign item data </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {

            InventorySlotItem _item = eventData.pointerDrag.GetComponent<InventorySlotItem>();
            if (_item != null)
            {
                // Empty old slot
                InventorySlot _parentSlot = _item.GetComponentInParent<InventorySlot>();
                if (_parentSlot != null)
                    _parentSlot.inventorySlotItem = null;

               //Asssign slot item instance to this slot data
                _item.parentAfterDrag = transform;
                inventorySlotItem = _item;
            }
            
        }
    }


}
