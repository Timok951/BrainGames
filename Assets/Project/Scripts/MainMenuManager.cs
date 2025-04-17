using Assets.Project.Scripts;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Assets.Project.Scripts.Database;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Connect.Core
{
    /// <summary>
    /// Class for working with main menu, also contains methods to work with database
    /// </summary>
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
        [SerializeField] private TMP_Text _registererrortext;
        [SerializeField] private TMP_Text _yourdatatext;

        [SerializeField] private int localeIdrus;
        [SerializeField] private int localeIdus;
        #endregion

        #region LOCALE_SELECTOR
        [SerializeField] private Button enbut;
        [SerializeField] private Button rubtn;
        private bool active = false;

        IEnumerator SetLocale(int _localeID)
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
            PlayerPrefs.SetInt("SelectedLocaleID", _localeID);
            PlayerPrefs.Save();
        }

        public void CliclkChangeLocale(int localeID, Button button)
        {
            if (active) return;
            active = true;
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                StartCoroutine(SetLocale(localeID));
                active = false;
            });
        }
        #endregion

        #region START_REGION
        AnimationDotWeen AnimationDotWeen = new AnimationDotWeen();

        private void Awake()
        {
            Instance = this;
            MainMenuShow();

            SetupInputField(nickfield, TouchScreenKeyboardType.Default);
            SetupInputField(passwordfield, TouchScreenKeyboardType.Default, true);
            SetupInputField(emailfield, TouchScreenKeyboardType.EmailAddress);
            SetupInputField(nickfieldlogin, TouchScreenKeyboardType.Default);
            SetupInputField(passwordfieldlogin, TouchScreenKeyboardType.Default, true);
            SetupInputField(_emailResetField, TouchScreenKeyboardType.EmailAddress);
            SetupInputField(_resetCodeField, TouchScreenKeyboardType.NumberPad);
            SetupInputField(_newPasswordField, TouchScreenKeyboardType.Default, true);

            enbut.onClick.AddListener(() => CliclkChangeLocale(localeIdus, enbut));
            rubtn.onClick.AddListener(() => CliclkChangeLocale(localeIdrus, rubtn));

            // Восстановление сохраненного языка
            int savedLocaleID = PlayerPrefs.GetInt("SelectedLocaleID", localeIdus);
            StartCoroutine(SetLocale(savedLocaleID));
        }

        private void MainMenuShow()
        {
            _titlePanel.SetActive(false);
            _levelPanelConnect.SetActive(false);
            _levelPanelPipes.SetActive(false);
            _choosegamescreen.SetActive(false);
            _leaderboardPannel.SetActive(false);
            _registerPannel.SetActive(false);
            _passwordchangepannelunnokwn.SetActive(false);
            _AuthorithationPannel.SetActive(false);
            _loginPannel.SetActive(false);
            _levelPanelColorsort.SetActive(false);

            _titlePanel.SetActive(true);
        }
        #endregion

        #region UPDATE_REGION
        #region MENU_REGION
        public UnityAction LevelOpened;

        [SerializeField] public Color CurrentColor;
        [SerializeField] private TMP_Text _levelTitleText;
        [SerializeField] private UnityEngine.UI.Image _levelTitleImage;

        public void ClickedPlay(Button button)
        {
            Debug.Log($"ClickedPlay called. Button: {(button != null ? button.name : "null")}");
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(true);
            });
        }

        public void ClickedBackToTitle(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                MainMenuShow();
            });
        }

        public void ClickedBackToGamemodes(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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

        public void QuitGame(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                Application.Quit();
            });
        }

        public void ClickedConnect(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                _titlePanel.SetActive(false);
                _choosegamescreen.SetActive(false);
                _levelPanelPipes.SetActive(false);
                _leaderboardPannel.SetActive(false);
                _AuthorithationPannel.SetActive(false);
                _loginPannel.SetActive(true);
            });
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
            _passwordchangepannelunnokwn.SetActive(true);
            ChangePasswordUnKnown();
        }

        public void CLickProfile(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                GameManager.Instance.GoToDailyChallenge();
            });
        }

        public void ClickedLeaderboard(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
        public void CallRegister()
        {
            StartCoroutine(Register(nickfield.text, passwordfield.text, emailfield.text));
        }

        private void ClearLevelPrefs()
        {
            PlayerPrefs.DeleteKey("LevelScore");
            PlayerPrefs.DeleteKey("InfinityScore");
            PlayerPrefs.DeleteKey("ColorsortLevels");
            PlayerPrefs.DeleteKey("ConnectLevels");
            PlayerPrefs.DeleteKey("PipesLevels");

            for (int i = 1; i <= 100; i++)
            {
                PlayerPrefs.DeleteKey("Level" + i + "Connect");
                PlayerPrefs.DeleteKey("Level" + i + "Colorsort");
                PlayerPrefs.DeleteKey("Level" + i + "Pipes");
            }

            PlayerPrefs.Save();
            Debug.Log("Cleared all level-related PlayerPrefs");
        }

        IEnumerator Register(string nick, string password, string email)
        {
            string url = "http://93.81.252.217/unity/register.php";
            WWWForm form = new WWWForm();
            form.AddField("nick", nick);
            form.AddField("password", password);
            form.AddField("email", email);
            ClearLevelPrefs();

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

                        PlayerPrefs.SetInt("LevelScore", 1);
                        DBManager.levelscore = 1;

                        PlayerPrefs.SetInt("InfinityScore", 0);
                        PlayerPrefs.SetInt("ColorsortLevels", 0);
                        PlayerPrefs.SetInt("ConnectLevels", 0);
                        PlayerPrefs.SetInt("PipesLevels", 0);

                        PlayerPrefs.Save();

                        Debug.Log("Data was saved: " + nick + ", " + password + ", " + email);
                        yield return StartCoroutine(Login(nick, password));
                        _registerPannel.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Error registration: " + response);
                        _registererrortext.text = new LocalizedString("StringMainMenuTable", "RegisterError") { Arguments = new object[] { response } }.GetLocalizedString();
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                    _registererrortext.text = new LocalizedString("StringMainMenuTable", "RegisterError") { Arguments = new object[] { www.error } }.GetLocalizedString();
                }
            }
        }

        public void CallLogin(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
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
                        _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Empty response from server" } }.GetLocalizedString();
                        yield break;
                    }

                    if (response.Contains("status"))
                    {
                        try
                        {
                            LoginResponse loginData = JsonConvert.DeserializeObject<LoginResponse>(response);
                            if (loginData == null)
                            {
                                Debug.LogError("Deserialization returned null");
                                _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                                yield break;
                            }

                            if (loginData.Status == "0")
                            {
                                DBManager.nick = loginData.Nick;
                                DBManager.email = loginData.Email;
                                DBManager.infinityscore = loginData.InfinityScore;
                                DBManager.colorsortLevels = loginData.ColorsortLevels;
                                DBManager.connectLevels = loginData.ConnectLevels;
                                DBManager.pipesLevels = loginData.PipesLevels;
                                DBManager.levelscore = loginData.LevelScore;

                                PlayerPrefs.SetString("Nick", loginData.Nick);
                                PlayerPrefs.SetString("Password", password);
                                PlayerPrefs.SetString("Email", loginData.Email);
                                PlayerPrefs.SetInt("InfinityScore", loginData.InfinityScore);
                                PlayerPrefs.SetInt("LevelScore", loginData.LevelScore);
                                PlayerPrefs.SetInt("ColorsortLevels", loginData.ColorsortLevels);
                                PlayerPrefs.SetInt("ConnectLevels", loginData.ConnectLevels);
                                PlayerPrefs.SetInt("PipesLevels", loginData.PipesLevels);

                                for (int i = 1; i <= loginData.ConnectLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Connect", 1);
                                for (int i = 1; i <= loginData.ColorsortLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Colorsort", 1);
                                for (int i = 1; i <= loginData.PipesLevels; i++)
                                    PlayerPrefs.SetInt("Level" + i + "Pipes", 1);

                                PlayerPrefs.Save();

                                if (GameManager.Instance != null)
                                {
                                    GameManager.Instance.CurrentLevelConnect = loginData.ConnectLevels;
                                    GameManager.Instance.CurrentLevelColorsort = loginData.ColorsortLevels;
                                    GameManager.Instance.CurrentLevelPipes = loginData.PipesLevels;
                                }

                                int localColorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
                                int localConnectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
                                int localPipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);

                                if (localColorsortLevels != loginData.ColorsortLevels ||
                                    localConnectLevels != loginData.ConnectLevels ||
                                    localPipesLevels != loginData.PipesLevels)
                                {
                                    ShowSyncPanel(loginData, localColorsortLevels, localConnectLevels, localPipesLevels);
                                }
                                else
                                {
                                    SaveLoginData(loginData);
                                    _loginPannel.gameObject.SetActive(false);
                                    ClickedLeaderboard(registerbutton);
                                    _yourdatatext.text = new LocalizedString("StringMainMenuTable", "YourData") { Arguments = new object[] { loginData.Nick, loginData.LevelScore } }.GetLocalizedString();
                                }
                            }
                            else
                            {
                                Debug.Log("Login failed: " + response);
                                _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { loginData.Status } }.GetLocalizedString();
                            }
                        }
                        catch (JsonException e)
                        {
                            Debug.LogError($"JSON parse error: {e.Message}. Raw response: {response}");
                            _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                        }
                    }
                    else
                    {
                        _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { response } }.GetLocalizedString();
                        Debug.Log("Login failed: " + response);
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                    _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { www.error } }.GetLocalizedString();
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
            _yourdatatext.text = new LocalizedString("StringMainMenuTable", "YourData") { Arguments = new object[] { loginData.Nick, DBManager.levelscore } }.GetLocalizedString();
        }

        private void UpdateFromDb(LoginResponse loginData)
        {
            DBManager.colorsortLevels = loginData.ColorsortLevels;
            DBManager.connectLevels = loginData.ConnectLevels;
            DBManager.pipesLevels = loginData.PipesLevels;
            DBManager.levelscore = loginData.LevelScore;
            DBManager.infinityscore = loginData.InfinityScore;

            PlayerPrefs.SetInt("ColorsortLevels", loginData.ColorsortLevels);
            PlayerPrefs.SetInt("ConnectLevels", loginData.ConnectLevels);
            PlayerPrefs.SetInt("PipesLevels", loginData.PipesLevels);
            PlayerPrefs.SetInt("LevelScore", loginData.LevelScore);
            PlayerPrefs.SetInt("InfinityScore", loginData.InfinityScore);
            PlayerPrefs.Save();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentLevelConnect = loginData.ConnectLevels;
                GameManager.Instance.CurrentLevelColorsort = loginData.ColorsortLevels;
                GameManager.Instance.CurrentLevelPipes = loginData.PipesLevels;
            }

            SaveLoginData(loginData);
            _syncPanel.SetActive(false);
            _yourdatatext.text = new LocalizedString("StringMainMenuTable", "YourData") { Arguments = new object[] { loginData.Nick, loginData.LevelScore } }.GetLocalizedString();
        }

        private void SaveLoginData(LoginResponse loginData)
        {
            PlayerPrefs.SetString("Nick", loginData.Nick);
            PlayerPrefs.SetString("Email", loginData.Email);
            PlayerPrefs.SetInt("InfinityScore", loginData.InfinityScore);
            PlayerPrefs.SetInt("LevelScore", loginData.LevelScore);
            PlayerPrefs.SetInt("ColorsortLevels", loginData.ColorsortLevels);
            PlayerPrefs.SetInt("ConnectLevels", loginData.ConnectLevels);
            PlayerPrefs.SetInt("PipesLevels", loginData.PipesLevels);
            PlayerPrefs.Save();
        }

        private void ShowSyncPanel(LoginResponse loginData, int localColorsort, int localConnect, int localPipes)
        {
            _syncPanel.SetActive(true);
            _syncMessage.text = new LocalizedString("StringMainMenuTable", "SyncMessage")
            {
                Arguments = new object[] { localColorsort, localConnect, localPipes, loginData.ColorsortLevels, loginData.ConnectLevels, loginData.PipesLevels }
            }.GetLocalizedString();

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

        private void SetupInputField(TMP_InputField inputField, TouchScreenKeyboardType keyboardType, bool isPassword = false)
        {
            if (inputField == null)
            {
                Debug.LogError("InputField is null");
                return;
            }

            inputField.keyboardType = keyboardType;
            inputField.shouldHideMobileInput = false;

            inputField.onSelect.AddListener((text) =>
            {
                Debug.Log($"InputField {inputField.name} selected");
                if (Application.isMobilePlatform)
                {
                    var keyboard = TouchScreenKeyboard.Open("", keyboardType, true, false, isPassword);
                    Debug.Log($"Keyboard opened: {keyboard != null}, status: {(keyboard != null ? keyboard.status : "null")}");
                }
                else
                {
                    Debug.LogWarning("Not a mobile platform, skipping TouchScreenKeyboard.Open");
                }
            });
        }

        public void VerifyInputs()
        {
            registerbutton.interactable = (nickfield.text.Length >= 2 && passwordfield.text.Length > 2 && emailfield.text.Contains('@'));
            if (passwordfield.text.Length < 2)
            {
                _registererrortext.text = new LocalizedString("StringMainMenuTable", "PasswordTooShort").GetLocalizedString();
            }
            else if (!emailfield.text.Contains('@'))
            {
                _registererrortext.text = new LocalizedString("StringMainMenuTable", "InvalidEmail").GetLocalizedString();
            }
            else
            {
                _registererrortext.text = "";
            }
        }

        public void VerifyInputsforChangepassword()
        {
            _sendCodeButton.interactable = (_emailResetField.text.Contains('@'));

            if (!_emailResetField.text.Contains('@'))
            {
                _resetMessageText.text = new LocalizedString("StringMainMenuTable", "InvalidEmail").GetLocalizedString();
            }
            else
            {
                _resetMessageText.text = "";
            }
        }

        public void VerivyInputsfroChangepasswordbutton()
        {
            _confirmResetButton.interactable = (_resetCodeField.text.Length > 2 && _newPasswordField.text.Length > 2);

            if (!_sendCodeButton.interactable)
            {
                if (_resetCodeField.text.Length < 2)
                {
                    _resetMessageText.text = new LocalizedString("StringMainMenuTable", "InvalidResetCode").GetLocalizedString();
                }
                else if (_newPasswordField.text.Length < 2)
                {
                    _resetMessageText.text = new LocalizedString("StringMainMenuTable", "PasswordTooShort").GetLocalizedString();
                }
                else
                {
                    _resetMessageText.text = "";
                }
            }
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
                        _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Empty response from server" } }.GetLocalizedString();
                        yield break;
                    }

                    if (response.Contains("status") && response.StartsWith("{"))
                    {
                        try
                        {
                            LoginResponse loginData = JsonConvert.DeserializeObject<LoginResponse>(response);
                            if (loginData == null)
                            {
                                Debug.LogError("Deserialization returned null");
                                AthorisationEnable();
                                _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                                yield break;
                            }

                            if (loginData.Status == "0")
                            {
                                DBManager.nick = loginData.Nick;
                                DBManager.email = loginData.Email;
                                DBManager.infinityscore = loginData.InfinityScore;
                                DBManager.colorsortLevels = loginData.ColorsortLevels;
                                DBManager.levelscore = loginData.LevelScore;
                                DBManager.connectLevels = loginData.ConnectLevels;
                                DBManager.pipesLevels = loginData.PipesLevels;

                                SaveLoginData(loginData);

                                if (GameManager.Instance != null)
                                {
                                    GameManager.Instance.CurrentLevelConnect = loginData.ConnectLevels;
                                    GameManager.Instance.CurrentLevelColorsort = loginData.ColorsortLevels;
                                    GameManager.Instance.CurrentLevelPipes = loginData.PipesLevels;
                                }

                                int localColorsortLevels = PlayerPrefs.GetInt("ColorsortLevels", 0);
                                int localConnectLevels = PlayerPrefs.GetInt("ConnectLevels", 0);
                                int localPipesLevels = PlayerPrefs.GetInt("PipesLevels", 0);

                                if (localColorsortLevels != loginData.ColorsortLevels ||
                                    localConnectLevels != loginData.ConnectLevels ||
                                    localPipesLevels != loginData.PipesLevels)
                                {
                                    ShowSyncPanel(loginData, localColorsortLevels, localConnectLevels, localPipesLevels);
                                }
                                else
                                {
                                    _leaderboardPannel.SetActive(true);
                                    _yourdatatext.text = new LocalizedString("StringMainMenuTable", "YourData") { Arguments = new object[] { loginData.Nick, loginData.LevelScore } }.GetLocalizedString();
                                }
                            }
                            else
                            {
                                Debug.Log("Server check failed: " + response);
                                AthorisationEnable();
                                _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { response } }.GetLocalizedString();
                            }
                        }
                        catch (JsonException e)
                        {
                            Debug.LogError($"JSON parse error in CheckServerData: {e.Message}. Raw response: {response}");
                            AthorisationEnable();
                            _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                        }
                    }
                    else
                    {
                        Debug.Log("Server check failed (not JSON): " + response);
                        AthorisationEnable();
                        _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { response } }.GetLocalizedString();
                    }
                }
                else
                {
                    Debug.LogError("Connect error: " + www.error);
                    AthorisationEnable();
                    _loginerrortext.text = new LocalizedString("StringMainMenuTable", "LoginError") { Arguments = new object[] { www.error } }.GetLocalizedString();
                }
            }
        }

        public void ClickLogout(Button button)
        {
            AnimationDotWeen.AnimateAndSwitch(button, () =>
            {
                DBManager.LoggedOut();
                MainMenuShow();
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
                    texts[0].text = $"{i + 1}. {entries[i].Nick}";
                    texts[1].text = entries[i].TotalScore.ToString();
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
                        LeaderboardResponse leaderboardData = JsonConvert.DeserializeObject<LeaderboardResponse>(response);
                        if (leaderboardData == null)
                        {
                            Debug.LogError("Deserialization returned null");
                            yield break;
                        }

                        if (leaderboardData.Status == "0")
                        {
                            DisplayLeaderboard(leaderboardData.Leaderboard);
                            _yourdatatext.text = new LocalizedString("StringMainMenuTable", "YourData") { Arguments = new object[] { DBManager.nick, DBManager.levelscore } }.GetLocalizedString();
                        }
                        else
                        {
                            Debug.LogError("Leaderboard fetch failed: " + response);
                        }
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError($"JSON parse error: {e.Message}. Raw response: {response}");
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
            _loginPannel.SetActive(false);
            Debug.Log("ChangePasswordUnKnown: Initializing password reset panel");
            if (_emailResetField != null) _emailResetField.text = "";
            if (_resetCodeField != null) _resetCodeField.text = "";
            if (_newPasswordField != null) _newPasswordField.text = "";
            if (_resetMessageText != null) _resetMessageText.text = "";

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
            _confirmResetButton.interactable = false;
            _sendCodeButton.interactable = false;
        }

        public void SendResetCode()
        {
            if (string.IsNullOrEmpty(_emailResetField.text))
            {
                _resetMessageText.text = new LocalizedString("StringMainMenuTable", "EmptyEmail").GetLocalizedString();
                return;
            }
            Debug.Log($"SendResetCode: Starting reset code request for email: {_emailResetField.text}");

            AnimationDotWeen.AnimateAndSwitch(_sendCodeButton, () =>
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
                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Server returned empty response" } }.GetLocalizedString();
                        yield break;
                    }

                    try
                    {
                        ResetResponse resetResponse = JsonConvert.DeserializeObject<ResetResponse>(response);
                        if (resetResponse == null)
                        {
                            Debug.LogError("Deserialization returned null");
                            _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                            yield break;
                        }

                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { resetResponse.Message } }.GetLocalizedString();
                        if (resetResponse.Status == "0")
                        {
                            Debug.Log("SendResetCodeCoroutine: Reset code sent successfully, disabling Send button");
                            _sendCodeButton.interactable = false;
                        }
                        else
                        {
                            Debug.LogWarning($"SendResetCodeCoroutine: Server reported failure - {resetResponse.Message}");
                        }
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError($"SendResetCodeCoroutine: JSON parse error: {e.Message}. Raw response: {response}");
                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                    }
                }
                else
                {
                    Debug.LogError($"SendResetCodeCoroutine: Connection error: {www.error}");
                    _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Connection error" } }.GetLocalizedString();
                }
            }
        }

        private void ConfirmPasswordReset()
        {
            AnimationDotWeen.AnimateAndSwitch(_confirmResetButton, () =>
            {
                if (string.IsNullOrEmpty(_emailResetField.text) ||
                    string.IsNullOrEmpty(_resetCodeField.text) ||
                    string.IsNullOrEmpty(_newPasswordField.text))
                {
                    _resetMessageText.text = new LocalizedString("StringMainMenuTable", "FillAllFields").GetLocalizedString();
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
                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Server returned empty response" } }.GetLocalizedString();
                        yield break;
                    }

                    try
                    {
                        ResetResponse resetResponse = JsonConvert.DeserializeObject<ResetResponse>(response);
                        if (resetResponse == null)
                        {
                            Debug.LogError("Deserialization returned null");
                            _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                            yield break;
                        }

                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { resetResponse.Message } }.GetLocalizedString();
                        if (resetResponse.Status == "0")
                        {
                            Debug.Log("ResetPasswordCoroutine: Password updated successfully, saving new password and redirecting to login");
                            PlayerPrefs.SetString("Password", newPassword);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            Debug.LogWarning($"ResetPasswordCoroutine: Server reported failure - {resetResponse.Message}");
                        }
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError($"ResetPasswordCoroutine: JSON parse error: {e.Message}. Raw response: {response}");
                        _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Error processing response" } }.GetLocalizedString();
                    }
                }
                else
                {
                    Debug.LogError($"ResetPasswordCoroutine: Connection error: {www.error}");
                    _resetMessageText.text = new LocalizedString("StringMainMenuTable", "ResetMessage") { Arguments = new object[] { "Connection error" } }.GetLocalizedString();
                }
            }
        }
        #endregion
        #endregion
    }
}

