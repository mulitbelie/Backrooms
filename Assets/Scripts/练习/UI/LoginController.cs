using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoginController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text MessageText;

    public void OnLoginButtonClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        if(username == "admin" && password == "123")
        {
            Debug.Log("登录成功");
            // 登录成功后，跳转到主界面
            SceneManager.LoadScene("Level_01");
        }
        else
        {
            MessageText.text = "Username or password incorrect";
            MessageText.enabled = true;
            StartCoroutine(HideMessage());
        }
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(1);
        MessageText.enabled = false;
    }
}
