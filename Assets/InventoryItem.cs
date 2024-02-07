using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int itemAmount;
    public int itemValue;
    public bool itemUsable;
    public delegate void UseAction();
    public UseAction onUse;
    public bool isWeapon; // Indicate if it's a weapon
    public GameObject weaponPrefab; // Reference to the weapon prefab, optional
    public int currentAmmoInMagazine; // Current ammo in the magazine

    // Constructor for regular items
    public InventoryItem(string name, int amount, int value, UseAction useAction = null, bool usable = false)
    {
        itemName = name;
        itemAmount = amount;
        itemValue = value;
        itemUsable = usable;
        onUse = useAction;
        isWeapon = false;
        weaponPrefab = null;
    }

    // Additional constructor for weapon items
    public InventoryItem(string name, int amount, int value, GameObject prefab, int currentAmmo = 0) : this(name, amount, value)
    {
        weaponPrefab = prefab;
        currentAmmoInMagazine = currentAmmo;
        isWeapon = true; // Set isWeapon to true for weapon items
    }

}
