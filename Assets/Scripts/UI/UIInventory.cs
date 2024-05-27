using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    
    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipeButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIndex;

    int curEquipIndex;

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;
        controller.inventory +=Toggle;
        CharacterManager.Instance.Player.addItem+=AddItem;

        inventoryWindow.SetActive(false);   
        slots = new ItemSlot[slotPanel.childCount];

        for(int i=0;i<slots.Length;i++){
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }
        ClearSelectedItemWindow();
    }

    void ClearSelectedItemWindow(){
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipeButton.SetActive(false);
        dropButton.SetActive(false);

    }

    public void Toggle(){
        if(IsOpen()){
            inventoryWindow.SetActive(false);
        }else{
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen(){
        return inventoryWindow.activeInHierarchy;
    }

    void AddItem(){
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.cansStack){
            ItemSlot slot = GetItemStack(data);
            if(slot!=null){
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData=null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot!=null){
            emptySlot.item=data;
            emptySlot.quantity=1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData=null;
    }

    private void UpdateUI()
    {
        foreach (ItemSlot slot in slots)
        {
            if (slot.item != null)
            {
                slot.Set();
            }
            else
            {
                slot.Clear();
            }
        }
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        for(int i=0; i<slots.Length ; i++){
            if(slots[i].item == data && slots[i].quantity<data.maxStackAmount){
                return slots[i];
            }
        }
        return null;
    }
    ItemSlot GetEmptySlot(){
        for(int i=0; i<slots.Length; i++){
            if(slots[i].item ==null){
                return slots[i];
            }
        }
        return null;
    }
    void ThrowItem(ItemData data){
        Instantiate(data.dropPrefab,dropPosition.position,Quaternion.Euler(Vector3.one*Random.value*360));
    }

    public void SelectItem(int index){
        if(slots[index].item == null)return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text =  selectedItem.description;
        
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for(int i=0; i<selectedItem.consumables.Length;i++){ //consum
            selectedStatName.text += selectedItem.consumables[i].type.ToString()+"\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipeButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton(){
        if(selectedItem.type==ItemType.Consumable){
            for(int i=0; i<selectedItem.consumables.Length;i++){
                switch(selectedItem.consumables[i].type){
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropButton(){
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem(){
        slots[selectedItemIndex].quantity--;

        if(slots[selectedItemIndex].quantity<=0){
            selectedItem =null;
            slots[selectedItemIndex].item =null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }

    public void OnEquipButton(){
        if(slots[curEquipIndex].equipped){
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    private void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index){
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton(){
        UnEquip(selectedItemIndex);
    }
}
