using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Outfit", menuName = "Character/Outfit Definition")]
public class OutfitDefinition : ScriptableObject
{
    public string outfitName;
    [Tooltip("List of item names (from CharacterWardrobe) to enable for this outfit.")]
    public List<string> activeItemNames = new List<string>();
}

public class OutfitManager : MonoBehaviour
{
    [Header("Available Outfits")]
    public List<OutfitDefinition> availableOutfits = new List<OutfitDefinition>();

    private CharacterWardrobe wardrobe;

    void Start()
    {
        wardrobe = GetComponent<CharacterWardrobe>();
        if (wardrobe == null)
        {
            Debug.LogError("OutfitManager requires a CharacterWardrobe component on the same GameObject.");
        }

        // Auto-generate default outfits if none are assigned
        if (availableOutfits == null || availableOutfits.Count == 0)
        {
            CreateDefaultOutfits();
        }
    }

    private void CreateDefaultOutfits()
    {
        availableOutfits = new List<OutfitDefinition>();

        OutfitDefinition casual = ScriptableObject.CreateInstance<OutfitDefinition>();
        casual.outfitName = "Casual";
        casual.activeItemNames = new List<string> { "Top", "Jeans", "Bra", "Panties" };

        OutfitDefinition lingerie = ScriptableObject.CreateInstance<OutfitDefinition>();
        lingerie.outfitName = "Lingerie";
        lingerie.activeItemNames = new List<string> { "Bra", "Panties" };

        OutfitDefinition dress = ScriptableObject.CreateInstance<OutfitDefinition>();
        dress.outfitName = "Dress";
        dress.activeItemNames = new List<string> { "Dress", "Panties" };

        OutfitDefinition nude = ScriptableObject.CreateInstance<OutfitDefinition>();
        nude.outfitName = "Nude";
        nude.activeItemNames = new List<string>(); // Empty means fully nude

        availableOutfits.Add(casual);
        availableOutfits.Add(lingerie);
        availableOutfits.Add(dress);
        availableOutfits.Add(nude);

        Debug.Log("OutfitManager: Initialized 4 default outfits (Casual, Lingerie, Dress, Nude).");
    }

    public bool ApplyOutfit(string requestedOutfitName)
    {
        if (wardrobe == null) return false;

        string searchName = requestedOutfitName.ToLower().Trim();
        OutfitDefinition targetOutfit = availableOutfits.Find(o => o.outfitName.ToLower() == searchName);

        if (targetOutfit != null)
        {
            // First, strip character completely
            wardrobe.FullNude();

            // Set state to custom to prevent UpdateWardrobeVisibility from overwriting our specific setup
            wardrobe.SetCustomState();

            // Equip specified items
            foreach (string itemName in targetOutfit.activeItemNames)
            {
                wardrobe.EquipItem(itemName);
            }

            Debug.Log($"OutfitManager: Successfully applied outfit '{targetOutfit.outfitName}'");
            return true;
        }

        Debug.LogWarning($"OutfitManager: Could not find outfit '{requestedOutfitName}'");
        return false;
    }
}
