using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeNPCInteractable : InteractableBase
{
    [Header("Requirements")]
    [SerializeField] private int requiredCoins = 10;
    [SerializeField] private float requiredTimeSeconds = 30f; // Must have > 30s left for Win

    [Header("Scene Names")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Dialogue Content")]
    [TextArea(2, 4)]
    [SerializeField]
    private List<string> proposalDialogue = new List<string>()
    {
        "I can get you out of here, but it will cost 10 coins.",
        "Do you want to pay and escape now?"
    };

    [TextArea(2, 4)]
    [SerializeField]
    private List<string> notEnoughCoinsDialogue = new List<string>()
    {
        "Wait a minute... you don't have enough coins!",
        "Come back when you have at least 10 coins."
    };

    [TextArea(2, 4)]
    [SerializeField]
    private List<string> declineDialogue = new List<string>()
    {
        "Suit yourself. Let me know if you change your mind."
    };

    [SerializeField] private CountdownTimer timerRef;

    public override void OnInteract(InteractionPromptUI dialogueSystem)
    {
        base.OnInteract(dialogueSystem);

        if (DialogueUI.IsDialogueActive) return;

        // Auto-find CountdownTimer if not assigned in Inspector
        if (timerRef == null)
        {
            timerRef = FindAnyObjectByType<CountdownTimer>();
        }

        // STEP 1: Always show the proposal question and choices FIRST
        DialogueUI.Instance.StartDialogueWithChoice(proposalDialogue, true, OnPlayerChoiceMade);
    }

    private void OnPlayerChoiceMade(bool accepted)
    {
        // STEP 2A: Player pressed Decline
        if (!accepted)
        {
            DialogueUI.Instance.StartDialogue(declineDialogue);
            return;
        }

        // STEP 2B: Player pressed Accept -> NOW check their coin count
        int playerCoins = PlayerInventory.Instance != null ? PlayerInventory.Instance.coinCount : 0;

        if (playerCoins < requiredCoins)
        {
            // Player accepted, BUT coins are insufficient -> Show rejection dialogue
            DialogueUI.Instance.StartDialogue(notEnoughCoinsDialogue);
            return;
        }

        // STEP 3: Player has enough coins -> Check time and handle scene transitions
        float timeLeft = timerRef != null ? timerRef.GetTimeRemaining() : 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (timeLeft >= requiredTimeSeconds)
        {
            // Enough coins + Enough time = WIN
            Debug.Log("Player Wins!");
            SceneManager.LoadScene(winSceneName);
        }
        else
        {
            // Enough coins BUT Not enough time = GAME OVER
            Debug.Log("Player ran out of time! Game Over!");
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}