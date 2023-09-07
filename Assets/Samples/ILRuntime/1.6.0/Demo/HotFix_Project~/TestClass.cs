using UnityEngine;
using UnityEngine.UI;


namespace HotFix_Project
{
    public class TestClass
    {
        public static void StaticFunTest()
        {
            Debug.Log("TestClass");
            GameObject Canvas = GameObject.Find("Canvas");
            GameObject LoginPanel = GameObject.Instantiate(Resources.Load<GameObject>("LoginPanel"));
            LoginPanel.transform.SetParent(Canvas.transform);
            LoginPanel.transform.localScale = Vector3.one;
            LoginPanel.transform.localPosition = Vector3.zero;

            GameObject Button_GuestLogin = GameObject.Find("Button_GuestLogin");
            Button button = Button_GuestLogin.GetComponent<Button>();
            button.onClick.AddListener(OnClick_Button_GuestLogin);
        }

        public static void OnClick_Button_GuestLogin()
        {
            Debug.Log("OnClick_Button_GuestLogin");
        }
    }
}