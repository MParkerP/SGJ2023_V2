using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class Torch : NetworkBehaviour
{
     public NetworkVariable<bool> isBeingHeld = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); 
}
