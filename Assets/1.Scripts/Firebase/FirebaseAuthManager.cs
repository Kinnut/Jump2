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

    private FirebaseAuth auth;
    private DatabaseReference databaseRef;

    public TextMeshProUGUI networkState;

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
                Debug.LogError("Firebase 초기화 실패: " + task.Exception);
            }
        });

        autoLoginToggle.isOn = PlayerPrefs.GetInt("AutoLogin", 0) == 1;
        autoLoginToggle.onValueChanged.AddListener(delegate { OnToggleChanged(); });
    }

    private void Update()
    {
        networkState.text = PhotonNetwork.NetworkClientState.ToString();
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
            Debug.Log("자동 로그인 성공: " + user.Email);

            // Firebase 로그인 성공 후 Photon 서버 연결
            ConnectToPhotonServer();
        }
    }

    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("자동 로그인 상태 저장: " + (autoLoginToggle.isOn ? "켜짐" : "꺼짐"));
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
            Debug.LogError("회원가입 실패: " + e.Message);
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

            // Firebase 로그인 성공 후 Photon 서버 연결
            ConnectToPhotonServer();
        }
        catch (System.Exception e)
        {
            Debug.LogError("로그인 실패: " + e.Message);
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

                    // Photon에 닉네임 설정
                    PhotonNetwork.NickName = username;
                    Debug.Log("Photon 닉네임 설정: " + username);
                }
                else
                {
                    Debug.LogError("아이디 불러오기 실패: 데이터 없음");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("데이터베이스에서 유저 이름 불러오기 실패: " + e.Message);
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
            Debug.Log("앱 종료 시 로그아웃 처리됨.");
        }
    }

    // Photon 서버 연결 함수
    private void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Photon 서버에 연결 시도 중...");
        }
    }

    // Photon 서버 연결 성공 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 서버에 성공적으로 연결되었습니다.");
    }

    // Photon 서버 연결 실패 콜백
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("Photon 서버 연결 실패: " + cause.ToString());
    }
}
