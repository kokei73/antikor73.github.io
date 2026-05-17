using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("UI References")]
    public InputField commandInputField;
    public Button sendButton;
    public Text chatHistoryText; // The Text component that shows history
    public ScrollRect chatScrollRect; // The ScrollRect containing the text

    [Header("Chat Settings")]
    public int maxHistoryLines = 50;
    private List<string> historyLines = new List<string>();

    [Header("Quick Test Buttons")]
    public Button undressAllButton;
    public Button dressAllButton;
    public Button danceButton;

    [Header("Character Reference")]
    public CommandProcessor commandProcessor;

    void Start()
    {
        if (commandProcessor == null)
        {
            Debug.LogError("ChatUI needs a reference to the CommandProcessor.");
            return;
        }

        // Setup Chat Input
        if (commandInputField != null)
        {
            // Set Placeholder text programmatically if it exists
            Text placeholder = commandInputField.placeholder as Text;
            if (placeholder != null)
            {
                placeholder.text = "Напиши команду...";
            }
            commandInputField.onSubmit.AddListener(delegate { OnSendClicked(); });
        }

        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendClicked);
        }

        // Setup Quick Buttons
        if (undressAllButton != null)
            undressAllButton.onClick.AddListener(() => commandProcessor.ProcessCommand("сними всё"));

        if (dressAllButton != null)
            dressAllButton.onClick.AddListener(() => commandProcessor.ProcessCommand("оденься"));

        if (danceButton != null)
            danceButton.onClick.AddListener(() => commandProcessor.ProcessCommand("потанцуй"));
    }

    void OnSendClicked()
    {
        if (!string.IsNullOrEmpty(commandInputField.text))
        {
            string cmd = commandInputField.text;
            AddMessageToHistory($"> {cmd}");
            commandProcessor.ProcessCommand(cmd);
            commandInputField.text = ""; // Clear input after sending
            commandInputField.ActivateInputField(); // Keep focus
        }
    }

    public void AddMessageToHistory(string message)
    {
        if (chatHistoryText == null) return;

        historyLines.Add(message);

        // Limit history lines
        if (historyLines.Count > maxHistoryLines)
        {
            historyLines.RemoveAt(0);
        }

        chatHistoryText.text = string.Join("\n", historyLines);

        // Force scroll to bottom
        if (chatScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
