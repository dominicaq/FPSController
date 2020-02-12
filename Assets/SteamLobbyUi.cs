using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobbyUi : MonoBehaviour
{
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyIdDisplay;
    public PlayerListCreator list;
    public Button lobbyType;
    public Text lobbyTypeText;
    public WindowManager windows;

    protected Callback<LobbyChatUpdate_t> clobbyUpdate;
    
    private void Start()
    {
        //clobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamLobbyManager.UpdateMetaData)
        {
            if(SteamLobbyManager.Initialized)
            {
                windows.SwitchToWindow("lobbyCanvas");
                SteamChatBox.ShowChat();
                
                lobbyName.SetText(SteamLobbyManager.currentLobbyName);
                lobbyIdDisplay.SetText("Lobby id: " +SteamLobbyManager.currentLobby);

                if (SteamLobbyManager.isClientHost)
                {
                    lobbyType.gameObject.SetActive(true);
                
                    if (SteamLobbyManager.isPrivate)
                        lobbyTypeText.text = "Perms: Private";
                    else
                        lobbyTypeText.text = "Perms: Public";
                }
                else
                {
                    lobbyType.gameObject.SetActive(false);
                }

            }
            else
            {
                list.ClearList();
                lobbyName.SetText("");
                lobbyIdDisplay.SetText("");
                windows.SwitchToWindow("lobbyListCanvas");
                SteamChatBox.HideChat();
            }

            SteamLobbyManager.UpdateMetaData = false;
        }
    }

    private void OnLobbyUpdate(LobbyChatUpdate_t result)
    {

    }
}
