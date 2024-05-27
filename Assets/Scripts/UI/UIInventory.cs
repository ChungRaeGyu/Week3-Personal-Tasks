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
}
