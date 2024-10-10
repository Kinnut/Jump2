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
    // 회원가입 및 로그인 UI 요소들
    public TMP_InputField signUpEmailInputField;       // 회원가입 - 이메일 입력 필드
    public TMP_InputField signUpPasswordInputField;    // 회원가입 - 비밀번호 입력 필드
    public TMP_InputField signUpUsernameInputField;    // 회원가입 - 아이디 입력 필드

    public TMP_InputField loginEmailInputField;        // 로그인 - 이메일 입력 필드
    public TMP_InputField loginPasswordInputField;     // 로그인 - 비밀번호 입력 필드

    // 로그인 및 회원가입 창 패널
    public GameObject loginPanel;
    public GameObject signUpPanel;

    public TMP_Text userIDText;                        // 로그인 후 사용자 아이디를 표시할 Text
    public Toggle autoLoginToggle;                    // 자동 로그인 여부 토글 버튼
    public GameObject warningPanel;                   // 경고 메시지 패널
    public TextMeshProUGUI warningText;               // 경고 메시지 표시용 텍스트
    public TextMeshProUGUI networkStateText;          // 네트워크 상태 표시용 텍스트

    private FirebaseAuth auth;                        // Firebase 인증 객체
    private DatabaseReference databaseRef;            // Firebase 데이터베이스 참조 객체

    void Start()
    {
        // Firebase 초기화 및 로그인 상태 확인
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                databaseRef = FirebaseDatabase.DefaultInstance?.RootReference;

                CheckLoginStatus(); // 로그인 상태 확인 후 자동 로그인 시도
            }
            else
            {
                ShowWarning("Firebase 초기화 실패: " + task.Exception.Message);
            }
        });

        // 자동 로그인 여부 설정
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

    // 로그인 상태 확인 및 자동 로그인 처리
    private async void CheckLoginStatus()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 1 && auth.CurrentUser != null)
        {
            await AutoLogin();
        }
        else if (auth.CurrentUser != null)
        {
            await LoadUsernameFromDatabase(auth.CurrentUser.UserId);
            loginPanel.SetActive(false);  // 이미 로그인된 경우 로그인 패널 숨김
        }
        else
        {
            loginPanel.SetActive(true);   // 로그인이 안 된 경우 로그인 패널 표시
        }
    }

    // 자동 로그인 처리
    private async Task AutoLogin()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            await LoadUsernameFromDatabase(user.UserId);
            loginPanel.SetActive(false);  // 자동 로그인 성공 시 패널 숨김
        }
    }

    // 자동 로그인 토글 변경 시 처리
    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 회원가입 처리
    public async void Register()
    {
        string email = signUpEmailInputField.text;
        string password = signUpPasswordInputField.text;
        string username = signUpUsernameInputField.text;

        try
        {
            // Firebase에 회원가입 요청
            FirebaseUser newUser = (await auth.CreateUserWithEmailAndPasswordAsync(email, password)).User;
            await SaveUsernameToDatabase(newUser.UserId, username);  // 데이터베이스에 사용자 정보 저장

            // 회원가입 후 회원가입 창 닫기
            if (signUpPanel != null)
            {
                signUpPanel.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            // 회원가입 실패 시 경고 메시지 표시
            ShowWarning("회원가입 실패: " + e.Message);
        }
    }

    // 데이터베이스에 사용자 정보 저장
    private async Task SaveUsernameToDatabase(string userId, string username)
    {
        if (databaseRef != null)
        {
            await databaseRef.Child("users").Child(userId).Child("username").SetValueAsync(username);
        }
    }

    // 로그인 처리
    public async void Login()
    {
        string email = loginEmailInputField.text;
        string password = loginPasswordInputField.text;

        try
        {
            // Firebase 로그인 요청
            FirebaseUser loggedInUser = (await auth.SignInWithEmailAndPasswordAsync(email, password)).User;
            await LoadUsernameFromDatabase(loggedInUser.UserId);

            loginPanel.SetActive(false);  // 로그인 성공 시 로그인 패널 숨김

            PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            // 로그인 실패 시 경고 메시지 표시
            ShowWarning("로그인 실패: " + e.Message);
        }
    }

    // 데이터베이스에서 사용자 이름 불러오기
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
                    userIDText.text = "아이디: " + username;
                    PhotonNetwork.NickName = username;  // Photon에서 닉네임 설정
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

    // 로그아웃 처리
    public void Logout()
    {
        auth.SignOut();
        userIDText.text = "로그아웃 상태";

        PlayerPrefs.SetInt("AutoLogin", 0);
        PlayerPrefs.Save();

        loginPanel.SetActive(true);  // 로그아웃 후 로그인 패널 표시
    }

    // 앱 종료 시 자동 로그인이 꺼져 있으면 Firebase 로그아웃 처리
    void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 0 && auth != null)
        {
            auth.SignOut();
        }
    }

    // 포톤 서버 연결 함수
    private void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 경고 메시지 표시 함수
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);  // 경고 패널 활성화
    }
}
