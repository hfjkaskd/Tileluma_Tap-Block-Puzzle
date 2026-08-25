#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Bizza.Unity.Android
{
     [Obfuz.ObfuzIgnore]
    public class JavaBridgeUtils
    {
        private static AndroidJavaObject _currentUnityActivity;

        public static AndroidJavaObject CurrentUnityActivity
        {
            get
            {
                if (_currentUnityActivity == null)
                {
                    AndroidJavaClass mainActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _currentUnityActivity = mainActivity.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return _currentUnityActivity;
            }
        }

        private static readonly object[] EmptyArgs = Array.Empty<object>();

        private static readonly Dictionary<string, AndroidJavaClass> CacheClass = new();

        /// <summary>
        /// 调用当前UnityPlayerActivity上的方法(无返回值)
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        [Conditional("UNITY_ANDROID")]
        public static void CallMainActivityMethod(string methodName, params object[] args)
        {
#if UNITY_EDITOR || TEST_NO_BRIDGE
            return;
#endif
            try
            {
                var activity = CurrentUnityActivity;
                activity?.Call(methodName, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        /// <summary>
        /// 调用静态方法(无返回值)
        /// </summary>
        /// <param name="className">java类名 需要包含package</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        [Conditional("UNITY_ANDROID")]
        public static void CallStaticMethod(string className, string methodName, params object[] args)
        {
#if UNITY_EDITOR || TEST_NO_BRIDGE
            return;
#endif
            try
            {
                using var androidJavaClass = new AndroidJavaClass(className);
                androidJavaClass.CallStatic(methodName, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public static T CallStaticMethod<T>(string className, string methodName, params object[] args)
        {
#if !UNITY_ANDROID || UNITY_EDITOR || TEST_NO_BRIDGE
            return default;
#else
            try
            {
                using var androidJavaClass = new AndroidJavaClass(className);
                return androidJavaClass.CallStatic<T>(methodName, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
#endif
        }

        [Conditional("UNITY_ANDROID")]
        public static void CallStaticMethod(string className, string methodName)
        {
            CallStaticMethod(className, methodName, EmptyArgs);
        }

        public static AndroidJavaClass GetCacheClass(string className)
        {
            try
            {
                if (!CacheClass.TryGetValue(className, out var javaClass))
                {
                    javaClass = new AndroidJavaClass(className);
                    CacheClass[className] = javaClass;
                }

                return javaClass;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }


        /// <summary>
        /// 调用java静态方法(无返回值) 会缓存java类
        /// </summary>
        /// <param name="className">java类名 需要包含package</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        [Conditional("UNITY_ANDROID")]
        public static void CallStaticMethodCacheClass(string className, string methodName, params object[] args)
        {
#if UNITY_EDITOR || TEST_NO_BRIDGE
            return;
#endif
            try
            {
                var javaClass = GetCacheClass(className);
                javaClass.CallStatic(methodName, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [Conditional("UNITY_ANDROID")]
        public static void CallStaticMethodCacheClass(string className, string methodName)
        {
            CallStaticMethodCacheClass(className, methodName, EmptyArgs);
        }

        public static T CallStaticMethodCacheClass<T>(string className, string methodName, params object[] args)
        {
#if !UNITY_ANDROID || UNITY_EDITOR || TEST_NO_BRIDGE
            return default;
#else
            try
            {
                var javaClass = GetCacheClass(className);
                return javaClass.CallStatic<T>(methodName, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
#endif
        }
    }
}
#endif