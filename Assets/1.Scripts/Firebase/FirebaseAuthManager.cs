using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Firebase;
using Photon.Pun;

public class FirebaseAuthManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField signUpEmailInputField;       // 회원가입 - 이메일 입력 필드
    public TMP_InputField signUpPasswordInputField;    // 회원가입 - 비밀번호 입력 필드
    public TMP_InputField signUpUsernameInputField;    // 회원가입 - 아이디 입력 필드

    public TMP_InputField loginEmailInputField;        // 로그인 - 이메일 입력 필드
    public TMP_InputField loginPasswordInputField;     // 로그인 - 비밀번호 입력 필드

    public GameObject loginPanel;                     // 로그인 창 패널
    public GameObject signUpPanel;                    // 회원가입 창 패널

    public TMP_Text userIDText;                        // 로그인 후 사용자 아이디를 표시할 Text

    public Toggle autoLoginToggle;                    // 자동 로그인 토글 버튼

    public GameObject warningPanel;                   // 경고 메시지 패널
    public TextMeshProUGUI warningText;               // 경고 메시지를 표시할 Text

    public TextMeshProUGUI networkStateText;

    private FirebaseAuth auth;
    private DatabaseReference databaseRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                databaseRef = FirebaseDatabase.DefaultInstance?.RootReference;

                CheckLoginStatus();
            }
            else
            {
                ShowWarning("Firebase 초기화 실패: " + task.Exception.Message);
            }
        });

        autoLoginToggle.isOn = PlayerPrefs.GetInt("AutoLogin", 0) == 1;
        autoLoginToggle.onValueChanged.AddListener(delegate { OnToggleChanged(); });

        // 포톤 서버 연결 시도
        ConnectToPhotonServer();
    }

    private void Update()
    {
        // 네트워크 상태를 표시
        networkStateText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    private async void CheckLoginStatus()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 1 && auth.CurrentUser != null)
        {
            await AutoLogin();
        }
        else if (auth.CurrentUser != null)
        {
            await LoadUsernameFromDatabase(auth.CurrentUser.UserId);
            loginPanel.SetActive(false);
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }

    private async Task AutoLogin()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            await LoadUsernameFromDatabase(user.UserId);
            loginPanel.SetActive(false);
        }
    }

    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public async void Register()
    {
        string email = signUpEmailInputField.text;
        string password = signUpPasswordInputField.text;
        string username = signUpUsernameInputField.text;

        try
        {
            FirebaseUser newUser = (await auth.CreateUserWithEmailAndPasswordAsync(email, password)).User;
            await SaveUsernameToDatabase(newUser.UserId, username);

            if (signUpPanel != null)
            {
                signUpPanel.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            ShowWarning("회원가입 실패: " + e.Message);
        }
    }

    private async Task SaveUsernameToDatabase(string userId, string username)
    {
        if (databaseRef != null)
        {
            await databaseRef.Child("users").Child(userId).Child("username").SetValueAsync(username);
        }
    }

    public async void Login()
    {
        string email = loginEmailInputField.text;
        string password = loginPasswordInputField.text;

        try
        {
            FirebaseUser loggedInUser = (await auth.SignInWithEmailAndPasswordAsync(email, password)).User;
            await LoadUsernameFromDatabase(loggedInUser.UserId);

            if (loginPanel != null)
            {
                loginPanel.SetActive(false);
            }

            PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            ShowWarning("로그인 실패: " + e.Message);
        }
    }

    private async Task LoadUsernameFromDatabase(string userId)
    {
        if (databaseRef != null)
        {
            try
            {
                DataSnapshot snapshot = await databaseRef.Child("users").Child(userId).Child("username").GetValueAsync();

                if (snapshot.Exists)
                {
                    string username = snapshot.Value.ToString();
                    if (userIDText != null)
                    {
                        userIDText.text = "아이디: " + username;
                    }
                    PhotonNetwork.NickName = username;
                }
                else
                {
                    ShowWarning("아이디 불러오기 실패: 데이터 없음");
                }
            }
            catch (System.Exception e)
            {
                ShowWarning("데이터베이스에서 유저 이름 불러오기 실패: " + e.Message);
            }
        }
    }

    public void Logout()
    {
        auth.SignOut();

        if (userIDText != null)
        {
            userIDText.text = "로그아웃 상태";
        }

        PlayerPrefs.SetInt("AutoLogin", 0);
        PlayerPrefs.Save();

        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
        }
    }

    void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 0 && auth != null)
        {
            // 자동 로그인이 꺼져 있으면 앱 종료 시 Firebase 로그아웃 처리
            auth.SignOut();
        }
    }

    // 포톤 서버 연결 함수 (경고 패널에 출력하지 않음)
    private void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 경고 메시지 표시 함수 (Firebase 관련 오류만 표시)
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);
    }
}
