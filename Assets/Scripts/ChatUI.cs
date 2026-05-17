using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("UI References")]
    public InputField commandInputField;
    public Button sendButton;

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
        if (sendButton != null && commandInputField != null)
        {
            sendButton.onClick.AddListener(OnSendClicked);
            commandInputField.onSubmit.AddListener(delegate { OnSendClicked(); });
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
            commandProcessor.ProcessCommand(commandInputField.text);
            commandInputField.text = ""; // Clear input after sending
            commandInputField.ActivateInputField(); // Keep focus
        }
    }
}
