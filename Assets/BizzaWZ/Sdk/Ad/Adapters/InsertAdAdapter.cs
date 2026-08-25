using Bizza.Sdk;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Sdk
{
     
    public abstract class InsertAdAdapter : 
#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
        HttpAdAdapterBase
#else
        VideoAdAdapterBase
#endif
    {
        protected Action onSuccess = null;
        protected Action onFail = null;
    }
    
     
    public class DebugInsertAdAdapter : InsertAdAdapter
    {
        public bool opening = false;
        public override double ECPM => 0;
        public override E_AdType adType => E_AdType.InsertAd;
        public override string AdUnitId => "DEBUG";
        public override bool IsValid { get; }
        public override bool IsReady { get; }
        public override void Init(string adUnitId)
        {

        }

        public override void Load()
        {
        }

        public DebugInsertAdAdapter()
        {
        }

        public override void ShowAds(Action onSuccess, Action onFailed, ShowAdArgs args)
        {
            opening = true;
        }
    }
}

