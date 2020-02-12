using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteamChatBox : MonoBehaviour
{
    public int maxEntries = 6;
    private static int currentChatSize;

    
    public TMP_InputField typeField;
    public Transform chatEntry;
    public Transform chatContainer;
    public CanvasGroup canvas;
    
    private static Transform s_chatEntry;
    private static Transform s_ChatContainer;
    private static CanvasGroup s_Canvas;
    
    #region Callbacks

    protected Callback<LobbyChatMsg_t> cChatMessageUpdate;

    #endregion
    
    void Start()
    {
        if (chatContainer && chatEntry)
        {
            s_ChatContainer = chatContainer;
            s_chatEntry = chatEntry;
            s_Canvas = canvas;
        }
        
        chatEntry.gameObject.SetActive(false);
        if (SteamManager.Initialized)
        {
            cChatMessageUpdate = Callback<LobbyChatMsg_t>.Create(OnChatUpdate);
        }
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            typeField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            OnSteamUserSendMessage(typeField.text);
        }
        
        if (Input.GetKeyDown(KeyCode.Y) && !typeField.isFocused)
        {
            typeField.ActivateInputField();
            EventSystem.current.SetSelectedGameObject(typeField.gameObject);
        }

        if (currentChatSize >= maxEntries)
        {
            Destroy(chatContainer.GetChild(0).gameObject);
            currentChatSize--;
        }
    }

    private void OnChatUpdate(LobbyChatMsg_t message)
    { 
        byte[] messageBuffer = new byte[1024];
        SteamMatchmaking.GetLobbyChatEntry(SteamLobbyManager.currentLobby, (int)message.m_iChatID, out CSteamID user, messageBuffer,messageBuffer.Length, out EChatEntryType type);
        string recievedMessage = System.Text.Encoding.UTF8.GetString(messageBuffer);

        Transform newChatEntry = Instantiate(chatEntry.transform, chatContainer);
        newChatEntry.GetComponent<TextMeshProUGUI>().SetText(SteamFriends.GetFriendPersonaName((CSteamID)message.m_ulSteamIDUser) + ": " + recievedMessage);
        newChatEntry.gameObject.SetActive(true);
        currentChatSize++;
    }
    
    public void OnSteamUserSendMessage(string textInput)
    {
        if (SteamManager.Initialized)
        {
            byte[] message = System.Text.Encoding.UTF8.GetBytes(textInput);
            SteamMatchmaking.SendLobbyChatMsg(SteamLobbyManager.currentLobby, message, message.Length);
        }
        
        typeField.text = "";
    }

    public static void SendGameMessage(string prompt, Color color)
    {
        Transform newChatEntry = Instantiate(s_chatEntry.transform, s_ChatContainer);
        TextMeshProUGUI text = newChatEntry.GetComponent<TextMeshProUGUI>();
        text.color = color;
        text.SetText(prompt);
        newChatEntry.gameObject.SetActive(true);
        currentChatSize++;
    }
    
    public static void ChatClear()
    {
        foreach (Transform child in s_ChatContainer) 
        {
            Destroy(child.gameObject);
        }
        
    }

    public static void HideChat()
    {
        s_Canvas.alpha = 0;
        s_Canvas.interactable = false;
        s_Canvas.blocksRaycasts = false;
    }

    public static void ShowChat()
    {
        s_Canvas.alpha = 1;
        s_Canvas.interactable = true;
        s_Canvas.blocksRaycasts = true;
    }
}
