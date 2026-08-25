using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obfuz.ObfuzIgnore]
public partial class AccountModuleCfg
{
    public const string add_ecpm = "/bus_stop/master/add_income"; // 添加收益  可以点击查看 -- 
    public const string add_order = "/bus_stop/master/apply_order"; // 去提现 - 全部提现 可以点击查看 -- 
    public const string add_order_task = "/bus_stop/master/apply_task"; // 去提现 - 现金或奖卷 可以点击查看
    public const string base_list = "/bus_stop/master/base_data"; // 其它配置信息 - 可以点击查看
    public const string from = "/bus_stop/master/from"; // 用户归因 - 后台查看
    public const string get_ecpm_id = "/bus_stop/master/income_id"; // 获取收益ID - 可以点击查看
    public const string login = "/bus_stop/master/login"; // 用户登陆 - 可以点击查看
    public const string msg = "/bus_stop/master/line"; // 添加反馈 - 需要服务器查看
    public const string msg_list = "/bus_stop/master/line_list"; // 反馈列表 - 需要服务器查看
    public const string new_user = "/bus_stop/master/user_new"; // 新用户奖励-领取 - 可以点击查看
    public const string number = "/bus_stop/master/add_reach"; // 关卡，完成次数上报 - 可以点击查看
    public const string order_list = "/bus_stop/master/order_list"; // 订单列表 - 可以点击查看
    public const string order_name = "/bus_stop/master/order_user"; // 提现用户列表 - 可以点击查看 
    public const string plat_from = "/bus_stop/master/plat_list"; // 提现平台 - 可以点击查看
    public const string task_list = "/bus_stop/master/task_list"; // 每日任务-现金 - 可以点击查看
    public const string user_info = "/bus_stop/master/user_data"; // 用户信息 - 可以点击查看
    public const string user_name = "/ApiBalls/V1/UserName"; // 昵称添加
    public const string ad_info = "/bus_stop/masterLog/ad_logs"; // 广告上报 - 可以点击查看
    public const string app_info = "/bus_stop/masterLog/event_logs"; // 日志上报 - 可以点击查看


    #region 每日任务
    public const string one_Count = "bsm_one";
    public const string one_Ratio = "bsm_one_rate";
    public const string two_Count = "bsm_two";
    public const string two_Ratio = "bsm_two_rate";
    public const string three_Count = "bsm_three";
    public const string three_Ratio = "bsm_three_rate";
    #endregion
}
