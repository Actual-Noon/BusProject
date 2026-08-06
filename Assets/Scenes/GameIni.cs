using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    // CHANGED: Now uses your updated InteractionPromptUI system
    [SerializeField] private InteractionPromptUI dialogueBubble;

    void Start()
    {
        if (dialogueBubble != null)
        {
            // Clean & simple: Just pass the text and the duration. 
            // It will stay exactly where you positioned it in your Unity Scene layout!
            dialogueBubble.Show("เห้ยตกรถตามล้อรถบัส!", 6f);
        }
    }
}