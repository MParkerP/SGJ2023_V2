using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;

public class UniversalClientNetworkTransform : ClientNetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
