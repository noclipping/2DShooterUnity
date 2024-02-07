using UnityEngine;
using UnityEngine.UI; // Include this if using regular UI Text
using UnityEngine.Events;
public class PickupItem : MonoBehaviour
{
    private bool isPlayerClose = false;
    public static PickupItem activeItem = null; // To keep track of the active item
    public int amount = 1;
    public int worth = 1;
    public bool usable = false;
    public GameObject pickupPrompt; // Assign this in the Inspector
    public UnityEvent onUse;
    public GameObject weaponPrefab;
    public int initialAmmoCount = 30; // Set initial ammo count for weapons
    public bool isWeapon = false;
    void Update()
    {
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E) && activeItem == this)
        {
            PickUp();
            activeItem = null; // Reset the active item
            pickupPrompt.gameObject.SetActive(false); // Hide the prompt
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (activeItem == null)
            {
                activeItem = this; // Set this item as the active one
                UpdatePickupPromptPosition(); // Update the prompt's position
                pickupPrompt.gameObject.SetActive(true); // Show the prompt
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerClose = false;
            if (activeItem == this)
            {
                activeItem = null; // Reset the active item
                pickupPrompt.gameObject.SetActive(false); // Hide the prompt
            }
        }
    }
    
    private void PickUp()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        GunShooting gunShooting = FindObjectOfType<GunShooting>(); // Find the GunShooting script

        if (inventory != null)
        {
            string theName = this.name;

            if (theName == "5.56mm") // Replace with your ammo type names as needed
            {
                // Handle ammo pickup
                InventoryItem newItem = new InventoryItem(theName, amount, worth);
                inventory.AddItem(newItem);

                if (gunShooting != null)
                {
                    gunShooting.UpdateAmmoUI(); // Update ammo UI when ammo is picked up
                }
            }
            else if (isWeapon)
            {
                Debug.Log(isWeapon);
                Debug.Log("weapon pickedup");
                // Create a new InventoryItem for the weapon
                InventoryItem newItem = new InventoryItem(this.name, 1, worth, weaponPrefab, initialAmmoCount) { isWeapon = true };
                inventory.AddItem(newItem);
            }
            else
            {
                // Handle regular item pickup
                InventoryItem newItem = new InventoryItem(theName, amount, worth, () => {
                    Debug.Log($"Item Used: {theName}");
                    onUse.Invoke();
                }, usable);
                inventory.AddItem(newItem);
            }

            Destroy(gameObject); // Destroy the item after it's picked up
        }
    }

    private void UpdatePickupPromptPosition()
    {
        if (pickupPrompt != null)
        {
            Vector3 offset = new Vector3(0, 1.0f, 0); // Adjust as needed
            Vector3 worldPosition = transform.position + offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            pickupPrompt.transform.position = screenPosition;
        }
    }
}
