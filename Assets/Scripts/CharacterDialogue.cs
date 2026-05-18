using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using System.Text.RegularExpressions;

public class CharacterDialogue : MonoBehaviour
{
    public DialogueSettings settings;
    private ChatUI chatUI;
    private CharacterExpressions expressions;

    // --- JSON Classes for OpenAI, DeepSeek, Grok ---
    [System.Serializable]
    private class OpenAIRequest { public string model; public Message[] messages; public float temperature; }
    [System.Serializable]
    private class Message { public string role; public string content; }
    [System.Serializable]
    private class OpenAIResponse { public Choice[] choices; }
    [System.Serializable]
    private class Choice { public Message message; }

    // --- JSON Classes for Gemini ---
    [System.Serializable]
    private class GeminiRequest { public GeminiContent[] contents; public GeminiSystemInstruction systemInstruction; public GeminiGenerationConfig generationConfig; }
    [System.Serializable]
    private class GeminiContent { public string role; public GeminiPart[] parts; }
    [System.Serializable]
    private class GeminiPart { public string text; }
    [System.Serializable]
    private class GeminiSystemInstruction { public GeminiPart[] parts; }
    [System.Serializable]
    private class GeminiGenerationConfig { public float temperature; }
    [System.Serializable]
    private class GeminiResponse { public GeminiCandidate[] candidates; }
    [System.Serializable]
    private class GeminiCandidate { public GeminiContent content; }

    // --- JSON Classes for Ollama ---
    [System.Serializable]
    private class OllamaRequest { public string model; public string prompt; public string system; public bool stream; public OllamaOptions options; }
    [System.Serializable]
    private class OllamaOptions { public float temperature; }
    [System.Serializable]
    private class OllamaResponse { public string response; }

    void Start()
    {
        chatUI = FindObjectOfType<ChatUI>();
        expressions = GetComponent<CharacterExpressions>();

        if (settings == null)
        {
            Debug.LogWarning("CharacterDialogue: No DialogueSettings assigned. AI Chat will not work.");
        }
    }

    public void SendMessageToAI(string userText)
    {
        if (settings == null) return;

        if (settings.provider == DialogueSettings.LLMProvider.None)
        {
            if (chatUI != null) chatUI.AddMessageToHistory("<i>[AI диалог отключён в настройках]</i>");
            return;
        }

        if (string.IsNullOrEmpty(settings.apiKey) && settings.provider != DialogueSettings.LLMProvider.Ollama)
        {
            if (chatUI != null) chatUI.AddMessageToHistory($"Алина: API ключ для {settings.provider} не настроен. Диалог отключён.");
            return;
        }

        StartCoroutine(SendRequestRoutine(userText));
    }

    private IEnumerator SendRequestRoutine(string userText)
    {
        string jsonPayload = "";
        string url = "";
        bool useBearer = true;

        switch (settings.provider)
        {
            case DialogueSettings.LLMProvider.OpenAI:
            case DialogueSettings.LLMProvider.DeepSeek:
            case DialogueSettings.LLMProvider.Grok:
                url = settings.baseUrl;
                // Common API URL fallbacks if user left baseUrl as default Gemini one
                if (url.Contains("generativelanguage"))
                {
                    if (settings.provider == DialogueSettings.LLMProvider.OpenAI) url = "https://api.openai.com/v1/chat/completions";
                    if (settings.provider == DialogueSettings.LLMProvider.DeepSeek) url = "https://api.deepseek.com/chat/completions";
                    if (settings.provider == DialogueSettings.LLMProvider.Grok) url = "https://api.x.ai/v1/chat/completions";
                }

                OpenAIRequest openAiReq = new OpenAIRequest
                {
                    model = settings.modelName,
                    temperature = settings.temperature,
                    messages = new Message[]
                    {
                        new Message { role = "system", content = settings.systemPrompt },
                        new Message { role = "user", content = userText }
                    }
                };
                jsonPayload = JsonUtility.ToJson(openAiReq);
                break;

            case DialogueSettings.LLMProvider.Gemini:
                // Expected baseUrl: https://generativelanguage.googleapis.com/v1beta/models/
                // e.g. https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=...
                string baseGemini = settings.baseUrl.EndsWith("/") ? settings.baseUrl : settings.baseUrl + "/";
                url = $"{baseGemini}{settings.modelName}:generateContent?key={settings.apiKey}";
                useBearer = false;

                GeminiRequest geminiReq = new GeminiRequest
                {
                    systemInstruction = new GeminiSystemInstruction { parts = new GeminiPart[] { new GeminiPart { text = settings.systemPrompt } } },
                    contents = new GeminiContent[] { new GeminiContent { role = "user", parts = new GeminiPart[] { new GeminiPart { text = userText } } } },
                    generationConfig = new GeminiGenerationConfig { temperature = settings.temperature }
                };
                jsonPayload = JsonUtility.ToJson(geminiReq);
                break;

            case DialogueSettings.LLMProvider.Ollama:
                url = settings.baseUrl; // e.g. http://localhost:11434/api/generate
                if (!url.EndsWith("/api/generate") && !url.EndsWith("/api/chat")) url = "http://localhost:11434/api/generate";
                useBearer = false;

                OllamaRequest ollamaReq = new OllamaRequest
                {
                    model = settings.modelName,
                    prompt = userText,
                    system = settings.systemPrompt,
                    stream = false,
                    options = new OllamaOptions { temperature = settings.temperature }
                };
                jsonPayload = JsonUtility.ToJson(ollamaReq);
                break;
        }

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (useBearer)
            {
                request.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string aiReply = "Что-то пошло не так...";
                try
                {
                    string resText = request.downloadHandler.text;
                    if (settings.provider == DialogueSettings.LLMProvider.Gemini)
                    {
                        GeminiResponse gRes = JsonUtility.FromJson<GeminiResponse>(resText);
                        aiReply = gRes.candidates[0].content.parts[0].text;
                    }
                    else if (settings.provider == DialogueSettings.LLMProvider.Ollama)
                    {
                        OllamaResponse oRes = JsonUtility.FromJson<OllamaResponse>(resText);
                        aiReply = oRes.response;
                    }
                    else // OpenAI, DeepSeek, Grok
                    {
                        OpenAIResponse oRes = JsonUtility.FromJson<OpenAIResponse>(resText);
                        aiReply = oRes.choices[0].message.content;
                    }

                    // Extract emotion tag if present
                    string cleanReply = aiReply;
                    Match match = Regex.Match(aiReply, @"\[(.*?)\]");
                    if (match.Success)
                    {
                        string emotion = match.Groups[1].Value;
                        if (expressions != null)
                        {
                            expressions.SetEmotion(emotion);
                        }
                        // Remove the tag from the text shown to user
                        cleanReply = aiReply.Replace(match.Value, "").Trim();
                    }

                    if (chatUI != null) chatUI.AddMessageToHistory($"Алина: {cleanReply}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing AI response: {e.Message}");
                    if (chatUI != null) chatUI.AddMessageToHistory("Алина: Извини, я не поняла ответ сервера.");
                }
            }
            else
            {
                Debug.LogError($"AI Request Error: {request.error}\n{request.downloadHandler.text}");
                if (chatUI != null) chatUI.AddMessageToHistory("Алина: У меня проблема со связью.");
            }
        }
    }
}
