using Unity.Netcode;

namespace PlayerScripts
{
    public class Death : NetworkBehaviour
    {
        public HealthNetwork health;

        void Start()
        {
            health = GetComponent<HealthNetwork>();
        }
        
        
    }
}