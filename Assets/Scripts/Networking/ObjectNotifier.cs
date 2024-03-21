using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class ObjectNotifier : NetworkBehaviour
{
    /// <summary>
    /// Event that gets called upon object spawning
    /// </summary>
    public static event Action<Transform> OnFirstObjectSpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            NetworkObject nob = base.LocalConnection.FirstObject;

            if (nob == base.NetworkObject)
            {
                OnFirstObjectSpawned?.Invoke(transform);
            }
        }
    }
}
