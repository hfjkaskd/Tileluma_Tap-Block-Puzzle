using LitMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bizza.Sdk
{
    public class OverseaPlatform : Platform
    {
        public override E_PlatformType Type => E_PlatformType.Oversea;

        public OverseaPlatform(ChannelConfig channelConfig) : base(channelConfig)
        {
            InitAdAdapter(channelConfig);
            InitAttributeAdapter(channelConfig);
            InitAnalysisAdapter(channelConfig);
        }

        public void InitAdAdapter(ChannelConfig channelConfig)
        {
            if (AdSdk != null)
            {
                return;
            }

            AdSdk = new AggregationAdSdk();
            AdSdk.InitAdAdapter(channelConfig);
        }

        #region 归因

        public void InitAttributeAdapter(ChannelConfig channelConfig)
        {
            if (AttributeSdk != null)
            {
                return;
            }

            switch (channelConfig.attributeConfig)
            {
                case E_AttributeSdkType.Adjust:
#if BIZZA_ENABLE_ADJUST
                    AttributeSdk = new AdjustAttributionAdapter();
#endif
                    break;
                case E_AttributeSdkType.Tenjin:
#if BIZZA_ENABLE_TENJIN
                    AttributeSdk = new TenjinAttributionAdapter();
#endif
                    break;
                default:

                    break;
            }

            if (AttributeSdk != null)
            {
                AttributeSdk.Init(channelConfig);
            }
        }

        #endregion

        #region 打点
        public void InitAnalysisAdapter(ChannelConfig channelConfig)
        {
            if (AnalysisSdk != null)
            {
                return;
            }

            switch (channelConfig.analysisSdkType)
            {
                case E_AnalysisSdkType.ThinkingData:
#if BIZZA_ENABLE_SHUSHU
                    AnalysisSdk = new ThinkingDataAdapter();
#endif
                    break;
                default:

                    break;
            }

            if (AnalysisSdk != null)
            {
                AnalysisSdk.Init(channelConfig);
            }
        }
        #endregion

    }
}