using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterWardrobe : MonoBehaviour
{
    public enum ClothingState
    {
        FullyClothed,
        Underwear,
        Nude
    }

    [System.Serializable]
    public class ClothingItem
    {
        public string itemName;
        public GameObject[] meshes; // Array in case an item consists of multiple parts
        public string category; // Top, Bottom, Bra, Panties, Dress, Shoes, etc.
    }

    [Header("Wardrobe Settings")]
    public ClothingState currentState = ClothingState.FullyClothed;
    public List<ClothingItem> wardrobeItems = new List<ClothingItem>();

    [Header("Base Body")]
    public GameObject[] baseBodyMeshes;

    private Dictionary<string, bool> activeItems = new Dictionary<string, bool>();

    void Start()
    {
        // Initialize state
        SetState(currentState);
    }

    public void SetState(ClothingState newState)
    {
        currentState = newState;
        UpdateWardrobeVisibility();
    }

    public void EquipItem(string itemName)
    {
        var item = wardrobeItems.FirstOrDefault(i => i.itemName.ToLower() == itemName.ToLower());
        if (item != null)
        {
            SetItemVisibility(item, true);
            activeItems[item.itemName] = true;
            Debug.Log($"Equipped: {itemName}");
        }
        else
        {
            Debug.LogWarning($"Clothing item '{itemName}' not found.");
        }
    }

    public void UnequipItem(string itemName)
    {
        var item = wardrobeItems.FirstOrDefault(i => i.itemName.ToLower() == itemName.ToLower());
        if (item != null)
        {
            SetItemVisibility(item, false);
            activeItems[item.itemName] = false;
            Debug.Log($"Unequipped: {itemName}");
        }
    }

    public void UnequipCategory(string category)
    {
        foreach (var item in wardrobeItems.Where(i => i.category.ToLower() == category.ToLower()))
        {
            UnequipItem(item.itemName);
        }
    }

    private void SetItemVisibility(ClothingItem item, bool isVisible)
    {
        foreach (var mesh in item.meshes)
        {
            if (mesh != null)
            {
                mesh.SetActive(isVisible);
            }
        }
    }

    public void UpdateWardrobeVisibility()
    {
        // First, ensure base body is always visible
        foreach(var body in baseBodyMeshes)
        {
            if(body != null) body.SetActive(true);
        }

        switch (currentState)
        {
            case ClothingState.FullyClothed:
                // Show everything, or a default outfit
                foreach (var item in wardrobeItems)
                {
                    // By default show tops, bottoms, shoes, bra, panties
                    if (item.category == "Top" || item.category == "Bottom" || item.category == "Shoes" || item.category == "Dress")
                        SetItemVisibility(item, true);
                    else if (item.category == "Bra" || item.category == "Panties")
                        SetItemVisibility(item, true); // Keep underwear active underneath
                    else
                        SetItemVisibility(item, false);
                }
                break;
            case ClothingState.Underwear:
                // Hide outer clothes, show underwear
                foreach (var item in wardrobeItems)
                {
                    if (item.category == "Bra" || item.category == "Panties")
                        SetItemVisibility(item, true);
                    else
                        SetItemVisibility(item, false);
                }
                break;
            case ClothingState.Nude:
                // Hide all clothing
                foreach (var item in wardrobeItems)
                {
                    SetItemVisibility(item, false);
                }
                break;
        }
    }

    public void ToggleItem(string itemName)
    {
        var item = wardrobeItems.FirstOrDefault(i => i.itemName.ToLower() == itemName.ToLower());
        if (item != null)
        {
            bool isCurrentlyActive = false;
            if (activeItems.ContainsKey(item.itemName))
                isCurrentlyActive = activeItems[item.itemName];
            else if (item.meshes.Length > 0 && item.meshes[0] != null)
                isCurrentlyActive = item.meshes[0].activeSelf;

            if (isCurrentlyActive)
                UnequipItem(itemName);
            else
                EquipItem(itemName);
        }
    }
}
