#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class UIPageIds
{
    public static readonly PageId QFA = "FAQPanel";
}
 
public class FAQPanel : UIPageBase
{    
     [Header("按钮")]
    [SerializeField] private BizzaButton clickBtn;
    protected override void OnAwake()
    {
        
        clickBtn.onClick.AddListener(OnClickCloseBtn);
    }
 
 
    protected override void OnOpen()
    {

    }

    protected override void OnClose()
    {
        
    }

    public void OnClickCloseBtn()
    {
        UIModule.Instance.ClosePage(UIPageIds.QFA);
    }
}
  
#endif