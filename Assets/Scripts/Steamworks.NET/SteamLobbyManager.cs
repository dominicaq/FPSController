using SteamPlayers;
using Steamworks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    // By default, the game will be Spacewar

    #region Callbacks

    // Creation
    protected Callback<LobbyCreated_t> clobbyCreated;
    protected Callback<LobbyEnter_t> clobbyEntrance;
    protected Callback<LobbyChatUpdate_t> clobbyUpdate;
    protected Callback<GameLobbyJoinRequested_t> clobbyJoin;

    #endregion

    public static int maxLobbySize = 4;
    public static string currentLobbyName;
    public static CSteamID currentOwner;
    public static CSteamID currentLobby;
    public static bool isClientHost;
    public static bool isInitialized;
    public static bool isPrivate = true;
    
    public static PlayerInfo[] currentPlayers;
    
    public static bool updateMetaDataUi;
    
    public static bool Initialized {
        get {
            return isInitialized;
        }
    }
    
    public static int MaxSize {
        get { return maxLobbySize; }
        set { maxLobbySize = value; }
    }
    
    public static bool UpdateMetaData {
        get { return updateMetaDataUi; }
        set { updateMetaDataUi = value; }
    }
    
    void Start()
    {
        if(SteamManager.Initialized)
        {
            clobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            clobbyEntrance = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            clobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
            clobbyJoin = Callback<GameLobbyJoinRequested_t>.Create(OnJoinLobbyRequest);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Replace these with error prompts because steamchat wont always be active
            SteamChatBox.SendGameMessage("Something went wrong with steam, try restarting steam", Color.red);
        }
    }

    public void HostLobby(bool iIsPrivate)
    {
        if (iIsPrivate)
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxLobbySize);
        else
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxLobbySize);
    }

    public void SetLobbyPermType()
    {
        if (Initialized)
        {
            if (!isPrivate)
            {
                SteamMatchmaking.SetLobbyType(currentLobby, ELobbyType.k_ELobbyTypePrivate);
                isPrivate = true;
            }
            else
            {
                SteamMatchmaking.SetLobbyType(currentLobby, ELobbyType.k_ELobbyTypePublic);
                isPrivate = false;
            }

            updateMetaDataUi = true;
        }
    }

    public void QuitLobby()
    {
        if (isInitialized)
        {
            SteamMatchmaking.LeaveLobby(currentLobby);

            for (int i = 0; i < currentPlayers.Length; i++)
            {
                SteamNetworking.CloseP2PSessionWithUser(currentPlayers[i].playerId);
            }

            isClientHost = false;
            currentOwner = CSteamID.Nil;
            currentLobby = CSteamID.Nil;
            isInitialized = false;
            updateMetaDataUi = true;
            
            // UI
            SteamChatBox.ChatClear();
        }
    }
    
    private void OnLobbyCreated(LobbyCreated_t lobby)
    {
        if (lobby.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby. Error: " + lobby.m_eResult);
        }
        else
        {
            isInitialized = true;
        }
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        FetchLobbyData((CSteamID)result.m_ulSteamIDLobby);
        
        if (currentOwner != SteamUser.GetSteamID())
        {
            SteamChatBox.SendGameMessage("You joined " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyOwner((CSteamID) result.m_ulSteamIDLobby)) + "'s lobby", Color.yellow);
        }
        else
        {
            SteamChatBox.SendGameMessage("Lobby successfully created", Color.yellow);
            SetLobbyName(SteamFriends.GetPersonaName() + "'s lobby");
        }

        currentLobbyName = SteamMatchmaking.GetLobbyData(currentLobby, "lobbyName");
        isInitialized = true;
        updateMetaDataUi = true;
    }

    private void FetchLobbyData(CSteamID lobby)
    {
        currentLobby = lobby;
        currentOwner = SteamMatchmaking.GetLobbyOwner(lobby);
        isClientHost = currentOwner == SteamUser.GetSteamID();
        currentPlayers = GetPlayers();
    }
    
    private void OnLobbyUpdate(LobbyChatUpdate_t result)
    {
        FetchLobbyData((CSteamID)result.m_ulSteamIDLobby);
        currentLobbyName = SteamMatchmaking.GetLobbyData(currentLobby, "lobbyName");
        updateMetaDataUi = true;
    }

    public void SetLobbyName(string newName)
    {
        SteamMatchmaking.SetLobbyData(currentLobby, "lobbyName", newName);
    }

    // Inviting users
    public void InviteUser()
    {
        if (currentLobby == CSteamID.Nil)
        {
            HostLobby(true);
        }
        
        if(Initialized)
            SteamFriends.ActivateGameOverlayInviteDialog(currentLobby);
    }
    
    private void OnJoinLobbyRequest(GameLobbyJoinRequested_t result)
    {
        if (result.m_steamIDLobby != CSteamID.Nil)
        {
            if (currentLobby != CSteamID.Nil)
            {
                QuitLobby();
            }
            
            SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
        }
    }
    
    // Player functions
    public static int GetPlayerCount()
    {
        return SteamMatchmaking.GetNumLobbyMembers(currentLobby);
    }
        
    public static PlayerInfo[] GetPlayers()
    {
        int userCount = GetPlayerCount();
        PlayerInfo[] currentPlayerList = new PlayerInfo[userCount];
        for (int i = 0; i < userCount; i++)
        {
            CSteamID indexId = SteamMatchmaking.GetLobbyMemberByIndex(currentLobby, i);

            currentPlayerList[i].playerId = indexId;
            currentPlayerList[i].playerNickname = SteamFriends.GetFriendPersonaName(indexId);
            currentPlayerList[i].playerAvatar = GetSteamImageAsTexture2D(SteamFriends.GetMediumFriendAvatar(indexId));
        }

        return currentPlayerList;
    }

    public static Texture2D GetSteamImageAsTexture2D(int iImage)
    {
        Texture2D ret = null;
        uint imageWidth;
        uint imageHeight;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out imageWidth, out imageHeight);

        if (bIsValid)
        {
            byte[] image = new byte[imageWidth * imageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, image, (int) (imageWidth * imageHeight * 4));
            if (bIsValid)
            {
                ret = new Texture2D((int) imageWidth, (int) imageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(image);
                ret.Apply();
            }
        }

        return ret;
    }
}
