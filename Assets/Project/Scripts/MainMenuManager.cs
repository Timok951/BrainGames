using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Connect.Core
{
    public class MainMenuManager : MonoBehaviour
    {
        #region START_VARIABLES
        public static MainMenuManager Instance;

        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _levelPanelConnect;
        [SerializeField] private GameObject _levelPanelColorsort;
        [SerializeField] private GameObject _levelPanelPipes;
        [SerializeField] private GameObject _AuthorithationPannel;
        [SerializeField] private GameObject _registerPannel;
        [SerializeField] private GameObject _loginPannel;
        [SerializeField] private GameObject _profilePannel;
        [SerializeField] private GameObject _passwordchangepannel;
        [SerializeField] private GameObject _passwordchangepannelunnokwn;
        private Tween playStartTween;
        private Tween playNextTween;

        [SerializeField] private GameObject _syncPanel;
        [SerializeField] private TMP_Text _syncMessage;
        [SerializeField] private Button _keepLocalButton;
        [SerializeField] private Button _updateFromDbButton;


        public TMP_InputField nickfieldlogin;
        public TMP_InputField passwordfieldlogin;

        public TMP_InputField nickfield;
        public TMP_InputField passwordfield;
        public TMP_InputField emailfield;

        public Button registerbutton;
        public Button LoginButton;

        [SerializeField] private GameObject _leaderboardPannel;
        [SerializeField] private GameObject _choosegamescreen;

        [SerializeField] private Transform _leaderboardContent; 
        [SerializeField] private GameObject _leaderboardRowPrefab;


        [SerializeField] private TMP_InputField _emailResetField;
        [SerializeField] private Button _sendCodeButton;
        [SerializeField] private TMP_InputField _resetCodeField;
        [SerializeField] private TMP_InputField _newPasswordField;
        [SerializeField] private Button _confirmResetButton;
        [SerializeField] private TMP_Text _resetMessageText;

        [SerializeField] private TMP_Text _loginerrortext;


        [SerializeField] public TMP_Text ErrorRegistration;

        #endregion

        #region START_REGION

        //Awakening
        private void Awake()
        {
            Instance = this;
            _titlePanel.SetActive(true);
            _levelPanelConnect.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _choosegamescreen.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _passwordchangepannel.SetActive(false);
            _passwordchangepannelunnokwn.SetActive(false);
            _AuthorithationPannel.SetActive(false);
        }
        #endregion

        #region UPDATE_REGION
        #region MENU_REGION
        public UnityAction LevelOpened;

        [SerializeField] public Color CurrentColor;
        [SerializeField] private TMP_Text _levelTitleText;
        [SerializeField] private UnityEngine.UI.Image _levelTitleImage;

        //Click playing button
        public void ClickedPlay(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _choosegamescreen.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _passwordchangepannel.SetActive(false);
                _passwordchangepannelunnokwn.SetActive(false);
                _AuthorithationPannel.SetActive(false);

                _choosegamescreen.SetActive(true);
            });

        }

        public void ClickedBackToTitle(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _choosegamescreen.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _passwordchangepannel.SetActive(false);
                _passwordchangepannelunnokwn.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _choosegamescreen.SetActive(false);
                _loginPannel.SetActive(false);
                _passwordchangepannelunnokwn.SetActive(false);
                _leaderboardPannel.SetActive(false) ;
                _levelPanelColorsort.SetActive(false);

                _titlePanel.SetActive(true);

            });
        }

        public void ClickedBackToGamemodes(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _choosegamescreen.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _passwordchangepannel.SetActive(false);
                _passwordchangepannelunnokwn.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _choosegamescreen.SetActive(false);

                _choosegamescreen.SetActive(true);
            });
        }

        public void QuitGame(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                Application.Quit();
            });
        }

        public void ClickedConnect(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelConnect.SetActive(true);
                _levelTitleImage.color = CurrentColor;
                LevelOpened?.Invoke();
            });

        }

        public void ClickedColorSort(Button button)
        {

            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelColorsort.SetActive(true);
                _levelTitleImage.color = CurrentColor;
                LevelOpened?.Invoke();
            });

        }

        public void ClickLogin(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _loginPannel.SetActive(true);
            });

        }

        private void ClickPasswordChange()
        {
            _titlePanel.SetActive(false);
            _levelPanelConnect.SetActive(false);
            _levelPanelColorsort.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _choosegamescreen.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _AuthorithationPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _loginPannel.SetActive(false);
        }

        private void ClickForgotPassword()
        {
            _titlePanel.SetActive(false);
            _levelPanelConnect.SetActive(false);
            _levelPanelColorsort.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _choosegamescreen.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _AuthorithationPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _loginPannel.SetActive(false);
            _passwordchangepannelunnokwn.SetActive(false);
        }

        public void CLickProfile(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _levelPanelConnect.SetActive(false);
                _levelPanelColorsort.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _choosegamescreen.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _loginPannel.SetActive(false);
                _profilePannel.SetActive(true);
            });
        }

        public void ClickedPipes(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(true);
                _levelTitleImage.color = CurrentColor;
                LevelOpened?.Invoke();
            });
        }

        public void ClickedDailyChallenge(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToDailyChallenge();
            });


        }

        public void ClickedLeaderboard(Button button)
        {
            AnimateAndSwitch(button, () =>
            {


                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(false);

                string nick = PlayerPrefs.GetString("Nick", "");
                string password = PlayerPrefs.GetString("Password", "");

                if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(password))
                {
                    _leaderboardPannel.SetActive(false);
                    AthorisationEnable();
                }
                else
                {
                    StartCoroutine(UpdateAndCheckServer(nick, password));
                }
            });
        }

        private IEnumerator UpdateAndCheckServer(string nick, string password)
        {
            if (GameManager.Instance != null)
            {
                DBManager.connectLevels = GameManager.Instance.CurrentLevelConnect;
                DBManager.colorsortLevels = GameManager.Instance.CurrentLevelColorsort;
                DBManager.pipesLevels = GameManager.Instance.CurrentLevelPipes;
                DBManager.levelscore = PlayerPrefs.GetInt("LevelScore", 0);
                DBManager.infinityscore = PlayerPrefs.GetInt("InfinityScore", 0);

                PlayerPrefs.SetInt("ConnectLevels", DBManager.connectLevels);
                PlayerPrefs.SetInt("ColorsortLevels", DBManager.colorsortLevels);
                PlayerPrefs.SetInt("PipesLevels", DBManager.pipesLevels);
                PlayerPrefs.SetInt("LevelScore", DBManager.levelscore);
                PlayerPrefs.SetInt("InfinityScore", DBManager.infinityscore);
                PlayerPrefs.Save();

                if (DBManager.LoggedIn)
                {
                    yield return StartCoroutine(GameManager.Instance.UpdateServerData());
                }
            }
            yield return StartCoroutine(CheckServerData(nick, password));
            _leaderboardPannel.SetActive(true); 
            StartCoroutine(FetchLeaderboard()); 
        }

        public void AthorisationEnable()
        {

            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _AuthorithationPannel.SetActive(true);
        }

        public void Registration(Button button)
        {
            AnimateAndSwitch(button, () =>
            {

                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _registerPannel.SetActive(true);
                _AuthorithationPannel.SetActive(false);
            });
        }

        public void Authorisation()
        {
            _AuthorithationPannel.SetActive(true);
        }

        public void ClickForgetPassword(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _passwordchangepannelunnokwn.SetActive(true);
                ChangePasswordUnKnown();
            });

        }

        public void ClickChangePassword()
        {
            _titlePanel.SetActive(false);
            _choosegamescreen.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _passwordchangepannel.SetActive(true);
        }
        #endregion

        #region DATABASE_WORK
        [System.Serializable]
        private class LoginResponse
        {
            public string status;
            public string nick;
            public string email;
            public int infinityscore;
            public int colorsortLevels;
            public int levelscore;
            public int connectLevels;
            public int pipesLevels;
        }

        [System.Serializable]
        private class LeaderboardEntry
        {
            public string nick;
            public int totalScore;
        }

        [System.Serializable]
        private class LeaderboardResponse
        {
            public string status;
            public LeaderboardEntry[] leaderboard;
        }


        [System.Serializable]
        private class ResetResponse
        {
            public string status;
            public string message;
        }

        public void CallRegister()
        {
            StartCoroutine(Register(nickfield.text, passwordfield.text, emailfield.text));
        }

        IEnumerator Register(string nick, string password, string email)
        {
            string url = "http://93.81.252.217/unity/register.php";
            WWWForm form = new WWWForm();
            form.AddField("nick", nick);
            form.AddField("password", password);
            form.AddField("email", email);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("Registration Success: " + response);

                    if (response == "0")
                    {
                        PlayerPrefs.SetString("Nick", nick);
                        PlayerPrefs.SetString("Password", password);
                        PlayerPrefs.SetString("Email", email);
                        PlayerPrefs.Save();

                        Debug.Log("Data was saved: " + nick + ", " + password + ", " + email);
                        yield return StartCoroutine(Login(nick, password));
                    }
                    else
                    {
                        Debug.Log("Error registration: " + response);
                        ErrorRegistration.text = response;
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                }
            }
        }

        public void CallLogin(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                StartCoroutine(Login(nickfieldlogin.text, passwordfieldlogin.text));
            });

        }

        public IEnumerator Login(string nick, string password)
        {
            string url = "http://93.81.252.217/unity/login.php";
            WWWForm form = new WWWForm();
            form.AddField("nick", nick);
            form.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("Login response: " + response);


                    if (string.IsNullOrEmpty(response))
                    {
                        Debug.LogError("Empty response from server");
                        yield break;
                    }

                    if (response.Contains("status"))
                    {
                        try
                        {
                            LoginResponse loginData = JsonUtility.FromJson<LoginResponse>(response);
                            if (loginData.status == "0")
                            {
                                DBManager.nick = loginData.nick;
                                DBManager.email = loginData.email;
                                DBManager.infinityscore = loginData.infinityscore;
                                DBManager.colorsortLevels = loginData.colorsortLevels;
                                DBManager.connectLevels = loginData.connectLevels;
                                DBManager.pipesLevels = loginData.pipesLevels;
                                DBManager.levelscore = loginData.levelscore;

                                PlayerPrefs.SetString("Nick", loginData.nick);
                                PlayerPrefs.SetString("Password", password);
                                PlayerPrefs.SetString("Email", loginData.email);
                                PlayerPrefs.SetInt("InfinityScore", loginData.infinityscore);
                                PlayerPrefs.SetInt("LevelScore", loginData.levelscore);
                                PlayerPrefs.SetInt("ColorsortLevels", loginData.colorsortLevels);
                                PlayerPrefs.SetInt("ConnectLevels", loginData.connectLevels);
                                PlayerPrefs.SetInt("PipesLevels", loginData.pipesLevels);

                                for (int i = 1; i <= loginData.connectLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Connect", 1);
                                for (int i = 1; i <= loginData.colorsortLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Colorsort", 1);
                                for (int i = 1; i <= loginData.pipesLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Pipes", 1);

                                PlayerPrefs.Save();

                                if (GameManager.Instance != null)
                                {
                                    GameManager.Instance.CurrentLevelConnect = loginData.connectLevels;
                                    GameManager.Instance.CurrentLevelColorsort = loginData.colorsortLevels;
                                    GameManager.Instance.CurrentLevelPipes = loginData.pipesLevels;
                                }

                                int localColorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
                                int localConnectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
                                int localPipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);

                                if (localColorsortLevels != loginData.colorsortLevels ||
                                    localConnectLevels != loginData.connectLevels ||
                                    localPipesLevels != loginData.pipesLevels)
                                {
                                    ShowSyncPanel(loginData, localColorsortLevels, localConnectLevels, localPipesLevels);
                                }
                                else
                                {
                                    SaveLoginData(loginData);
                                    ClickedLeaderboard(registerbutton);

                                }

                            }
                            else
                            {
                                Debug.Log("Login failed: " + response);
                                _loginerrortext.text = "Login Failed" + response;

                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("JSON parse error: " + e.Message);
                        }
                    }
                    else
                    {
                        _loginerrortext.text = "Login Failed" + response;

                        Debug.Log("Login failed: " + response);
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                }
            }
        }

        private void KeepLocalData(LoginResponse loginData)
        {
            DBManager.colorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
            DBManager.connectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
            DBManager.pipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);
            DBManager.levelscore = PlayerPrefs.GetInt("LevelScore", 0);
            DBManager.infinityscore = PlayerPrefs.GetInt("InfinityScore", 0);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentLevelConnect = DBManager.connectLevels;
                GameManager.Instance.CurrentLevelColorsort = DBManager.colorsortLevels;
                GameManager.Instance.CurrentLevelPipes = DBManager.pipesLevels;
            }

            if (DBManager.LoggedIn)
            {
                StartCoroutine(GameManager.Instance.UpdateServerData());
            }
            SaveLoginData(loginData);
            _syncPanel.SetActive(false);
        }

        private void UpdateFromDb(LoginResponse loginData)
        {
            DBManager.colorsortLevels = loginData.colorsortLevels;
            DBManager.connectLevels = loginData.connectLevels;
            DBManager.pipesLevels = loginData.pipesLevels;
            DBManager.levelscore = loginData.levelscore;
            DBManager.infinityscore = loginData.infinityscore;

            PlayerPrefs.SetInt("ColorsortLevels", loginData.colorsortLevels);
            PlayerPrefs.SetInt("ConnectLevels", loginData.connectLevels);
            PlayerPrefs.SetInt("PipesLevels", loginData.pipesLevels);
            PlayerPrefs.SetInt("LevelScore", loginData.levelscore);
            PlayerPrefs.SetInt("InfinityScore", loginData.infinityscore);
            PlayerPrefs.Save();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentLevelConnect = loginData.connectLevels;
                GameManager.Instance.CurrentLevelColorsort = loginData.colorsortLevels;
                GameManager.Instance.CurrentLevelPipes = loginData.pipesLevels;
            }

            SaveLoginData(loginData);
            _syncPanel.SetActive(false);
        }

        private void SaveLoginData(LoginResponse loginData)
        {
            PlayerPrefs.SetString("Nick", loginData.nick);
            PlayerPrefs.SetString("Email", loginData.email);
            PlayerPrefs.SetInt("InfinityScore", loginData.infinityscore);
            PlayerPrefs.SetInt("LevelScore", loginData.levelscore);
            PlayerPrefs.SetInt("ColorsortLevels", loginData.colorsortLevels);
            PlayerPrefs.SetInt("ConnectLevels", loginData.connectLevels);
            PlayerPrefs.SetInt("PipesLevels", loginData.pipesLevels);
            PlayerPrefs.Save();
        }

        private void ShowSyncPanel(LoginResponse loginData, int localColorsort, int localConnect, int localPipes)
        {
            _syncPanel.SetActive(true);
            _syncMessage.text = $"Data mismatch detected!\n" +
                                $"Local: Colorsort={localColorsort}, Connect={localConnect}, Pipes={localPipes}\n" +
                                $"Server: Colorsort={loginData.colorsortLevels}, Connect={loginData.connectLevels}, Pipes={loginData.pipesLevels}\n" +
                                $"Choose an option:";

            _keepLocalButton.onClick.RemoveAllListeners();
            _keepLocalButton.onClick.AddListener(() => KeepLocalData(loginData));
            _updateFromDbButton.onClick.RemoveAllListeners();
            _updateFromDbButton.onClick.AddListener(() => UpdateFromDb(loginData));
        }

        private class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        public void VerifyInputs()
        {
            registerbutton.interactable = (nickfield.text.Length >= 2 && passwordfield.text.Length > 2 && emailfield.text.Contains('@'));
        }

        public void VerifyInputsLogin()
        {
            LoginButton.interactable = (nickfieldlogin.text.Length >= 2 && passwordfieldlogin.text.Length > 2);
        }

        private IEnumerator CheckServerData(string nick, string password)
        {
            string url = "http://93.81.252.217/unity/login.php";
            WWWForm form = new WWWForm();
            form.AddField("nick", nick);
            form.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("CheckServerData response: " + response);

                    if (string.IsNullOrEmpty(response))
                    {
                        Debug.LogError("Empty response from server");
                        AthorisationEnable();
                        yield break;
                    }

                    if (response.Contains("status") && response.StartsWith("{"))
                    {
                        try
                        {
                            LoginResponse loginData = JsonUtility.FromJson<LoginResponse>(response);
                            if (loginData.status == "0")
                            {
                                DBManager.nick = loginData.nick;
                                DBManager.email = loginData.email;
                                DBManager.infinityscore = loginData.infinityscore;
                                DBManager.colorsortLevels = loginData.colorsortLevels;
                                DBManager.levelscore = loginData.levelscore;
                                DBManager.connectLevels = loginData.connectLevels;
                                DBManager.pipesLevels = loginData.pipesLevels;

                                SaveLoginData(loginData);

                                if (GameManager.Instance != null)
                                {
                                    GameManager.Instance.CurrentLevelConnect = loginData.connectLevels;
                                    GameManager.Instance.CurrentLevelColorsort = loginData.colorsortLevels;
                                    GameManager.Instance.CurrentLevelPipes = loginData.pipesLevels;
                                }

                                int localColorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
                                int localConnectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
                                int localPipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);

                                if (localColorsortLevels != loginData.colorsortLevels ||
                                    localConnectLevels != loginData.connectLevels ||
                                    localPipesLevels != loginData.pipesLevels)
                                {
                                    ShowSyncPanel(loginData, localColorsortLevels, localConnectLevels, localPipesLevels);
                                }
                                else
                                {
                                    _leaderboardPannel.SetActive(true);
                                }
                            }
                            else
                            {
                                Debug.Log("Server check failed: " + response);
                                AthorisationEnable();
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("JSON parse error in CheckServerData: " + e.Message + " | Raw response: " + response);
                            AthorisationEnable();
                        }
                    }
                    else
                    {
                        Debug.Log("Server check failed (not JSON): " + response);
                        AthorisationEnable();
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                    AthorisationEnable();
                }
            }
        }

        public void ClickLogout(Button button)
        {
            AnimateAndSwitch(button, () =>
            {
                DBManager.LoggedOut();
                _titlePanel.SetActive(true);
                _levelPanelConnect.SetActive(false);
                _levelPanelColorsort.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _choosegamescreen.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _registerPannel.SetActive(false);
                _loginPannel.SetActive(false);
                _registerPannel.SetActive(true);
                _leaderboardPannel.SetActive(true);
            });
        }

        private void DisplayLeaderboard(LeaderboardEntry[] entries)
        {
            if (_leaderboardContent == null)
            {
                Debug.LogError("_leaderboardContent is null");
                return;
            }
            if (_leaderboardRowPrefab == null)
            {
                Debug.LogError("_leaderboardRowPrefab is null");
                return;
            }

            foreach (Transform child in _leaderboardContent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < entries.Length; i++)
            {
                GameObject row = Instantiate(_leaderboardRowPrefab, _leaderboardContent);
                row.SetActive(true);
                TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>(true);
                if (texts.Length >= 2)
                {
                    texts[0].gameObject.SetActive(true); 
                    texts[1].gameObject.SetActive(true); 
                    texts[0].text = $"{i + 1}. {entries[i].nick}";
                    texts[1].text = entries[i].totalScore.ToString();
                    Debug.Log($"Set text: {texts[0].text} | {texts[1].text}");
                }
            }
        }

        private IEnumerator FetchLeaderboard()
        {
            string url = "http://93.81.252.217/unity/get_leaderboard.php";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("Leaderboard response: " + response);

                    try
                    {
                        LeaderboardResponse leaderboardData = JsonUtility.FromJson<LeaderboardResponse>(response);
                        if (leaderboardData.status == "0")
                        {
                            DisplayLeaderboard(leaderboardData.leaderboard);
                        }
                        else
                        {
                            Debug.LogError("Leaderboard fetch failed: " + response);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("JSON parse error: " + e.Message + " | Raw response: " + response);
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                }
            }
        }


        public void ChangePasswordUnKnown()
        {
            Debug.Log("ChangePasswordUnKnown: Initializing password reset panel");
            if (_emailResetField != null) _emailResetField.text = "";
            if (_resetCodeField != null) _resetCodeField.text = "";
            if (_newPasswordField != null) _newPasswordField.text = "";
            if (_resetMessageText != null) _resetMessageText.text = "";

            if (_sendCodeButton != null)
            {
                Debug.Log("ChangePasswordUnKnown: Binding SendCodeButton");
                _sendCodeButton.onClick.RemoveAllListeners();
                _sendCodeButton.onClick.AddListener(SendResetCode);
            }
            else
            {
                Debug.LogError("ChangePasswordUnKnown: _sendCodeButton is null");
            }

            if (_confirmResetButton != null)
            {
                Debug.Log("ChangePasswordUnKnown: Binding ConfirmResetButton");
                _confirmResetButton.onClick.RemoveAllListeners();
                _confirmResetButton.onClick.AddListener(ConfirmPasswordReset);
            }
            else
            {
                Debug.LogError("ChangePasswordUnKnown: _confirmResetButton is null");
            }
        }

        private void SendResetCode()
        {
            if (string.IsNullOrEmpty(_emailResetField.text))
            {
                _resetMessageText.text = "Please enter your email";
                return;
            }
            Debug.Log($"SendResetCode: Starting reset code request for email: {_emailResetField.text}");
            AnimateAndSwitch(_sendCodeButton, () =>
            {

                StartCoroutine(SendResetCodeCoroutine(_emailResetField.text));
            });
        }

        private IEnumerator SendResetCodeCoroutine(string email)
        {
            string url = "http://93.81.252.217/unity/send_reset_code.php";
            Debug.Log($"SendResetCodeCoroutine: Sending POST request to {url} with email: {email}");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            Debug.Log("SendResetCodeCoroutine: Form data prepared");

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                Debug.Log("SendResetCodeCoroutine: Bypassing certificate validation");

                yield return www.SendWebRequest();
                Debug.Log("SendResetCodeCoroutine: Request sent, awaiting response");

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log($"SendResetCodeCoroutine: Response received: {response}");

                    if (string.IsNullOrEmpty(response))
                    {
                        Debug.LogError("SendResetCodeCoroutine: Server returned empty response");
                        _resetMessageText.text = "Server returned empty response";
                        yield break;
                    }

                    try
                    {
                        var resetResponse = JsonUtility.FromJson<ResetResponse>(response);
                        Debug.Log($"SendResetCodeCoroutine: Parsed response - Status: {resetResponse.status}, Message: {resetResponse.message}");

                        _resetMessageText.text = resetResponse.message;
                        if (resetResponse.status == "0")
                        {
                            Debug.Log("SendResetCodeCoroutine: Reset code sent successfully, disabling Send button");
                            _sendCodeButton.interactable = false; 
                        }
                        else
                        {
                            Debug.LogWarning($"SendResetCodeCoroutine: Server reported failure - {resetResponse.message}");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"SendResetCodeCoroutine: JSON parse error: {e.Message} | Raw response: {response}");
                        _resetMessageText.text = "Error processing response";
                    }
                }
                else
                {
                    Debug.LogError($"SendResetCodeCoroutine: Connection error: {www.error}");
                    _resetMessageText.text = "Connection error";
                }
            }
        }

        private void ConfirmPasswordReset()
        {
            AnimateAndSwitch(_confirmResetButton, () =>
            {

                if (string.IsNullOrEmpty(_emailResetField.text) ||
                string.IsNullOrEmpty(_resetCodeField.text) ||
                string.IsNullOrEmpty(_newPasswordField.text))
                {
                    _resetMessageText.text = "Please fill all fields";
                    return;
                }


                StartCoroutine(ResetPasswordCoroutine(_emailResetField.text, _resetCodeField.text, _newPasswordField.text));
            });
        }

        private IEnumerator ResetPasswordCoroutine(string email, string code, string newPassword)
        {
            string url = "http://93.81.252.217/unity/reset_password.php";
            Debug.Log($"ResetPasswordCoroutine: Sending POST request to {url} with email: {email}, code: {code}");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("code", code);
            form.AddField("new_password", newPassword);
            Debug.Log("ResetPasswordCoroutine: Form data prepared");

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.certificateHandler = new BypassCertificate();
                Debug.Log("ResetPasswordCoroutine: Bypassing certificate validation");

                yield return www.SendWebRequest();
                Debug.Log("ResetPasswordCoroutine: Request sent, awaiting response");

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log($"ResetPasswordCoroutine: Response received: {response}");

                    if (string.IsNullOrEmpty(response))
                    {
                        Debug.LogError("ResetPasswordCoroutine: Server returned empty response");
                        _resetMessageText.text = "Server returned empty response";
                        yield break;
                    }

                    try
                    {
                        var resetResponse = JsonUtility.FromJson<ResetResponse>(response);
                        Debug.Log($"ResetPasswordCoroutine: Parsed response - Status: {resetResponse.status}, Message: {resetResponse.message}");

                        _resetMessageText.text = resetResponse.message;
                        if (resetResponse.status == "0")
                        {
                            Debug.Log("ResetPasswordCoroutine: Password updated successfully, saving new password and redirecting to login");
                            PlayerPrefs.SetString("Password", newPassword);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            Debug.LogWarning($"ResetPasswordCoroutine: Server reported failure - {resetResponse.message}");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"ResetPasswordCoroutine: JSON parse error: {e.Message} | Raw response: {response}");
                        _resetMessageText.text = "Error processing response";
                    }
                }
                else
                {
                    Debug.LogError($"ResetPasswordCoroutine: Connection error: {www.error}");
                    _resetMessageText.text = "Connection error";
                }
            }
        }
        #endregion


        #region Animations

        public void Animate(GameObject target, System.Action onComplete, float duration = 1f)
        {

            if (playStartTween != null && playStartTween.IsActive())
            {
                playStartTween.Kill();
            }

            playStartTween = target.transform
            .DOScale(1.1f, 0.1f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo).OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }
        
        private void AnimateAndSwitch(Button button, System.Action switchAction)
        {
            if (button != null)
            {
                Animate(button.gameObject, switchAction);
            }
            else {
                Debug.LogError("Button is null");
                switchAction?.Invoke();
            }
        }

        #endregion
        #endregion
    }
}