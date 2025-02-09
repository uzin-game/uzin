using Unity.Netcode;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerScaleFix : NetworkBehaviour
    {
        public Vector3 correctScale = new Vector3(10f, 10f, 10f);

        public override void OnNetworkSpawn()
        {
            transform.localScale = correctScale;
        }
    }
}

