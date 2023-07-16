using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class Torch : NetworkBehaviour
{
    public NetworkVariable<bool> isBeingHeld = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private GameObject torchSpawnPosition;

    public override void OnNetworkSpawn()
    {
        torchSpawnPosition = GameObject.Find("TorchSpawnPosition");
        if (torchSpawnPosition != null) { transform.position = torchSpawnPosition.transform.position; }
    }
}

