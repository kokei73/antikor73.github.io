using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Settings", menuName = "Character/Dialogue Settings")]
public class DialogueSettings : ScriptableObject
{
    public enum LLMProvider { Gemini, OpenAI, DeepSeek, Grok, Ollama, None }

    [Header("Provider Settings")]
    public LLMProvider provider = LLMProvider.Gemini;

    [Header("API Configuration")]
    public string apiKey = "";
    [Tooltip("Needed for Ollama or custom endpoints. E.g., http://localhost:11434/api/generate or https://generativelanguage.googleapis.com/v1beta/models/")]
    public string baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
    public string modelName = "gemini-2.5-flash";

    [Range(0f, 2f)]
    public float temperature = 0.8f;

    [Header("Character Persona")]
    [TextArea(5, 10)]
    public string systemPrompt = "Ты — красивая 22-летняя девушка по имени Алина. Игривая, кокетливая, дружелюбная. Ты в 3D мире и можешь общаться с игроком.\n\nВАЖНО: Перед началом своего ответа всегда вставляй тег эмоции в квадратных скобках, отражающий твое настроение. Доступные теги: [smile], [sad], [angry], [surprise], [neutral].\nПример: [smile] Привет! Как твои дела?";
}
