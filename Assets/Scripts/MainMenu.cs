using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Match
{
    public string ID;
    public int Map;
    public bool PublicMatch;
    public bool InMatch;
    public bool MatchFull;
    public List<GameObject> players = new List<GameObject>();

    public Match(string ID, GameObject player, bool publicMatch, int mapIndex)
    {
        Map = mapIndex;
        MatchFull = false;
        InMatch = false;
        this.ID = ID;
        PublicMatch = publicMatch;
        players.Add(player);
    }

    public Match()
    {

    }
}

public class MainMenu : NetworkBehaviour
{
    public static MainMenu instance;
    public readonly SyncList<Match> matches = new SyncList<Match>();
    public readonly SyncList<string> matchIDs = new SyncList<string>();
    public int MaxPlayers;
    private NetworkManager networkManager;

    [Header("MainMenu")]
    public InputField JoinInput;
    public Button[] Buttons;
    public Canvas LobbyCanvas;
    public Canvas SearchCanvas;
    private bool searching;

    [Header("Name")]
    public GameObject ChangeNamePanel;
    public GameObject CloseButton;
    public Button SetNameButton;
    public InputField NameInput;
    public int firstTime = 1;
    [SyncVar] public string DisplayName;

    [Header("Lobby")]
    public Transform UIPLayerParent;
    public GameObject UIPlayerPrefab;
    public Text IDText;
    public Button BeginGameButton;
    public GameObject localPlayerLobbyUI;
    public Image LobbyMapImage;
    public Text LobbyMapNameText;
    public bool inGame;

    [Header("Error")]
    public GameObject ErrorPanel;
    public Text ErrorText;

    [Header("Car")]
    public Button ChooseButton;
    public Transform PreviewParent;
    public Text NameText;
    public List<Car> Cars;
    public Text CoinsText;
    public int ChangeNameCost;
    public int Coins;
    private int index;
    private List<GameObject> previewCars = new List<GameObject>();

    [Header("Chat")]
    public Text ChatHistoryText;
    public InputField MessageInput;
    public Button SendButton;
    public Color PlayerColor;

    [Header("CustomLobby")]
    public Image MapImage;
    public Text MapNameText;
    public Toggle LobbyTypeToggle;
    public List<Map> Maps;
    private int mapIndex;

    private void Start()
    {
        instance = this;

        networkManager = FindObjectOfType<NetworkManager>();

        firstTime = PlayerPrefs.GetInt("firstTime", 1);
        Coins = PlayerPrefs.GetInt("Coins", Coins);

        if (PlayerPrefs.HasKey("index"))
        {
            index = PlayerPrefs.GetInt("index");
        }

        foreach (var car in Cars)
        {
            GameObject previewCar = Instantiate(car.PreviewObj, PreviewParent);
            previewCar.SetActive(false);
            previewCars.Add(previewCar);
        }

        if (index == PlayerPrefs.GetInt("index"))
        {
            ChooseButton.GetComponent<Image>().color = Color.white;
            ChooseButton.interactable = false;
            ChooseButton.GetComponentInChildren<Text>().text = "Chosen";
        }
        else
        {
            if (Cars[index].purchased == 0)
            {
                ChooseButton.interactable = true;
                ChooseButton.GetComponentInChildren<Text>().text = Cars[index].Cost + "C";

                if (Coins >= Cars[index].Cost)
                {
                    ChooseButton.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    ChooseButton.GetComponent<Image>().color = Color.red;
                }
            }
            else
            {
                ChooseButton.GetComponent<Image>().color = Color.white;
                ChooseButton.interactable = true;
                ChooseButton.GetComponentInChildren<Text>().text = "Choose";
            }
        }
        previewCars[index].SetActive(true);
        NameText.text = Cars[index].Name;
        Cars[0].purchased = 1;

        for (int i = 0; i < Cars.Count; i++)
        {
            if (PlayerPrefs.HasKey("purchased" + i))
            {
                Cars[i].purchased = PlayerPrefs.GetInt("purchased" + i);
            }
        }

        if (PlayerPrefs.HasKey("R"))
        {
            PlayerColor = new Color(PlayerPrefs.GetFloat("R"), PlayerPrefs.GetFloat("G"), PlayerPrefs.GetFloat("B"));
        }

        SetSendButtonActive(MessageInput.text);

        MapImage.sprite = Maps[mapIndex].Image;
        MapNameText.text = Maps[mapIndex].Name;

        if (!PlayerPrefs.HasKey("Name"))
        {
            return;
        }

        string defaultName = PlayerPrefs.GetString("Name");
        NameInput.text = defaultName;
        DisplayName = defaultName;
        SetName(defaultName);
    }

    private void Update()
    {
        if (!inGame)
        {
            Player[] players = FindObjectsOfType<Player>();

            for (int i = 0; i < players.Length; i++)
            {
                players[i].gameObject.transform.localScale = Vector3.zero;
            }

            if (firstTime == 1)
            {
                ChangeNamePanel.SetActive(true);
                CloseButton.SetActive(false);

                JoinInput.interactable = false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].interactable = false;
                }
            }
            else
            {
                SetNameButton.GetComponentInChildren<Text>().text = ChangeNameCost + "C";
                CloseButton.SetActive(true);

                if (Coins >= ChangeNameCost)
                {
                    SetNameButton.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    SetNameButton.GetComponent<Image>().color = Color.red;
                }
            }

            CoinsText.text = Coins + "C";
        }
    }

    public void SetName(string name)
    {
        if (name == DisplayName || string.IsNullOrEmpty(name))
        {
            SetNameButton.interactable = false;
        }
        else
        {
            SetNameButton.interactable = true;
        }
    }

    public void SaveName()
    {
        if (firstTime == 0)
        {
            if (Coins >= ChangeNameCost)
            {
                Coins -= ChangeNameCost;
                ChangeNamePanel.SetActive(false);
                JoinInput.interactable = false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].interactable = false;
                }
                firstTime = 0;
                DisplayName = NameInput.text;
                GetColor();
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.SetInt("firstTime", firstTime);
                PlayerPrefs.SetString("Name", DisplayName);
                Invoke(nameof(Disconnect), 1f);
            }
            else
            {
                JoinInput.interactable = false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].interactable = false;
                }
                ErrorPanel.SetActive(true);
                ErrorText.text = "������������ �����";
            }
        }
        else
        {
            ChangeNamePanel.SetActive(false);
            JoinInput.interactable = false;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].interactable = false;
            }
            firstTime = 0;
            DisplayName = NameInput.text;
            GetColor();
            PlayerPrefs.SetInt("firstTime", firstTime);
            PlayerPrefs.SetString("Name", DisplayName);
            Invoke(nameof(Disconnect), 1f);
        }
    }

    void GetColor()
    {
        PlayerColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        PlayerPrefs.SetFloat("R", PlayerColor.r);
        PlayerPrefs.SetFloat("G", PlayerColor.g);
        PlayerPrefs.SetFloat("B", PlayerColor.b);
    }

    public void Choose()
    {
        if (Cars[index].purchased == 0)
        {
            if (Coins >= Cars[index].Cost)
            {
                Coins -= Cars[index].Cost;
                Cars[index].purchased = 1;
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.SetInt("purchased" + index, Cars[index].purchased);
                PlayerPrefs.SetInt("index", index);
                JoinInput.interactable = false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].interactable = false;
                }
                Invoke(nameof(Disconnect), 1f);
            }
            else
            {
                JoinInput.interactable = false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].interactable = false;
                }
                ErrorPanel.SetActive(true);
                ErrorText.text = "������������ �����";
            }
        }
        else
        {
            PlayerPrefs.SetInt("index", index);
            JoinInput.interactable = false;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].interactable = false;
            }
            Invoke(nameof(Disconnect), 1f);
        }
    }

    public void ChangeIndex(bool previous)
    {
        previewCars[index].SetActive(false);

        if (!previous)
        {
            index = (index + 1) % previewCars.Count;
        }
        else
        {
            index--;
            if (index < 0)
            {
                index += previewCars.Count;
            }
        }

        if (index == PlayerPrefs.GetInt("index"))
        {
            ChooseButton.GetComponent<Image>().color = Color.white;
            ChooseButton.interactable = false;
            ChooseButton.GetComponentInChildren<Text>().text = "Chosen";
        }
        else
        {
            if (Cars[index].purchased == 0)
            {
                ChooseButton.interactable = true;
                ChooseButton.GetComponentInChildren<Text>().text = Cars[index].Cost + "C";

                if (Coins >= Cars[index].Cost)
                {
                    ChooseButton.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    ChooseButton.GetComponent<Image>().color = Color.red;
                }
            }
            else
            {
                ChooseButton.GetComponent<Image>().color = Color.white;
                ChooseButton.interactable = true;
                ChooseButton.GetComponentInChildren<Text>().text = "Choose";
            }
        }
        previewCars[index].SetActive(true);
        NameText.text = Cars[index].Name;
    }

    public void ChangeMapIndex(bool previous)
    {
        if (!previous)
        {
            mapIndex = (mapIndex + 1) % Maps.Count;
        }
        else
        {
            mapIndex--;
            if (mapIndex < 0)
            {
                mapIndex += Maps.Count;
            }
        }

        MapImage.sprite = Maps[mapIndex].Image;
        MapNameText.text = Maps[mapIndex].Name;
    }

    public void SetSendButtonActive(string message)
    {
        SendButton.interactable = !string.IsNullOrWhiteSpace(message);
    }

    public void HandleMessage()
    {
        if (!string.IsNullOrWhiteSpace(MessageInput.text))
        {
            Player.localPlayer.CmdHandleMessage(MessageInput.text);
        }
    }

    public void SendMessageToServer(string message)
    {
        ChatHistoryText.text += message + "\n";
        MessageInput.text = string.Empty;
    }

    public void Disconnect()
    {
        if (networkManager.mode == NetworkManagerMode.Host)
        {
            networkManager.StopHost();
        }
        else if (networkManager.mode == NetworkManagerMode.ClientOnly)
        {
            networkManager.StopClient();
        }
    }

    public void SetBeginButtonActive(bool active)
    {
        BeginGameButton.interactable = active;
    }

    public void Host()
    {
        JoinInput.interactable = false;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = false;
        }

        Player.localPlayer.HostGame(LobbyTypeToggle.isOn, mapIndex);
    }

    public void HostSuccess(bool success, string matchID, int mapIndex)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null)
            {
                Destroy(localPlayerLobbyUI);
            }

            localPlayerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            IDText.text = matchID;
            SetLobbyMap(mapIndex);
            BeginGameButton.interactable = true;
        }
        else
        {
            ErrorPanel.SetActive(true);
            ErrorText.text = "�� ������� ������� �����";
        }
    }

    public void SetLobbyMap(int mapIndex)
    {
        LobbyMapImage.sprite = Maps[mapIndex].Image;
        LobbyMapNameText.text = Maps[mapIndex].Name;
    }

    public void Join()
    {
        JoinInput.interactable = false;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = false;
        }

        Player.localPlayer.JoinGame(JoinInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null)
            {
                Destroy(localPlayerLobbyUI);
            }

            localPlayerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = false;
        }
        else
        {
            ErrorPanel.SetActive(true);
            ErrorText.text = "ID �� ������";
        }
    }

    public void Enable()
    {
        ErrorPanel.SetActive(false);
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = true;
        }
        JoinInput.interactable = true;
    }

    public void DisconnectGame()
    {
        if (localPlayerLobbyUI != null)
        {
            Destroy(localPlayerLobbyUI);
        }

        Player.localPlayer.DisconnectGame();
        LobbyCanvas.enabled = false;
        JoinInput.interactable = true;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = true;
        }
    }

    public bool HostGame(string matchID, GameObject player, bool publicMatch, int mapIndex)
    {
        if (!matchIDs.Contains(matchID))
        {
            matchIDs.Add(matchID);
            Match match = new Match(matchID, player, publicMatch, mapIndex);
            matches.Add(match);
            player.GetComponent<Player>().CurrentMatch = match;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool JoinGame(string matchID, GameObject player)
    {
        if (matchIDs.Contains(matchID))
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].ID == matchID)
                {
                    if (!matches[i].InMatch && !matches[i].MatchFull)
                    {
                        matches[i].players.Add(player);
                        player.GetComponent<Player>().CurrentMatch = matches[i];
                        matches[i].players[0].GetComponent<Player>().PlayerCountUpdated(matches[i].players.Count);
                        if (matches[i].players.Count == MaxPlayers)
                        {
                            matches[i].MatchFull = true;
                        }
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SearchGame(GameObject player, out string ID)
    {
        ID = "";

        for (int i = 0; i < matches.Count; i++)
        {
            Debug.Log("�������� ID " + matches[i].ID + " | � ���� " + matches[i].InMatch + " | ����� ������ " + matches[i].MatchFull + " | ��������� ����� " + matches[i].PublicMatch);
            if (!matches[i].InMatch && !matches[i].MatchFull && matches[i].PublicMatch)
            {
                if (JoinGame(matches[i].ID, player))
                {
                    ID = matches[i].ID;
                    return true;
                }
            }
        }

        return false;
    }

    public static string GetRandomID()
    {
        string ID = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int rand = UnityEngine.Random.Range(0, 36);
            if (rand < 26)
            {
                ID += (char)(rand + 65);
            }
            else
            {
                ID += (rand - 26).ToString();
            }
        }
        return ID;
    }

    public GameObject SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPLayerParent);
        newUIPlayer.GetComponent<PlayerUI>().SetPlayer(player.PlayerDisplayName);

        return newUIPlayer;
    }

    public void StartGame()
    {
        Player.localPlayer.BeginGame();
    }

    public void SearchGame()
    {
        StartCoroutine(Searching());
    }

    public void CancelSearchGame()
    {
        JoinInput.interactable = true;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = true;
        }

        searching = false;
    }

    public void SearchGameSuccess(bool success, string ID)
    {
        if (success)
        {
            SearchCanvas.enabled = false;
            searching = false;
            JoinSuccess(success, ID);
        }
    }

    public void BeginGame(string matchID)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].ID == matchID)
            {
                matches[i].InMatch = true;
                foreach (var player in matches[i].players)
                {
                    player.GetComponent<Player>().StartGame();
                }
                break;
            }
        }
    }

    public void PlayerDisconnected(GameObject player, string ID)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].ID == ID)
            {
                int playerIndex = matches[i].players.IndexOf(player);
                if (matches[i].players.Count > playerIndex)
                {
                    matches[i].players.RemoveAt(playerIndex);
                }

                if (matches[i].players.Count == 0)
                {
                    matches.RemoveAt(i);
                    matchIDs.Remove(ID);
                }
                else
                {
                    matches[i].players[0].GetComponent<Player>().PlayerCountUpdated(matches[i].players.Count);
                }
                break;
            }
        }
    }

    public void Exit()
    {
        Disconnect();
        Application.Quit();
    }

    IEnumerator Searching()
    {
        JoinInput.interactable = false;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].interactable = false;
        }
        SearchCanvas.enabled = true;
        searching = true;

        float searchInterval = 1;
        float currentTime = 1;

        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = searchInterval;
                Player.localPlayer.SearchGame();
            }
            yield return null;
        }
        SearchCanvas.enabled = false;
    }
}

public static class MatchExtension
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hasBytes = provider.ComputeHash(inputBytes);

        return new Guid(hasBytes);
    }
}

[System.Serializable]
public class Car
{
    public string Name;
    public GameObject PreviewObj;
    public Sprite[] Sprites;
    [Space(5)]
    public int Cost;

    [HideInInspector] public int purchased;
}

[System.Serializable]
public class Map
{
    public string Name;
    public Sprite Image;
    [Scene] public string MapScene;
}