using System;
using UnityEngine;
using LitMotion;

namespace Bizza.Sdk
{
    public interface ITimer
    {
        void TimeOut(float timeOut, Action callback);
        void Stop();
    }
}

public class LitMotionTimer : Bizza.Sdk.ITimer
{
    private MotionHandle handler;
    private Action callback;

    public LitMotionTimer()
    {
        handler.StorageId = -1;
    }

    void OnComplete()
    {
        handler.StorageId = -1;
        this.callback?.Invoke();
        this.callback = null;
    }

    public void TimeOut(float timeOut, Action callback)
    {
        if (handler.StorageId > -1)
        {
            return;
        }

        this.callback = callback;
        handler = LMotion.Create(0f, 1f, timeOut)
            .WithScheduler(MotionScheduler.InitializationIgnoreTimeScale)
            .WithOnComplete(OnComplete)
            .RunWithoutBinding();
    }

    public void Stop()
    {
        if (handler.StorageId > -1)
        {
            try
            {
                MotionHandleExtensions.Cancel(handler);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            handler.StorageId = -1;
            callback = null;
        }
    }
}

namespace Bizza.Sdk
{
    public abstract class RewardTimeOutAdapter : RewardAdAdapter
    {
        protected ITimer timer = new LitMotionTimer();
        public float maxWaitTime = 10;
        protected override void ShowRewardAdFinish()
        {
            Debug.Log("ShowRewardAdFinish");
            // onReward = null;
            // onShowFail = null;
            onShowFinish?.Invoke();
            timer.Stop();
            ProtectedDestroyRewardAd();
        }

        protected abstract void ProtectedShowRewardAds();
        protected abstract void ProtectedDestroyRewardAd();

        public override void ShowAds(Action onSuccess, Action onFailed, ShowAdArgs args)
        {
            LogLogger.LogAdInfo($"进入到MAX 的激励播放器中： ");
            onReward = onSuccess;
            onShowFail = onFailed;
#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
            var id = _cachedBatchId;
            if (args.fakeDollarNum > 0)
            {
                LogLogger.LogAdInfo($"入队钞票： {args.fakeDollarNum}");
                CurrencyBar.OnEnQueue(id, args.fakeDollarNum);
            }
#endif

            //请求展示广告 
            bool isShowSuccess = IsReady;
            LogLogger.LogAdInfo($"Max激励广告 {isShowSuccess}");
            if (!isShowSuccess)
            {
                timer.TimeOut(maxWaitTime, () => { LoadRewardAdFail(); });
            }
            else
            {
                ProtectedShowRewardAds();
            }
        }
    }

    public class DebugRewardAdAdapter : RewardTimeOutAdapter
    {
        public override double ECPM => 0;

        public override E_AdType adType => E_AdType.RewardAd;
        public override string AdUnitId => "DebugRewardAdAdapter";
        public override bool IsValid { get; }
        public override bool IsReady => true;

        public override void Init(string adUnitId)
        {

        }

        public override void Load()
        {
        }

        protected override void ProtectedShowRewardAds()
        {
            const float rate = 100;
            const float delay = 2f;
            bool success = UnityEngine.Random.Range(0, 100) < rate;
            LMotion.Create(0f, 1f, delay)
                .WithScheduler(MotionScheduler.InitializationIgnoreTimeScale)
                .WithOnComplete(success ? Success : Fail)
                .RunWithoutBinding();

            void Success()
            {
                OnReward();
                ShowRewardAdFinish();
            }

            void Fail()
            {
                ShowRewardAdFail();
                ShowRewardAdFinish();
            }
        }

        protected override void ProtectedDestroyRewardAd()
        {
            // not need
        }
    }
}