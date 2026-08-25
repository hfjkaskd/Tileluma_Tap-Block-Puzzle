using LitMotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Sdk
{
    public class DebugPlatform : Platform
    {
        public DebugPlatform(ChannelConfig channelConfig) : base(channelConfig)
        {
        }

        public override E_PlatformType Type => E_PlatformType.Editor;
    }
}