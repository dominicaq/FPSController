using SteamPlayers;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListCreator : MonoBehaviour
{
    public GameObject containerPrefab;
    private PlayerInfo[] playerListArray;
    
    protected Callback<LobbyChatUpdate_t> clobbyUpdate;
    protected Callback<LobbyEnter_t> clobbyEnter;
    
    public void Start()
    {
        containerPrefab.SetActive(false);

        if (SteamManager.Initialized)
        {
            clobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
            clobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        }
    }

    private void OnLobbyEnter(LobbyEnter_t result)
    {
        InitList();
    }
    
    private void OnLobbyUpdate(LobbyChatUpdate_t result)
    {
        Debug.Log("Update");
        if (result.m_rgfChatMemberStateChange == 0x0001)
        {
            AddPlayer((CSteamID)result.m_ulSteamIDUserChanged);
        }
        else if (result.m_rgfChatMemberStateChange == 0x0002)
        {
            RemovePlayer((CSteamID)result.m_ulSteamIDUserChanged);
        }
    }

    public void InitList()
    {
        ClearList();
        
        playerListArray = SteamLobbyManager.currentPlayers;
        for (int i = 0; i < SteamLobbyManager.GetPlayerCount(); i++)
        {
            GenerateContainer(playerListArray[i].playerNickname, playerListArray[i].playerAvatar);
        }
    }

    public void AddPlayer(CSteamID steamId)
    {
        playerListArray = SteamLobbyManager.currentPlayers;
        PlayerInfo newPlayer = playerListArray[playerListArray.Length];
        
        GenerateContainer(newPlayer.playerNickname, newPlayer.playerAvatar);
    }
    
    public void RemovePlayer(CSteamID steamId)
    {
        for (int i = 0; i < SteamLobbyManager.GetPlayerCount(); i++)
        {
            if (playerListArray[i].playerId == steamId)
            {
                Destroy(transform.GetChild(i));
            }
        }

        playerListArray = SteamLobbyManager.currentPlayers;
    }
    public void ClearList()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateContainer(string steamName, Texture2D steamAvatar)
    {
        Transform newContainer = Instantiate(containerPrefab.transform, transform);
        newContainer.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(steamName);
        newContainer.GetChild(1).GetComponent<RawImage>().texture = steamAvatar;
        newContainer.gameObject.SetActive(true);
    }
}
