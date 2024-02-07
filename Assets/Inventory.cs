using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemTemplate;
    private bool inventoryActive = false;
    private List<InventoryItem> items = new List<InventoryItem>();
    public Text ammoTextUI; // Add this line
    private int selectedItemIndex = -1;
    private List<Text> itemTextComponents = new List<Text>();
    private GameObject currentWeapon; // Reference to the currently equipped weapon
    public Transform playerTransform; // Assign this in the Unity Inspector
    public GameObject leftArm; // Assign this in the Unity Inspector
    public Text ammoUIText; // Assign this in the Unity Inspector to your UI Text element

    public void UnequipCurrentWeapon()
    {
        
        if (currentWeapon != null)
        {
            Debug.Log("log1");
            GunShooting gunShooting = currentWeapon.GetComponent<GunShooting>();
            if (gunShooting != null)
            {
                Debug.Log("log2");
                InventoryItem weaponItem = items.Find(item => {
                    Debug.Log("ItemLooped:");
                    Debug.Log(item.itemName);
                    Debug.Log("CurrWeapon:");
                    Debug.Log(currentWeapon.name);
                    return item.itemName == currentWeapon.name;
                    });
                Debug.Log("weaponItem:");
                Debug.Log(weaponItem.itemName);
                if (weaponItem != null)
                {
                    Debug.Log("log3");
                    Debug.Log($"[Unequip] Current Ammo before unequip: {gunShooting.GetCurrentAmmo()}");
                    weaponItem.currentAmmoInMagazine = gunShooting.GetCurrentAmmo();
                    Debug.Log($"[Unequip] Ammo in InventoryItem after update: {weaponItem.currentAmmoInMagazine}");

                    leftArm.SetActive(!leftArm.active);
                }
            }
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
    }

    public void EquipWeapon(InventoryItem weaponItem)
    {
        if (weaponItem.isWeapon && weaponItem.weaponPrefab != null)
        {
            GameObject weapon = Instantiate(weaponItem.weaponPrefab);
            weapon.name = weaponItem.itemName;
            weapon.transform.SetParent(playerTransform, false);

            GunShooting gunShooting = weapon.GetComponent<GunShooting>();
            if (gunShooting != null)
            {
                gunShooting.ammoText = ammoUIText;
                Debug.Log($"[Equip] Ammo in InventoryItem before equip: {weaponItem.currentAmmoInMagazine}");
                gunShooting.SetCurrentAmmo(weaponItem.currentAmmoInMagazine);
                gunShooting.UpdateAmmoUI();
                leftArm.SetActive(!leftArm.active);
            }

            UnequipCurrentWeapon();
            currentWeapon = weapon;
        }
    }
    // You can call EquipWeapon from UseSelectedItem when an item is a weapon

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActive = !inventoryActive;
            inventoryPanel.SetActive(inventoryActive);
             if (inventoryActive)
               UpdateInventoryDisplay();
            Debug.Log("inventory");
          
        }
        if (inventoryActive){
            if (Input.GetKeyDown(KeyCode.UpArrow)) // Scroll up
            {
                Debug.Log("upArrow");
                if (selectedItemIndex > 0)
                    selectedItemIndex--;
                UpdateSelectedItem();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) // Scroll down
            {

                Debug.Log("downArrow");
                if (selectedItemIndex < items.Count - 1)
                    selectedItemIndex++;
                UpdateSelectedItem();
            }

            if (Input.GetKeyDown(KeyCode.U)) // Use item
            {
                UseSelectedItem();
            }
            else if (Input.GetKeyDown(KeyCode.X)) // Drop item
            {
                DropSelectedItem();
            }
        }
    }

    public void AddItem(InventoryItem newItem)
    {
        // Check if the item already exists in the inventory
        InventoryItem existingItem = items.Find(item => item.itemName == newItem.itemName);
        if (existingItem != null)
        {
            // If it exists, increase its quantity
            existingItem.itemAmount += newItem.itemAmount;
        }
        else
        {
            // If it doesn't exist, add it as a new item
            items.Add(newItem);
        }
        if (inventoryActive) UpdateInventoryDisplay();
    }

    private void UpdateInventoryDisplay()
    {
        // Clear existing items in the UI and the list
        foreach (Transform child in itemTemplate.transform.parent)
        {
            if (child.gameObject != itemTemplate)
                Destroy(child.gameObject);
        }
        itemTextComponents.Clear(); // Clear the text components list

        // Add current items to the UI
        foreach (var inventoryItem in items)
        {
            GameObject newItem = Instantiate(itemTemplate, itemTemplate.transform.parent);
            Text itemText = newItem.GetComponentInChildren<Text>();
            itemText.text = $"{inventoryItem.itemName}x{inventoryItem.itemAmount} {inventoryItem.itemValue}g";
            itemTextComponents.Add(itemText); // Add to the text components list
            newItem.SetActive(true);
        }

        // Update selection after updating the display
        UpdateSelectedItem();
    }
    // EXTRA BELOW
    private void UpdateSelectedItem()
    {
        for (int i = 0; i < itemTextComponents.Count; i++)
        {
            if (i == selectedItemIndex)
            {
                itemTextComponents[i].color = Color.yellow; // Highlight selected item
            }
            else
            {
                itemTextComponents[i].color = Color.white; // Normal color for other items
            }
        }
    }

    private void UseSelectedItem()
    {
        
        if (selectedItemIndex >= 0 && selectedItemIndex < items.Count)
        {
            InventoryItem selectedItem = items[selectedItemIndex];
            Debug.Log(selectedItem.isWeapon);
            if (selectedItem.isWeapon)
            {
                Debug.Log("try equip wep");
                if (!currentWeapon)
                {
                    EquipWeapon(selectedItem);

                } else
                {
                    UnequipCurrentWeapon();

                }
            }
            // Check if the onUse delegate has any methods attached to it
            if (selectedItem.onUse != null && selectedItem.onUse.GetInvocationList().Length > 0 && selectedItem.itemUsable)
            {
                // Use the item
                selectedItem.onUse.Invoke();
                Debug.Log("Using item: " + selectedItem.itemName);

                // Decrement the item amount
                selectedItem.itemAmount--;

                // Remove the item if the amount reaches zero
                if (selectedItem.itemAmount <= 0)
                {
                    items.RemoveAt(selectedItemIndex);

                    // Adjust the selected index if necessary
                    if (selectedItemIndex >= items.Count)
                    {
                        selectedItemIndex = items.Count - 1;
                    }
                }

                // Update the inventory display
                UpdateInventoryDisplay();
            }
            else
            {
                Debug.Log("Item " + selectedItem.itemName + " has no use function attached.");
            }
        }
    }

    public InventoryItem GetItemByName(string itemName)
    {
        return items.Find(item => item.itemName == itemName);
    }
    private void DropSelectedItem()
    {
        if (selectedItemIndex >= 0)
        {
            Debug.Log("Dropping item: " + items[selectedItemIndex]);
            // Remove the item from the inventory
            items.RemoveAt(selectedItemIndex);
            UpdateInventoryDisplay();
        }
    }

}