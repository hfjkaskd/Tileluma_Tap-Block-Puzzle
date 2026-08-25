using System.Collections.Generic;
using System.Linq;
using SRDebugger;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

public class SRShortcutHandler : MonoBehaviour
{
    public void Init()
    {

    }

    private List<OptionDefinition> _matches = new();

    void Update()
    {
        var mgr = SRServiceManager.GetService<IDebugService>();
        if (mgr != null)
        {
            if (mgr.IsDebugPanelVisible)
            {
                return;
            }
        }

        _matches.Clear();
        var options = Service.Options.Options;
        foreach (var v in options)
        {
            if (v.keycodes == null || v.keycodes.Length == 0) continue;
            var match = true;
            var anyKeyDown = false;
            foreach (var key in v.keycodes)
            {
                if (!Input.GetKey(key))
                {
                    match = false;
                }

                if (Input.GetKeyDown(key)) anyKeyDown = true;
            }

            if (match && anyKeyDown)
            {
                _matches.Add(v);
            }
        }

        if (_matches.Count > 0)
        {
            _matches = _matches.OrderByDescending(x => x.keycodes.Length).ToList();
            _matches[0].Method.Invoke(null);
        }
    }
}