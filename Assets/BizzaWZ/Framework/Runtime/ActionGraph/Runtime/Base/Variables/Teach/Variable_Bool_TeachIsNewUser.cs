using System;
using UnityEngine.Scripting;

 
[Preserve]
[GraphElementInfo(Category = "教学", Text = "教学是不是新人", SupportTypes = new Type[] { typeof(ActionGraphBase) })]
[Obfuz.ObfuzIgnore]
public class Variable_Bool_TeachIsNewUser : Variable_Bool
{
        public override object Clone()
        {
                return new Variable_Bool_TeachIsNewUser
                {

                };
        }

        public override bool GetValue(in ExecuteArgs executeArgs)
        {
                bool newUser = false;

#if !UNITY_EDITOR
                newUser = AccountModule.Instance.Os_Current_Uso.Os_Ncm;
#elif BIZZA_REAL_WITHDRAW
                if (AccountModule.Instance.Os_Current_Uso.Os_Ncm == true)
                {
                        return true;
                }
                newUser = Bizza.Sdk.ChannelConfig.Instance.real_CustomConfig.enterNewbieGuide;
#endif
                return newUser;
        }
}