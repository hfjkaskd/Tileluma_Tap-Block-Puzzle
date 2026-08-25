using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bizza.Sdk
{
    public class ChannelShowOnEnable : MonoBehaviour
    {
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetLayerSelection))]
#endif
        public List<string> showChannel = new() { "editor" };

        private void OnEnable()
        {
            if (!showChannel.Contains(ChannelConfig.Instance.channel))
            {
                gameObject.SetActive(false);
            }
        }
        
#if UNITY_EDITOR
        private IEnumerable GetLayerSelection()
        {
            var array = Resources.LoadAll<ChannelConfig>(ChannelConfig.ResourcesPath);
            var valueDropList = new ValueDropdownList<string>();
            foreach (var t in array)
            {
                if (!showChannel.Contains(t.channel))
                {
                    valueDropList.Add(t.channel, t.channel);
                }
            }
            return valueDropList;
        }
#endif
    }
}