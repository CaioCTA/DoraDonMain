using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using PlayFab.ClientModels;
using TMPro;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayFabLogin : MonoBehaviour
{

    public static string PlayFabID;
    public string Nickname;
    public string EntityID;
    public string EntityType;

    public TMP_Text statusTextCreate;
    public TMP_Text statusTextLogin;

    public string usernameOrEmail;
    public string userPassword;
    public string username;

    // campos utilizados para efetuar o login do jogador
    public TMP_InputField inputUserEmailLogin;
    public TMP_InputField inputUserPasswordLogin;

    // campos utilizados para criar uma nova conta para o jogador
    public TMP_InputField inputUsername;
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;

    public GameObject loginPanel;

    static Loading loadManager;

    public static PlayFabLogin PFL;

    private void Awake()
    {
        if (PFL != null && PFL != this)
        {
            Destroy(PFL);
        }
        PFL = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #region Login

    public void Login()
    {
        if (string.IsNullOrEmpty(inputUserEmailLogin.text) || string.IsNullOrEmpty(inputUserPasswordLogin.text))
        {
            Debug.Log("Preencha os dados corretamente!");
            statusTextLogin.text = "Preencha os dados corretamente!";
        }
        else
        {
            // credenciais para autenticação
            usernameOrEmail = inputUserEmailLogin.text;
            userPassword = inputUserPasswordLogin.text;

            if (usernameOrEmail.Contains("@"))
            {
                //payload de requisição
                var requestEmail = new LoginWithEmailAddressRequest { Email = usernameOrEmail, Password = userPassword };

                // Requisição
                PlayFabClientAPI.LoginWithEmailAddress(requestEmail, SucessoLogin, FalhaLogin);
            }
            else
            {
                //payload de requisição
                var requestUsername = new LoginWithPlayFabRequest { Username = usernameOrEmail, Password = userPassword };

                // Requisição
                PlayFabClientAPI.LoginWithPlayFab(requestUsername, SucessoLogin, FalhaLogin);

            }
        }
    }

    public void CriarConta()
    {
        if (string.IsNullOrEmpty(inputUsername.text) || string.IsNullOrEmpty(inputEmail.text) || string.IsNullOrEmpty(inputPassword.text))
        {
            Debug.Log("Preencha os dados corretamente!");
            statusTextCreate.text = "Preencha os dados corretamente!";
            if (inputUsername.text.Contains("@") || inputUsername.text.Contains("!") || inputUsername.text.Contains("$"))
            {
                statusTextCreate.text = "Caracteres especiais não são permitidos no nome de usuário!";
            }
        }
        else
        {
            username = inputUsername.text;
            usernameOrEmail = inputEmail.text;
            userPassword = inputPassword.text;

            // payload da requisição
            var request = new RegisterPlayFabUserRequest { Email = usernameOrEmail, Password = userPassword, Username = username };
            // Requisição
            PlayFabClientAPI.RegisterPlayFabUser(request, SucessoCriarConta, FalhaCriarConta);
        }


    }

    //public void SucessoLogin(LoginResult resulto)
    //{
    //    Debug.Log("Login foi feito com sucesso!");
    //    statusTextLogin.text = "Login foi feito com sucesso!";
    //    loginPanel.SetActive(false);
    //    SceneManager.LoadScene("Loading");
    //}

    public void SucessoLogin(LoginResult result)
    {
        // captura o playfabID
        PlayFabID = result.PlayFabId;

        // Mensagens de status
        Debug.Log("Login foi feito com sucesso!");
        statusTextLogin.text = "Login foi feito com sucesso!";

        // desabilita o painel de login
        loginPanel.SetActive(false);

        // captura do nickname
        PegaDisplayName(PlayFabID);

        if (result.EntityToken != null && result.EntityToken.Entity != null)
        {
            EntityID = result.EntityToken.Entity.Id;
            EntityType = result.EntityToken.Entity.Type;

            Debug.Log($"EntityID: {EntityID}, EntityType: {EntityType}");
        }
        else
        {
            Debug.LogWarning("O LoginResult não retornou EntityToken. Talvez seja preciso chamar GetEntityToken separadamente.");
        }

        // carrega nova cena e conecta no photon
        //loadManager.Connect();
        PhotonNetwork.LoadLevel("Loading");
    }

    public void FalhaLogin(PlayFabError error)
    {
        Debug.Log("Não foi possível fazer login!");
        statusTextLogin.text = "Não foi possível fazer login!";

        switch (error.Error)
        {
            case PlayFabErrorCode.AccountNotFound:
                statusTextLogin.text = "Não foi possível efetuar o login!\nConta não existe.";
                break;
            case PlayFabErrorCode.InvalidEmailOrPassword:
                statusTextLogin.text = "Não foi possível efetuar o login!\nE-mail ou senha inválidos.";
                break;
            default:
                statusTextLogin.text = "Não foi possível efetuar o login!\nVerifique os dados infomados.";
                break;

        }
    }

    public void FalhaCriarConta(PlayFabError error)
    {
        Debug.Log("Falhou a tentativa de criar uma conta nova");
        statusTextCreate.text = "Falhou a tentativa de criar uma conta nova";

        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidEmailAddress:
                statusTextCreate.text = "Já possui um conta com esse email!";
                break;
            case PlayFabErrorCode.InvalidUsername:
                statusTextCreate.text = "Username já está em uso.";
                break;
            case PlayFabErrorCode.InvalidParams:
                statusTextCreate.text = "Não foi possível criar um conta! \nVerifique os dados informados";
                break;
            default:
                statusTextCreate.text = "Não foi possível efetuar o login!\nVerifique os dados infomados.";
                Debug.Log(error.ErrorMessage);
                break;
        }
    }

    public void SucessoCriarConta(RegisterPlayFabUserResult result)
    {
        Debug.Log("Sucesso ao criar uma conta nova!");
        statusTextCreate.text = "Sucesso ao criar uma conta nova!";

        inputEmail.text = "";
        inputPassword.text = "";
        inputUsername.text = "";
    }

    #endregion

    public void PegaDadosJogador(string id)
    {
        // requisição para pegar dados do jogador
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayFabID,
            Keys = null
        }, result => {

            if (result.Data == null || !result.Data.ContainsKey(id))
            {
                Debug.Log("Conteúdo vazio!");
            }

            else if (result.Data.ContainsKey(id))
            {
                PlayerPrefs.SetString(id, result.Data[id].Value);
            }

        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SalvaDadosJogador(string id, string valor)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {id, valor}
            }
        },
        result => Debug.Log("Dados do jogador atualizados com sucesso!"),
        error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void PegaDisplayName(string playFabId)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => {
            Nickname = result.PlayerProfile.DisplayName;
        },
        error => Debug.Log(error.ErrorMessage));
    }




    //Teste LeaderBoard

    public void UpdatePlayerScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "Score",
                Value = score
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatisticsUpdated, OnStatisticsUpdateFailed);
    }

    void OnStatisticsUpdated(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Pontuação atualizada com sucesso!");
    }

    void OnStatisticsUpdateFailed(PlayFabError error)
    {
        Debug.LogError("Erro ao atualizar pontuação: " + error.GenerateErrorReport());
    }


    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardReceived, OnLeaderboardError);
    }

    void OnLeaderboardReceived(GetLeaderboardResult result)
    {
        Debug.Log("Placar de líderes recebido:");
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"{entry.Position + 1}. {entry.DisplayName} - {entry.StatValue}");
        }
    }

    void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Erro ao obter placar de líderes: " + error.GenerateErrorReport());
    }


}
