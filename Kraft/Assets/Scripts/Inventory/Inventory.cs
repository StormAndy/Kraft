using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

/// <summary> Manages a collection of items in the inventory. </summary>
public class Inventory : MonoBehaviour, IContainer
{
    private int selectedSlot = -1;

    [SerializeField] public GameObject slotItemPrefab;

    public GameObject[] itemSlotsObjects; ///<summary> List of items in the inventory. </summary>
    [SerializeField]private Dictionary<int, InventorySlot> itemSlots = new Dictionary<int, InventorySlot>(); ///<summary> List of items in the inventory. </summary>


    private void ChangeSelectedSlot(int newSlotID)
    {
        //Deselect selected slot
        itemSlots.TryGetValue(selectedSlot, out InventorySlot _selectedSlot);
        if (_selectedSlot)
            _selectedSlot.Deselect();

        //Select new slot
        itemSlots.TryGetValue(newSlotID, out InventorySlot _slot);
        if (_slot)
            _slot.Select();
        selectedSlot = newSlotID;
    }

    /// <summary> Attempts to craft an item using a recipe, returns false if missing recipe ingredients or fails to retrieve item from database </summary>
    public bool TryCraft(RecipeData recipe)
    {
        if (!HasRequiredIngredients(recipe))
        { 
            Debug.Log("Not enough ingredients to craft " + recipe.recipeName);
            return false;
        }

        RemoveIngredients(recipe);

        // Retrieve the ItemData from the database
        ItemData craftedItem = Game.Instance.databaseManager.GetItemData(recipe.outputItemID);

        if (craftedItem == null)
        {
            Debug.LogError($"ItemData not found for ID: {recipe.outputItemID}. Crafting failed.");
            return false;
        }

        // Add the crafted item to the inventory
        AddItem(craftedItem, recipe.outputQuantity);

        Debug.Log($"Successfully crafted {craftedItem.name} (x{recipe.outputQuantity})");
        return true;
    }


    #region Setup...

    private void Start()
    {
        PopulateItemSlotDictionary();
        Invoke("AddTestItems", 3.0f);
    }

    /// <summary> Populate inventory slot lookup dictionary from slot prefabs assigned in itemSlotsObjects </summary>
    public void PopulateItemSlotDictionary()
    {
        int _slotcounter = 1;
        foreach (GameObject obj in itemSlotsObjects)
        {
            InventorySlot _slot = obj.GetComponent<InventorySlot>();
            if (_slot != null)
            {
                itemSlots.Add(_slotcounter, _slot);
                _slotcounter++;
            }
        }
    }
    
    private void AddTestItems()
    {
        AddItem(new ItemData()); AddItem(new ItemData()); AddItem(new ItemData()); AddItem(new ItemData());
        ChangeSelectedSlot(1);
    }

    #endregion

    #region Has... Inventory Queries

    /// <summary> (IContainer interface) Return true if space for stackables or free slots available for non stackables </summary>
    public bool HasInventorySpace(ItemData item, int quantity)
    {
        if(item == null) return false;

        if(item.isStackable == true)
            return HasSpaceForStackables(item, quantity); //Check for stackable space
        else
            return HasInventorySpace(quantity);
    }

    /// <summary> Return true if atleast one free slot available </summary>
    public bool HasInventorySpace()
    {
        foreach (var slot in itemSlots)
            if (slot.Value.inventorySlotItem == null)
                return true;

        return false;
    }

    /// <summary> Return true if atleast the Amount of slots specified empty </summary>
    public bool HasInventorySpace(int AmountOfSlots)
    {
        int _slotCounter = 0;
        foreach (var slot in itemSlots)
            if (slot.Value.inventorySlotItem == null)
                _slotCounter++;

        if (_slotCounter >= AmountOfSlots)
            return true;
        else
            return false;
    }

    /// <summary> Return trues if it has item with same uniqueID</summary>
    public bool HasItem(ItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem.itemData.uniqueID == item.uniqueID)
                return true;
        }
        return false;
    }

    /// <summary> Return trues if space for 1 stackable item </summary>
    public bool HasSpaceForStackable(ItemData item)
    {
        if (item.isStackable != true)
        {
            Debug.Log(item.name + " item is not stackable");
            return false;
        }

        // Search for stack with stack size lower than max size
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem.itemData.uniqueID == item.uniqueID)
                if (slot.Value.inventorySlotItem.stackSize < slot.Value.inventorySlotItem.itemData.maxStackSize)
                    return true;               
        }

        //If no stacks with room check for empty slot to create new stack if possible
        return HasInventorySpace();
    }



    /// <summary> Return trues if space for 1 stackable item </summary>
    public bool HasSpaceForStackables(ItemData item, int quantity)
    {
        if (item.isStackable == false)
        {
            Debug.Log(item.name + " item is not stackable");
            return false;
        }

        int _stackSpaceAvailable = 0;
        // Search for stack with stack size lower than max size and tally remaining space in all stacks
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem.itemData.uniqueID == item.uniqueID)
                if (slot.Value.inventorySlotItem.stackSize < slot.Value.inventorySlotItem.itemData.maxStackSize)
                    _stackSpaceAvailable += slot.Value.inventorySlotItem.itemData.maxStackSize - slot.Value.inventorySlotItem.stackSize;
        }

        //if enough space in stacks return out true, else check for more stack creation space
        if(_stackSpaceAvailable >= quantity)
            return true;
        else
        {
            int _spillover = quantity - _stackSpaceAvailable;
            //Can create 1 new stack if smaller than 1 stack in size
            if (_spillover >= item.maxStackSize)
                return HasInventorySpace();
            else
            {
                //Work out how many stack slots needed, then check for multiple slot space
                int _numberOfStacksNeeded = Mathf.CeilToInt((float)_spillover / (float)item.maxStackSize);
                return HasInventorySpace(_numberOfStacksNeeded);
            }
        }
    }

    /// <summary> Checks if the inventory contains the required ingredients for a recipe. </summary>
    public bool HasRequiredIngredients(RecipeData recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int count = 0;
            foreach (var slot in itemSlots)
                if (slot.Value.inventorySlotItem != null && slot.Value.inventorySlotItem.itemData.uniqueID == ingredient.itemID)
                    count += slot.Value.inventorySlotItem.stackSize;

            if (count < ingredient.quantity)
                return false; // Not enough of this ingredient
        }
        return true; // All ingredients are available
    }

    #endregion

    #region Add Items...

    /// <summary> Adds stackables to existing stacks first. Then to the first available slot inventory, creates ItemSlotItem for ItemData and assigns to empty Itemslot comp. Overflow method is then called to handle placing items to game world if inventory full  </summary>
    public void AddItem(ItemData item, int amount = 1)
    {
        //Autoplace in stacks first
        if (item.isStackable == true)
        {
            //Place in existing stacks
            int _itemsRemaining = amount;
            // Search for stack with stack size lower than max size and tally remaining space in all stacks
            foreach (var slot in itemSlots)
            {
                if (slot.Value.inventorySlotItem.itemData.uniqueID == item.uniqueID)
                    if (slot.Value.inventorySlotItem.stackSize < slot.Value.inventorySlotItem.itemData.maxStackSize)
                    {
                        int stackSpace = slot.Value.inventorySlotItem.itemData.maxStackSize - slot.Value.inventorySlotItem.stackSize;
                        int stackIncrease = stackSpace;
                        
                        //if less space to fill than amount to place fill to max, else use remainder
                        if(amount >= stackSpace)
                            amount -= stackSpace;
                        else
                            stackIncrease = amount;

                        //Add the stack amount to the slot item component
                        InventorySlotItem _slotitem = slot.Value.GetComponent<InventorySlotItem>();
                        if (_slotitem != null)
                            _slotitem.AddToStack(stackIncrease);
                        else
                            Debug.LogError("failed to access InventorySlotItem Component when AddItem stackable item to slot " + slot.Key);

                    }
                //Break out if no more items to place
                if (_itemsRemaining <= 0)
                    break;
            }

            //Form new stacks for items remaining
            if (_itemsRemaining > 0)
            {
                //Work out how many stack slots needed, then check for multiple slot space
                int _numberOfStacksNeeded = Mathf.CeilToInt((float)_itemsRemaining / (float)item.maxStackSize);

                if (_numberOfStacksNeeded == 1)
                {
                    //only create 1 stack with items remaining
                    foreach (var slot in itemSlots)
                    {
                        if (slot.Value.inventorySlotItem == null)
                        {
                            GameObject _newObj = Instantiate(slotItemPrefab, slot.Value.transform);
                            slot.Value.inventorySlotItem = _newObj.GetComponent<InventorySlotItem>();
                            slot.Value.inventorySlotItem.SetItemData(item);
                            slot.Value.inventorySlotItem.stackSize = _itemsRemaining;
                            _itemsRemaining = 0;
                            break;
                        }
                    }
                }
                else
                {
                    // Handle multiple stacks
                    for (int i = 0; i < _numberOfStacksNeeded; i++)
                    {
                        //if not last stack use maxStackSize, else last stack and use remainder
                        int stackToAdd = (i == _numberOfStacksNeeded - 1) ? _itemsRemaining : item.maxStackSize;

                        foreach (var slot in itemSlots)
                        {
                            if (slot.Value.inventorySlotItem == null)
                            {
                                GameObject _newObj = Instantiate(slotItemPrefab, slot.Value.transform);
                                slot.Value.inventorySlotItem = _newObj.GetComponent<InventorySlotItem>();
                                slot.Value.inventorySlotItem.SetItemData(item);
                                slot.Value.inventorySlotItem.stackSize = stackToAdd;

                                _itemsRemaining -= stackToAdd;
                                break;
                            }
                        }

                        // If inventory is full before all stacks are placed, handle overflow
                        if (_itemsRemaining > 0 && i == _numberOfStacksNeeded - 1)
                            Overflow(item, _itemsRemaining);
                    }
                }
            }
        }
        else
        {
            //Find next empty slot to autoplace non stackables to
            foreach (var slot in itemSlots)
            {

                if (slot.Value.inventorySlotItem == null)
                {
                    Debug.Log(slot.Value.name + " slot value exists, instantiate new item prefab");
                    GameObject _newObj = Instantiate(slotItemPrefab, slot.Value.transform);

                    slot.Value.inventorySlotItem = _newObj.GetComponent<InventorySlotItem>();
                    if (slot.Value.inventorySlotItem != null)
                        slot.Value.inventorySlotItem.SetItemData(item);


                    return;
                }
                Debug.Log("slot is not empty, check next slot");
            }
            Debug.Log("No slots available, add to Drop List?");
            Overflow(item);
        }
    }



    #endregion

    #region Remove items...

    /// <summary> Removes an item from the inventory. </summary>
    public void RemoveItem(ItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem != null)
            {
                if (slot.Value.inventorySlotItem.itemData == item)
                {
                    Debug.Log(item.name + " removed item at slot: " + slot.Value);
                    Destroy(slot.Value.gameObject);
                }
            }
        }
    }

    /// <summary> Removes the items from the item slot specified </summary>
    public void RemoveItemAtSlot(InventorySlot slot)
    {
        Debug.Log("TO DO: implement destroying an item at a slot");
    }

    /// <summary> Removes the required ingredients for a recipe from the inventory. </summary>
    public void RemoveIngredients(RecipeData recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int quantityToRemove = ingredient.quantity;

            foreach (var slot in itemSlots)
            {
                if (slot.Value.inventorySlotItem != null && slot.Value.inventorySlotItem.itemData.uniqueID == ingredient.itemID)
                {
                    if (slot.Value.inventorySlotItem.stackSize >= quantityToRemove)
                    {
                        slot.Value.inventorySlotItem.stackSize -= quantityToRemove;
                        if (slot.Value.inventorySlotItem.stackSize == 0)
                            Destroy(slot.Value.inventorySlotItem.gameObject); // Clear slot
                        break;
                    }
                    else
                    {
                        quantityToRemove -= slot.Value.inventorySlotItem.stackSize;
                        Destroy(slot.Value.inventorySlotItem.gameObject); // Clear slot
                    }
                }
            }
        }
    }

#endregion



    /// <summary> Handles item overflow by dropping item to ground at active character position </summary>
    public void Overflow(ItemData item, int amount = 1)
    {
        //Instantiate pickup
        GameObject _dropitemObject = GameObject.Instantiate(Game.Instance.prefabPickup);
        _dropitemObject.transform.position = Game.Instance.activeCharacter.transform.position;

        //Send item data and setup pickup
        ItemPickup _pickup = _dropitemObject.GetComponent<ItemPickup>();
        if (_pickup != null)
            _pickup.SetItemData(item);
    }


    /// <summary> Retrieves all items in the inventory to manage as a List indenpendant of Slots </summary>
    public List<ItemData> GetItemList()
    {
        Debug.Log("TO DO: implement returning an item list from dictionary items");
        return null;
    }

    #region Convert To

    /// <summary> Converts the current inventory into an <see cref="ItemSlotContainer"/>, which can then be converted to a serializable format for saving. </summary>
    /// <returns>A populated ItemSlotContainer representing the inventory contents.</returns>
    public ItemSlotContainer ConvertToItemSlotContainer()
    {
        // Create a new container with a max slot count equal to the number of UI slots.
        ItemSlotContainer container = new ItemSlotContainer(itemSlots.Count);
        foreach (KeyValuePair<int, InventorySlot> kvp in itemSlots)
        {
            InventorySlot slot = kvp.Value;
            if (slot.inventorySlotItem != null)
            {
                // Add the item data and its quantity to the container.
                container.AddItem(slot.inventorySlotItem.itemData, slot.inventorySlotItem.stackSize);
            }
        }
        return container;
    }

    #endregion


}

