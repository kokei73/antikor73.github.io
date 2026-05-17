using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterDialogue : MonoBehaviour
{
    public DialogueSettings settings;
    private ChatUI chatUI;

    [System.Serializable]
    private class OpenAIRequest
    {
        public string model;
        public Message[] messages;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    void Start()
    {
        chatUI = FindObjectOfType<ChatUI>();
        if (settings == null)
        {
            Debug.LogWarning("CharacterDialogue: No DialogueSettings assigned. AI Chat will not work.");
        }
    }

    public void SendMessageToAI(string userText)
    {
        if (settings == null || string.IsNullOrEmpty(settings.apiKey) || settings.apiKey == "YOUR_API_KEY_HERE")
        {
            if (chatUI != null) chatUI.AddMessageToHistory("Алина: Извини, но мой API ключ не настроен.");
            return;
        }

        StartCoroutine(SendRequestRoutine(userText));
    }

    private IEnumerator SendRequestRoutine(string userText)
    {
        OpenAIRequest requestData = new OpenAIRequest
        {
            model = settings.model,
            messages = new Message[]
            {
                new Message { role = "system", content = settings.systemPrompt },
                new Message { role = "user", content = userText }
            }
        };

        string jsonPayload = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(settings.apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                    if (response != null && response.choices != null && response.choices.Length > 0)
                    {
                        string aiReply = response.choices[0].message.content;
                        if (chatUI != null)
                        {
                            chatUI.AddMessageToHistory($"Алина: {aiReply}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing AI response: {e.Message}");
                    if (chatUI != null) chatUI.AddMessageToHistory("Алина: Что-то пошло не так с моим ответом.");
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
