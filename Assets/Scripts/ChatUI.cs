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
    private CharacterDialogue characterDialogue;

    void Start()
    {
        characterDialogue = FindObjectOfType<CharacterDialogue>();

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
            string userInput = commandInputField.text;
            AddMessageToHistory($"> {userInput}");

            // Clear input and focus immediately
            commandInputField.text = "";
            commandInputField.ActivateInputField();

            // First try to parse as an action command
            bool wasCommand = commandProcessor.ProcessCommand(userInput);

            if (wasCommand)
            {
                // Optionally add a system acknowledgment
                AddMessageToHistory("<i>[Действие выполнено]</i>");
            }
            else
            {
                // Not a command, pass to AI dialogue system
                if (characterDialogue != null)
                {
                    characterDialogue.SendMessageToAI(userInput);
                }
                else
                {
                    AddMessageToHistory("<i>[Команда не распознана, а AI не настроен]</i>");
                }
            }
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
