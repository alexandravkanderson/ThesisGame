using UnityEngine;

namespace Prototype_v_0_Scripts
{
    public class Characters : MonoBehaviour
    {
        [SerializeField] private string scriptableObjectName;
        [SerializeField] private CharacterAttributes characterAttributes;
    
        // For attributes
        public float HP;      // Health point
        public float AD;      // Attach damage
        public float SP;      // Attach speed
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            // Set up the scriptable object
            characterAttributes = Resources.Load<CharacterAttributes>("Prototype_v_0_Resources/ScriptableObject/" + scriptableObjectName);

            HP = characterAttributes.healthPoint;
            AD = characterAttributes.attackDamage;
            SP = characterAttributes.attackSpeed;
        }
    }
}
