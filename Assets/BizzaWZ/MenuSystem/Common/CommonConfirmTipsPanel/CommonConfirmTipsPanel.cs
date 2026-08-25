using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public partial class UIPageIds
{
    public static readonly PageId CommonConfirmTipsPanel = "CommonConfirmTipsPanel";
}

public class CommonConfirmTipsPanel : UIPageBase<CommonConfirmTipsPanel.Args>
{
    public struct Args
    {
        public bool isLanguage;
        public string des;
        public Action onFinish;
    }

    public TMP_Text desTxt;
    public BizzaButton confirmBtn;

    private Args _args;

    void Awake()
    {
        confirmBtn.onClick.AddListener(CloseSelf);
    }

    protected override void OnOpen(CommonConfirmTipsPanel.Args args)
    {
        desTxt.text = args.isLanguage ? LanguageUtils.GetText(args.des) : args.des;
        LogLogger.LogHttpInfo("接收" , "服务器传回描述" , args.des);
        _args = args;
    }

    protected override void OnClose()
    {
        _args.onFinish?.Invoke();
    }
}
