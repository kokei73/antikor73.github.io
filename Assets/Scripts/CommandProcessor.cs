using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandProcessor : MonoBehaviour
{
    private CharacterWardrobe wardrobe;
    private CharacterAnimationController animController;
    private OutfitManager outfitManager;

    void Start()
    {
        wardrobe = GetComponent<CharacterWardrobe>();
        animController = GetComponent<CharacterAnimationController>();
        outfitManager = GetComponent<OutfitManager>();

        if (wardrobe == null) Debug.LogError("CommandProcessor requires CharacterWardrobe on the same GameObject.");
        if (animController == null) Debug.LogError("CommandProcessor requires CharacterAnimationController on the same GameObject.");
        if (outfitManager == null) Debug.LogWarning("CommandProcessor: OutfitManager not found. Outfit switching commands will be ignored.");
    }

    public void ProcessCommand(string input)
    {
        // Normalize input: lowercase, trim, remove excessive spaces and basic punctuation
        string command = input.ToLower().Trim();
        command = command.Replace(".", "").Replace("!", "").Replace("?", "").Replace(",", "");
        // Collapse multiple spaces
        command = System.Text.RegularExpressions.Regex.Replace(command, @"\s+", " ");
        Debug.Log($"Processing command: {command}");

        // --- Outfit & Global State Commands ---

        if (command.Contains("надень casual") || command.Contains("оденься обычно") || command.Contains("оденься в casual") || command.Contains("outfit casual") || command.Contains("put on casual"))
        {
            if (wardrobe.ApplyOutfit("Casual")) return;
        }
        else if (command.Contains("надень lingerie") || command.Contains("смени outfit на lingerie") || command.Contains("надень бельё") || command.Contains("надень белье") || command.Contains("lingerie") || command.Contains("outfit lingerie") || command.Contains("put on lingerie"))
        {
            if (wardrobe.ApplyOutfit("Lingerie")) return;

            // Fallback if outfit doesn't exist
            wardrobe.SetUnderwearOnly();
            return;
        }
        else if (command.Contains("надень красное бельё") || command.Contains("надень красное белье") || command.Contains("красное белье") || command.Contains("outfit red lingerie"))
        {
            if (wardrobe.ApplyOutfit("Red Lingerie")) return;
        }
        else if (command.Contains("надень платье") || command.Contains("оденься в платье") || command.Contains("outfit dress") || command.Contains("put on dress"))
        {
            if (wardrobe.ApplyOutfit("Dress")) return;
        }
        else if (command.Contains("сними всё") || command.Contains("сними всю одежду") || command.Contains("разденься полностью") || command.Contains("полностью разденься") || command.Contains("полностью голая") || command.Contains("останься голой") || command.Contains("покажи тело") || command.Contains("nude") || command.Contains("голая") || command.Contains("take off everything") || command.Contains("get naked"))
        {
            if (wardrobe.ApplyOutfit("Nude")) return;

            // Fallback
            wardrobe.FullNude();
            return;
        }

        // Fully Clothed (Fallback)
        if (command.Contains("оденься") || command.Contains("надень одежду") || command.Contains("надень всё") || command.Contains("get dressed") || command.Contains("put on clothes"))
        {
            if (wardrobe.ApplyOutfit("Casual")) return;
            wardrobe.SetDefaultOutfit();
            return;
        }

        // Specific Item - Tops
        if (command.Contains("сними только топ") || command.Contains("сними топ") || command.Contains("take off top") || command.Contains("сними футболку") || command.Contains("take off shirt"))
        {
            wardrobe.UnequipCategory("Top");
        }
        else if (command.Contains("надень топ") || command.Contains("put on top") || command.Contains("надень футболку") || command.Contains("put on shirt"))
        {
            wardrobe.EquipItem("Top"); // Assuming "Top" is the itemName
        }

        // Specific Item - Bottoms
        if (command.Contains("сними только джинсы") || command.Contains("сними джинсы") || command.Contains("take off jeans") || command.Contains("сними штаны") || command.Contains("take off pants"))
        {
            wardrobe.UnequipCategory("Bottom");
        }
        else if (command.Contains("надень джинсы") || command.Contains("put on jeans") || command.Contains("надень штаны") || command.Contains("put on pants"))
        {
            wardrobe.EquipItem("Jeans"); // Assuming "Jeans" is the itemName
        }

        // Specific Item - Dress
        if (command.Contains("сними платье") || command.Contains("take off dress"))
        {
            wardrobe.UnequipCategory("Dress");
        }
        else if (command.Contains("надень красное платье") || command.Contains("put on red dress") || command.Contains("надень платье") || command.Contains("put on dress"))
        {
            wardrobe.UnequipCategory("Top");
            wardrobe.UnequipCategory("Bottom");
            wardrobe.EquipItem("Dress"); // Or "RedDress" based on setup
        }

        // Specific Item - Underwear
        if (command.Contains("сними только белье") || command.Contains("сними только бельё") || command.Contains("сними бельё") || command.Contains("сними белье"))
        {
            wardrobe.UnequipCategory("Bra");
            wardrobe.UnequipCategory("Panties");
        }
        else
        {
            if (command.Contains("сними лифчик") || command.Contains("take off bra"))
            {
                wardrobe.UnequipCategory("Bra");
            }
            else if (command.Contains("надень лифчик") || command.Contains("put on bra"))
            {
                wardrobe.EquipItem("Bra");
            }

            if (command.Contains("сними трусики") || command.Contains("сними трусы") || command.Contains("take off panties"))
            {
                wardrobe.UnequipCategory("Panties");
            }
            else if (command.Contains("надень трусики") || command.Contains("надень трусы") || command.Contains("put on panties"))
            {
                wardrobe.EquipItem("Panties");
            }
        }


        // --- Animation Commands ---
        if (animController != null)
        {
            if (command.Contains("потанцуй") || command.Contains("dance"))
            {
                animController.PlayAnimation("Dance");
            }
            else if (command.Contains("повернись спиной") || command.Contains("повернись назад"))
            {
                animController.PlayAnimation("TurnBack");
            }
            else if (command.Contains("повернись") || command.Contains("turn around") || command.Contains("spin"))
            {
                animController.PlayAnimation("Turn");
            }
            else if (command.Contains("наклонись вперёд") || command.Contains("наклонись вперед") || command.Contains("bend over"))
            {
                animController.PlayAnimation("BendOver");
            }
            else if (command.Contains("наклонись назад") || command.Contains("bend back"))
            {
                animController.PlayAnimation("BendBack");
            }
            else if (command.Contains("наклонись"))
            {
                animController.PlayAnimation("BendOver");
            }
            else if (command.Contains("ляг на спину") || command.Contains("ляг") || command.Contains("lie down") || command.Contains("lay on back"))
            {
                animController.PlayAnimation("LieDown");
            }
            else if (command.Contains("сядь на колени") || command.Contains("на колени") || command.Contains("kneel"))
            {
                animController.PlayAnimation("Kneel");
            }
            else if (command.Contains("сядь") || command.Contains("sit"))
            {
                animController.PlayAnimation("Sit");
            }
            else if (command.Contains("встань") || command.Contains("stand up"))
            {
                animController.PlayAnimation("Idle");
            }
            else if (command.Contains("помахай рукой") || command.Contains("помаши") || command.Contains("привет") || command.Contains("wave") || command.Contains("hello"))
            {
                animController.PlayAnimation("Wave");
            }
        }
    }
}
