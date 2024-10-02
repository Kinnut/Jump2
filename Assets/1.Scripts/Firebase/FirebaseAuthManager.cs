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
    public TMP_InputField signUpEmailInputField;       // ȸ������ - �̸��� �Է� �ʵ�
    public TMP_InputField signUpPasswordInputField;    // ȸ������ - ��й�ȣ �Է� �ʵ�
    public TMP_InputField signUpUsernameInputField;    // ȸ������ - ���̵� �Է� �ʵ�

    public TMP_InputField loginEmailInputField;        // �α��� - �̸��� �Է� �ʵ�
    public TMP_InputField loginPasswordInputField;     // �α��� - ��й�ȣ �Է� �ʵ�

    public GameObject loginPanel;                     // �α��� â �г�
    public GameObject signUpPanel;                    // ȸ������ â �г�

    public TMP_Text userIDText;                        // �α��� �� ����� ���̵� ǥ���� Text

    public Toggle autoLoginToggle;                    // �ڵ� �α��� ��� ��ư

    public GameObject warningPanel;                   // ��� �޽��� �г�
    public TextMeshProUGUI warningText;               // ��� �޽����� ǥ���� Text

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
                ShowWarning("Firebase �ʱ�ȭ ����: " + task.Exception.Message);
            }
        });

        autoLoginToggle.isOn = PlayerPrefs.GetInt("AutoLogin", 0) == 1;
        autoLoginToggle.onValueChanged.AddListener(delegate { OnToggleChanged(); });

        // ���� ���� ���� �õ�
        ConnectToPhotonServer();
    }

    private void Update()
    {
        // ��Ʈ��ũ ���¸� ǥ��
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
            ShowWarning("ȸ������ ����: " + e.Message);
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
            ShowWarning("�α��� ����: " + e.Message);
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
                        userIDText.text = "���̵�: " + username;
                    }
                    PhotonNetwork.NickName = username;
                }
                else
                {
                    ShowWarning("���̵� �ҷ����� ����: ������ ����");
                }
            }
            catch (System.Exception e)
            {
                ShowWarning("�����ͺ��̽����� ���� �̸� �ҷ����� ����: " + e.Message);
            }
        }
    }

    public void Logout()
    {
        auth.SignOut();

        if (userIDText != null)
        {
            userIDText.text = "�α׾ƿ� ����";
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
            // �ڵ� �α����� ���� ������ �� ���� �� Firebase �α׾ƿ� ó��
            auth.SignOut();
        }
    }

    // ���� ���� ���� �Լ� (��� �гο� ������� ����)
    private void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ��� �޽��� ǥ�� �Լ� (Firebase ���� ������ ǥ��)
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);
    }
}
