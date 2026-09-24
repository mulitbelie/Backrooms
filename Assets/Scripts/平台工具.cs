using UnityEngine;

public static class 平台工具
{
    public enum 平台 { 自动, PC, 移动端 }

    public static 平台 强制平台 = 平台.自动;

    static bool 已打印一次 = false;

    public static bool 是移动端
    {
        get
        {
            if (强制平台 == 平台.PC) return false;
            if (强制平台 == 平台.移动端) return true;

            if (!已打印一次)
            {
               已打印一次 = true;
                Debug.Log($"[平台检测] platform={Application.platform} " +
                          $"touchSupported={Input.touchSupported} " +
                          $"deviceType={SystemInfo.deviceType} " +
                          $"touchCount={Input.touchCount} " +
                          $"isEditor={Application.isEditor} " +
                          $"screen={Screen.width}x{Screen.height}");
            }

            if (Application.platform == RuntimePlatform.Android) return true;
            if (Application.platform == RuntimePlatform.IPhonePlayer) return true;
            if (Application.platform == RuntimePlatform.WSAPlayerARM) return true;

            if (SystemInfo.deviceType == DeviceType.Handheld) return true;

            if (Input.touchSupported && Application.platform != RuntimePlatform.WindowsPlayer
                                    && Application.platform != RuntimePlatform.OSXPlayer
                                    && Application.platform != RuntimePlatform.LinuxPlayer)
                return true;

            if (Application.isEditor && Screen.height > Screen.width) return true;

            return false;
        }
    }

    public static string 按键提示 => 是移动端 ? "tap" : "press E";
}