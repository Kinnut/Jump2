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
    // ȸ������ �� �α��� UI ��ҵ�
    public TMP_InputField signUpEmailInputField;       // ȸ������ - �̸��� �Է� �ʵ�
    public TMP_InputField signUpPasswordInputField;    // ȸ������ - ��й�ȣ �Է� �ʵ�
    public TMP_InputField signUpUsernameInputField;    // ȸ������ - ���̵� �Է� �ʵ�

    public TMP_InputField loginEmailInputField;        // �α��� - �̸��� �Է� �ʵ�
    public TMP_InputField loginPasswordInputField;     // �α��� - ��й�ȣ �Է� �ʵ�

    // �α��� �� ȸ������ â �г�
    public GameObject loginPanel;
    public GameObject signUpPanel;

    public TMP_Text userIDText;                        // �α��� �� ����� ���̵� ǥ���� Text
    public Toggle autoLoginToggle;                    // �ڵ� �α��� ���� ��� ��ư
    public GameObject warningPanel;                   // ��� �޽��� �г�
    public TextMeshProUGUI warningText;               // ��� �޽��� ǥ�ÿ� �ؽ�Ʈ
    public TextMeshProUGUI networkStateText;          // ��Ʈ��ũ ���� ǥ�ÿ� �ؽ�Ʈ

    private FirebaseAuth auth;                        // Firebase ���� ��ü
    private DatabaseReference databaseRef;            // Firebase �����ͺ��̽� ���� ��ü

    void Start()
    {
        // Firebase �ʱ�ȭ �� �α��� ���� Ȯ��
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                databaseRef = FirebaseDatabase.DefaultInstance?.RootReference;

                CheckLoginStatus(); // �α��� ���� Ȯ�� �� �ڵ� �α��� �õ�
            }
            else
            {
                ShowWarning("Firebase �ʱ�ȭ ����: " + task.Exception.Message);
            }
        });

        // �ڵ� �α��� ���� ����
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

    // �α��� ���� Ȯ�� �� �ڵ� �α��� ó��
    private async void CheckLoginStatus()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 1 && auth.CurrentUser != null)
        {
            await AutoLogin();
        }
        else if (auth.CurrentUser != null)
        {
            await LoadUsernameFromDatabase(auth.CurrentUser.UserId);
            loginPanel.SetActive(false);  // �̹� �α��ε� ��� �α��� �г� ����
        }
        else
        {
            loginPanel.SetActive(true);   // �α����� �� �� ��� �α��� �г� ǥ��
        }
    }

    // �ڵ� �α��� ó��
    private async Task AutoLogin()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            await LoadUsernameFromDatabase(user.UserId);
            loginPanel.SetActive(false);  // �ڵ� �α��� ���� �� �г� ����
        }
    }

    // �ڵ� �α��� ��� ���� �� ó��
    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ȸ������ ó��
    public async void Register()
    {
        string email = signUpEmailInputField.text;
        string password = signUpPasswordInputField.text;
        string username = signUpUsernameInputField.text;

        try
        {
            // Firebase�� ȸ������ ��û
            FirebaseUser newUser = (await auth.CreateUserWithEmailAndPasswordAsync(email, password)).User;
            await SaveUsernameToDatabase(newUser.UserId, username);  // �����ͺ��̽��� ����� ���� ����

            // ȸ������ �� ȸ������ â �ݱ�
            if (signUpPanel != null)
            {
                signUpPanel.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            // ȸ������ ���� �� ��� �޽��� ǥ��
            ShowWarning("ȸ������ ����: " + e.Message);
        }
    }

    // �����ͺ��̽��� ����� ���� ����
    private async Task SaveUsernameToDatabase(string userId, string username)
    {
        if (databaseRef != null)
        {
            await databaseRef.Child("users").Child(userId).Child("username").SetValueAsync(username);
        }
    }

    // �α��� ó��
    public async void Login()
    {
        string email = loginEmailInputField.text;
        string password = loginPasswordInputField.text;

        try
        {
            // Firebase �α��� ��û
            FirebaseUser loggedInUser = (await auth.SignInWithEmailAndPasswordAsync(email, password)).User;
            await LoadUsernameFromDatabase(loggedInUser.UserId);

            loginPanel.SetActive(false);  // �α��� ���� �� �α��� �г� ����

            PlayerPrefs.SetInt("AutoLogin", autoLoginToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            // �α��� ���� �� ��� �޽��� ǥ��
            ShowWarning("�α��� ����: " + e.Message);
        }
    }

    // �����ͺ��̽����� ����� �̸� �ҷ�����
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
                    userIDText.text = "���̵�: " + username;
                    PhotonNetwork.NickName = username;  // Photon���� �г��� ����
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

    // �α׾ƿ� ó��
    public void Logout()
    {
        auth.SignOut();
        userIDText.text = "�α׾ƿ� ����";

        PlayerPrefs.SetInt("AutoLogin", 0);
        PlayerPrefs.Save();

        loginPanel.SetActive(true);  // �α׾ƿ� �� �α��� �г� ǥ��
    }

    // �� ���� �� �ڵ� �α����� ���� ������ Firebase �α׾ƿ� ó��
    void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("AutoLogin", 0) == 0 && auth != null)
        {
            auth.SignOut();
        }
    }

    // ���� ���� ���� �Լ�
    private void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ��� �޽��� ǥ�� �Լ�
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);  // ��� �г� Ȱ��ȭ
    }
}
