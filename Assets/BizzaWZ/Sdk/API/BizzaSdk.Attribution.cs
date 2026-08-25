using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using UnityEngine;
#if BIZZA_ENABLE_ADJUST
using AdjustSdk;
#endif

namespace BizzaSdk
{
    public static class Attribution
    {
        public static void AttributeStart(string eventName)
        {
            if (Platform.Instance != null && Platform.Instance.AttributeSdk != null)
            {
                Platform.Instance.AttributeSdk.AttributeStart(eventName);
            }
        }

        public static string AD_REVENUE_APPLOVIN_MAX = "applovin_max_sdk";
        public static void ReportAdShowForAdjust(double revenue, string currency, string AdRevenueNetwork, string AdRevenueUnit, string AdRevenuePlacement)
        {
#if BIZZA_ENABLE_ADJUST
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AD_REVENUE_APPLOVIN_MAX);
            adjustAdRevenue.SetRevenue(revenue, currency);
            adjustAdRevenue.AdRevenueNetwork = AdRevenueNetwork;
            adjustAdRevenue.AdRevenueUnit = AdRevenueUnit;
            adjustAdRevenue.AdRevenuePlacement = AdRevenuePlacement;

            RealWzAnalytics.AddDict(RealWzAnalytics.eventType_Revenue_AdjustAttiribute, adjustAdRevenue);
            RealWzAnalytics.TrackBiz_Revenue();
            LogLogger.LogAttribute($"Adjust收益上报 revenue:{revenue}; currency:{currency}; AdRevenueNetwork:{AdRevenueNetwork}; AdRevenueUnit:{AdRevenueUnit}; AdRevenuePlacement:{AdRevenuePlacement}");
            Adjust.TrackAdRevenue(adjustAdRevenue);
#endif
        }
    }
}