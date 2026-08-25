package com.bangsawan.oceanshine;

import android.content.Context;
import android.content.res.Resources;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.text.TextUtils;
import android.os.Build;
import android.provider.Settings;
import android.telephony.TelephonyManager;
import com.bangsawan.BangsawanLog;
import com.google.android.gms.ads.identifier.AdvertisingIdClient;
import com.bangsawan.unity.UnityHelper;
import com.unity3d.player.UnityPlayer;
import java.io.File;
import java.util.Locale;
import java.util.Random;

public final class DeviceUtils {


    private static Context getContext() {
        return UnityPlayer.currentActivity.getApplicationContext();
    }

    private static final String TAG_Android = "[测试日志][安卓ID]";

    private static final boolean ENABLED = true;

    /**
     * 安卓 ANDROID_ID
     */
    public static String GetAndroidId() {
        try {
            // ✅ 真机模式
            Context ctx = UnityPlayer.currentActivity;
            if (ctx == null) return "";
    
            String androidId = Settings.Secure.getString(
                    ctx.getContentResolver(),
                    Settings.Secure.ANDROID_ID
            );
            return androidId != null ? androidId : "";
        } catch (Exception e) {
            BangsawanLog.e(TAG_Android, "获取安卓ID出现错误", e, ENABLED);
            return "";
        }
    }

    /**
     * 手机品牌 Build.BRAND
     */
    public static String GetBrand() {

        return Build.BRAND != null ? Build.BRAND : "";
    }

    /**
     * 设备型号 Build.MODEL
     */
    public static String GetModel() {
        return Build.MODEL != null ? Build.MODEL : "";
    }

    /**
     * 系统版本 Build.VERSION.RELEASE
     */
    public static String GetOSVersion() {
        return Build.VERSION.RELEASE != null ? Build.VERSION.RELEASE : "";
    }

    private static final String TAG_GAID = "[测试日志][GAID]";
    /**
     * Google Advertising ID (GAID)
     * ⚠️ 必须在子线程调用
     * ⚠️ 需要用户同意 & Google Play Services
     */
    public static void GetGAID() {
        final Context activity = UnityPlayer.currentActivity;

        new Thread(() -> {
            String result = "";

            try {
                
                    if (activity != null) {
                        AdvertisingIdClient.Info info =
                                AdvertisingIdClient.getAdvertisingIdInfo(activity.getApplicationContext());
                        result = (info != null) ? info.getId() : "";
                    } else {
                        result = "";
                    }
                
            } catch (Exception e) {
                BangsawanLog.e(TAG_GAID, "获取GAID失败", e, ENABLED);
                result = "";
            }

            if (UnityPlayer.currentActivity != null) {
                final String finalResult = result;
                UnityPlayer.currentActivity.runOnUiThread(() ->
                        UnityHelper.SendUnityMessageForGoogleId(finalResult, null)
                );
            }
        }).start();
    }

    /**
     * 网络类型
     * WIFI / MOBILE / NONE
     */
    public static String GetNetworkType() {
        try {
            ConnectivityManager cm =
                    (ConnectivityManager) getContext().getSystemService(Context.CONNECTIVITY_SERVICE);
            if (cm == null) return "NONE";

            NetworkInfo info = cm.getActiveNetworkInfo();
            if (info == null || !info.isConnected()) {
                return "NONE";
            }

            if (info.getType() == ConnectivityManager.TYPE_WIFI) {
                return "WIFI";
            } else if (info.getType() == ConnectivityManager.TYPE_MOBILE) {
                return "MOBILE";
            }
        } catch (Exception ignored) {
        }

        return "UNKNOWN";
    }

    /**
     * 是否 Root
     * 1 = 是
     * 0 = 否
     */
    public static int IsRooted() {
        String[] paths = {
                "/system/bin/su",
                "/system/xbin/su",
                "/sbin/su",
                "/system/app/Superuser.apk"
        };

        for (String path : paths) {
            if (new File(path).exists()) {
                return 1;
            }
        }
        return 0;
    }

    /**
     * 你这个方法名叫 RootCallback，但里面逻辑是判断 SIM 状态（我不改逻辑，只保证能跑）
     */
    public static void GetRootCallback() {
        Context context = UnityPlayer.currentActivity;

        new Thread(() -> {
            int result = 0;

            try {
                if (context != null) {
                    TelephonyManager tm =
                            (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);

                    if (tm != null) {
                        int simState = tm.getSimState();
                        result = (simState == TelephonyManager.SIM_STATE_READY) ? 1 : 0;
                    }
                }
            } catch (Exception e) {
                BangsawanLog.e(TAG, "GetRootCallback exception", e, ENABLED);
                result = 0;
            }

            if (UnityPlayer.currentActivity != null) {
                final int finalResult = result;
                UnityPlayer.currentActivity.runOnUiThread(() -> {
                    UnityHelper.SendUnityMessageForRoot(finalResult, null);
                });
            }
        }).start();
    }

    /**
     * 是否有 SIM 卡
     * 1 = 有
     * 0 = 无
     */
    public static int HasSimCard() {
        try {
            TelephonyManager tm =
                    (TelephonyManager) getContext().getSystemService(Context.TELEPHONY_SERVICE);
            if (tm == null) return 0;
            return tm.getSimState() == TelephonyManager.SIM_STATE_READY ? 1 : 0;
        } catch (Exception e) {
            return 0;
        }
    }

    public static void GetHasSimCardMethod() {
        Context context = UnityPlayer.currentActivity;

        new Thread(() -> {
            int result = 0;

            try {
                if (context != null) {
                    TelephonyManager tm =
                            (TelephonyManager) getContext().getSystemService(Context.TELEPHONY_SERVICE);

                    if (tm == null) {
                        result = 0;
                    } else {
                        result = (tm.getSimState() == TelephonyManager.SIM_STATE_READY) ? 1 : 0;
                    }
                }
            } catch (Exception e) {
                BangsawanLog.e(TAG, "GetHasSimCardMethod exception", e, ENABLED);
                result = 0;
            }

            if (UnityPlayer.currentActivity != null) {
                final int finalResult = result;
                UnityPlayer.currentActivity.runOnUiThread(() -> {
                    UnityHelper.SendUnityMessageForSimCard(finalResult, null);
                });
            }
        }).start();
    }

    private static final String TAG = "[流程日志]";
    private static final String TAG_Country = "[测试日志][国家选择]";
    
    /**
     * 国家码（例如 CN / US BR ID） -----------------------------------------------------
     */
    public static String GetCountryCode(String unityCountry) {
        String userCountry = "";
        BangsawanLog.d(TAG_Country, "Unity传入的国家码 = " + safeShow(unityCountry), ENABLED);

        try {
            // 1) 优先使用 Unity 传入的国家码
            if (!TextUtils.isEmpty(unityCountry)) {
                userCountry = unityCountry;
                BangsawanLog.d(TAG_Country, "[优先使用] 使用 Unity 传入的国家码 = " + safeShow(userCountry), ENABLED);
            } else {
                // 2) Unity 没传时，再尝试读取 SIM 卡国家码
                Context ctx = null;
                try {
                    ctx = UnityPlayer.currentActivity;
                } catch (Throwable t) {
                    BangsawanLog.e(TAG_Country, "[上下文] UnityPlayer 未初始化，获取 currentActivity 失败: " + t, ENABLED);
                }

                BangsawanLog.d(TAG_Country, "[上下文] 当前 ctx = " + (ctx == null ? "null" : ctx.getClass().getName()), ENABLED);

                TelephonyManager tm = null;
                if (ctx != null) {
                    try {
                        Object svc = ctx.getSystemService(Context.TELEPHONY_SERVICE);
                        BangsawanLog.d(TAG_Country, "[电话服务] getSystemService 返回 = " + (svc == null ? "null" : svc.getClass().getName()), ENABLED);
                        if (svc instanceof TelephonyManager) {
                            tm = (TelephonyManager) svc;
                        }
                    } catch (Throwable t) {
                        BangsawanLog.e(TAG_Country, "[电话服务] 获取 TelephonyManager 时发生异常: " + t, ENABLED);
                    }
                } else {
                    BangsawanLog.w(TAG_Country, "[电话服务] ctx 为空，跳过获取 TelephonyManager", ENABLED);
                }

                BangsawanLog.d(TAG_Country, "[电话服务] TelephonyManager 状态 = " + (tm == null ? "null" : "可用"), ENABLED);

                String simIso = "";
                if (tm != null) {
                    try {
                        simIso = tm.getSimCountryIso();
                    } catch (Throwable t) {
                        BangsawanLog.e(TAG_Country, "[SIM国家码] 调用 getSimCountryIso 发生异常: " + t, ENABLED);
                    }
                }

                BangsawanLog.d(TAG_Country, "[SIM国家码] 原始结果 = " + safeShow(simIso), ENABLED);

                if (!TextUtils.isEmpty(simIso)) {
                    userCountry = simIso;
                    BangsawanLog.d(TAG_Country, "[选中国家] 使用 SIM 卡国家码 = " + safeShow(userCountry), ENABLED);
                } else {
                    userCountry = "";
                    BangsawanLog.w(TAG_Country, "[选中国家] 未获取到 SIM 卡国家码，返回空字符串", ENABLED);
                }
            }
        } catch (Exception e) {
            BangsawanLog.e(TAG_Country, "[异常] GetCountryCode 执行失败: " + e, ENABLED);
            userCountry = "";
        }

        // 规范化
        if (!TextUtils.isEmpty(userCountry)) {
            String before = userCountry;
            userCountry = userCountry.trim();
            BangsawanLog.d(TAG_Country, "[规范化] 去除首尾空格: \"" + before + "\" -> \"" + userCountry + "\"", ENABLED);

            String upper = userCountry.toUpperCase(Locale.US);
            BangsawanLog.d(TAG_Country, "[规范化] 转为大写: \"" + userCountry + "\" -> \"" + upper + "\"", ENABLED);
            userCountry = upper;
        }

        // 屏蔽 CN
        if ("CN".equalsIgnoreCase(userCountry)) {
            BangsawanLog.w(TAG_Country, "[国家拦截] 检测到国家码为 CN，改为空字符串", ENABLED);
            userCountry = "";
        }

        BangsawanLog.d(TAG_Country, "================ 获取国家码结束 ================", ENABLED);
        BangsawanLog.d(TAG_Country, "最终选中的国家码 = " + safeShow(userCountry), ENABLED);
        return userCountry;
    }
    private static String safeShow(String s) {
        if (s == null) return "null";
        if (s.length() == 0) return "\"\"(empty)";
        return "\"" + s + "\"";
    }


    /**
     * 渠道号
     */
    public static String GetChannel() {
        // TODO: 替换为你们真实渠道来源
        return "default";
    }


}
