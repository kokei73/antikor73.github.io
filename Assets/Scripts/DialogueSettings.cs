using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Settings", menuName = "Character/Dialogue Settings")]
public class DialogueSettings : ScriptableObject
{
    [Header("API Configuration")]
    public string apiKey = "YOUR_API_KEY_HERE";
    public string apiUrl = "https://api.openai.com/v1/chat/completions";
    public string model = "gpt-4o";

    [Header("Character Persona")]
    [TextArea(5, 10)]
    public string systemPrompt = "Ты — красивая 22-летняя девушка по имени Алина. Игривая, немного дерзкая, дружелюбная. Отвечай естественно, с эмоциями. Ты находишься в 3D-пространстве и можешь выполнять действия по команде.";
}
