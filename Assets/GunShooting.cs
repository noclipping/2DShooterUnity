using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GunShooting : MonoBehaviour
{
    public enum FireMode { SemiAutomatic, Automatic }
    public FireMode fireMode = FireMode.SemiAutomatic;
    public Transform barrelEnd;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.2f; // Time in seconds between shots for automatic fire
    public int magazineSize = 30;
    private int currentAmmoInMagazine;
    public string ammoType = "5.56mm"; // Example ammo type
    public Text ammoText; // Assign in the inspector
    private float timeSinceLastShot = 0f;
    private GunShooting currentGun;
    public AudioSource gunShot;
    public AudioSource gunReload;
    private bool isReloading = false;

    public float reloadTime = 3.2f; // Reload time in seconds

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        switch (fireMode)
        {
            case FireMode.SemiAutomatic:
                if (Input.GetButtonDown("Fire1") && timeSinceLastShot >= fireRate)
                {
                    Shoot();
                }
                break;
            case FireMode.Automatic:
                if (Input.GetButton("Fire1") && timeSinceLastShot >= fireRate)
                {
                    Shoot();
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.R)) // Example key for reloading
        {
            Reload();
        }
    }
    private void Reload()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        InventoryItem ammoItem = inventory.GetItemByName(ammoType);

        // Check if there's reserve ammo and the magazine isn't full, and not already reloading
        if (ammoItem != null && ammoItem.itemAmount > 0 && currentAmmoInMagazine < magazineSize && !isReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
        else
        {
            Debug.Log("No reserve ammo to reload or magazine is already full.");
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        // Play reload sound
        if (gunReload != null)
        {
            gunReload.Play();
        }

        // Wait for the reload time to complete
        yield return new WaitForSeconds(reloadTime);

        // Reload logic
        Inventory inventory = FindObjectOfType<Inventory>();
        InventoryItem ammoItem = inventory.GetItemByName(ammoType);

        if (ammoItem != null && ammoItem.itemAmount > 0)
        {
            int ammoNeeded = magazineSize - currentAmmoInMagazine;
            int ammoToReload = Mathf.Min(ammoItem.itemAmount, ammoNeeded);

            currentAmmoInMagazine += ammoToReload;
            ammoItem.itemAmount -= ammoToReload;

            UpdateAmmoUI();
        }

        isReloading = false;
    }


    public void SetAmmoTextUI(Text ammoTextUI)
    {
        ammoText = ammoTextUI;
        UpdateAmmoUI(); // Update UI with current ammo status
    }
    public void UpdateAmmoUI()
    {
        int reserveAmmo = GetReserveAmmo();
        ammoText.text = $"{currentAmmoInMagazine} / {reserveAmmo}";
    }
    private void Shoot()
    {
        if (isReloading)
        {
            return; // Don't shoot if currently reloading
        }
        if (currentAmmoInMagazine > 0)
        {
            Debug.Log("Before shot: " + currentAmmoInMagazine);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            mousePosition.z = transform.position.z; // Ensure the Z position is consistent with the player's position

            Vector2 direction = (mousePosition - barrelEnd.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Minimum distance check
            float minDistanceToShoot = 1.0f; // Adjust this value as needed
            if ((mousePosition - transform.position).magnitude < minDistanceToShoot)
            {
                // Optional: Prevent shooting or adjust direction
                // For example, shoot forward based on player's facing direction
                // direction = transform.right; // Assuming the right direction is the player's forward
                return; // Or use the line above to set a default direction
            }

            // Offset the bullet spawn position slightly forward to avoid self-collision
            Vector3 spawnPosition = barrelEnd.position + (Vector3)(direction * 0.5f); // Adjust the 0.5f offset as needed
            gunShot.Play();
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.AngleAxis(angle - 90f, Vector3.forward));
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction * bulletSpeed;
            currentAmmoInMagazine--;
            Debug.Log("After shot: " + currentAmmoInMagazine);
            UpdateAmmoUI();

        }
        else
        {
            Debug.Log("Out of ammo in magazine!");
            // Trigger reload or other logic
        }
        timeSinceLastShot = 0f;
    }
    private int GetReserveAmmo()
    {
        InventoryItem ammoItem = FindObjectOfType<Inventory>().GetItemByName(ammoType);
        return ammoItem != null ? ammoItem.itemAmount : 0;
    }
    public void SetCurrentAmmo(int ammo)
    {
        currentAmmoInMagazine = ammo; 
        Debug.Log($"[set curr ammo] currentAmmoInMag: {currentAmmoInMagazine}");
        UpdateAmmoUI();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmoInMagazine;
    }
}
