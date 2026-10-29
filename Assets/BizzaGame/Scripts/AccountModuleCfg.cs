using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obfuz.ObfuzIgnore]
public partial class AccountModuleCfg
{
    public const string add_ecpm = "/Box/Sort/AddEcpm"; // 添加收益  可以点击查看 --
    public const string add_order = "/Box/Sort/AddOrder"; // 去提现 - 全部提现 可以点击查看 --
    public const string add_order_task = "/Box/Sort/AddOrderTask"; // 去提现 - 现金或奖卷 可以点击查看
    public const string base_list = "/Box/Sort/DataBase"; // 其它配置信息 - 可以点击查看
    public const string from = "/Box/Sort/From"; // 用户归因 - 后台查看
    public const string get_ecpm_id = "/Box/Sort/EcpmId"; // 获取收益ID - 可以点击查看
    public const string login = "/Box/Sort/Login"; // 用户登陆 - 可以点击查看
    public const string msg = "/Box/Sort/Talk"; // 添加反馈 - 需要服务器查看
    public const string msg_list = "/Box/Sort/TalkList"; // 反馈列表 - 需要服务器查看
    public const string new_user = "/Box/Sort/NewUser"; // 新用户奖励-领取 - 可以点击查看
    public const string number = "/Box/Sort/AddReach"; // 关卡，完成次数上报 - 可以点击查看
    public const string order_list = "/Box/Sort/OrderList"; // 订单列表 - 可以点击查看
    public const string order_name = "/Box/Sort/OrderName"; // 提现用户列表 - 可以点击查看
    public const string plat_from = "/Box/Sort/PlatForm"; // 提现平台 - 可以点击查看
    public const string task_list = "/Box/Sort/Task"; // 每日任务-现金 - 可以点击查看
    public const string user_info = "/Box/Sort/UserData"; // 用户信息 - 可以点击查看
    public const string user_name = "/Box/Sort/UserName"; // 昵称添加
    public const string ad_info = "/Box/SortLog/AdData"; // 广告上报 - 可以点击查看
    public const string app_info = "/Box/SortLog/AppEvent"; // 日志上报 - 可以点击查看


    #region 每日任务
    public const string one_Count = "ph_one";
    public const string one_Ratio = "ph_one_rate";
    public const string two_Count = "ph_two";
    public const string two_Ratio = "ph_two_rate";
    public const string three_Count = "ph_three";
    public const string three_Ratio = "ph_three_rate";
    #endregion
}
