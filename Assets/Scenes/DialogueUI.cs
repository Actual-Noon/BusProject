using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public static bool IsDialogueActive { get; private set; } = false;

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Choice Buttons")]
    [SerializeField] private GameObject choiceContainer; // Parent containing both buttons
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    private List<string> dialogueLines = new List<string>();
    private int currentLineIndex = 0;
    private bool isChoiceQuestion = false;
    private Action<bool> onChoiceMadeCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choiceContainer != null) choiceContainer.SetActive(false);

        if (acceptButton != null) acceptButton.onClick.AddListener(() => MakeChoice(true));
        if (declineButton != null) declineButton.onClick.AddListener(() => MakeChoice(false));
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // Advance dialogue on input ONLY if we are NOT on a choice line
        if (!isChoiceQuestion)
        {
            if ((Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))
                || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
            {
                NextLine();
            }
        }
    }

    // Standard Multi-line Dialogue
    public void StartDialogue(List<string> lines)
    {
        StartDialogueWithChoice(lines, false, null);
    }

    // Choice Dialogue (Question at the end)
    public void StartDialogueWithChoice(List<string> lines, bool hasChoiceAtEnd, Action<bool> callback)
    {
        if (lines == null || lines.Count == 0) return;

        dialogueLines = lines;
        currentLineIndex = 0;
        isChoiceQuestion = false;
        onChoiceMadeCallback = callback;
        IsDialogueActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (choiceContainer != null) choiceContainer.SetActive(false);

        DisplayCurrentLine(hasChoiceAtEnd);
    }

    private void DisplayCurrentLine(bool hasChoiceAtEnd)
    {
        if (dialogueText != null)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }

        // Check if we are on the final line and choices are expected
        bool isFinalLine = (currentLineIndex == dialogueLines.Count - 1);
        if (isFinalLine && hasChoiceAtEnd)
        {
            isChoiceQuestion = true;
            if (choiceContainer != null) choiceContainer.SetActive(true);
        }
    }

    public void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Count)
        {
            DisplayCurrentLine(onChoiceMadeCallback != null);
        }
        else
        {
            EndDialogue();
        }
    }

    private void MakeChoice(bool accepted)
    {
        if (choiceContainer != null) choiceContainer.SetActive(false);
        EndDialogue();

        // Send decision to the NPC script
        onChoiceMadeCallback?.Invoke(accepted);
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choiceContainer != null) choiceContainer.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}