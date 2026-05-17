using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helper to dynamically attach and configure KawaiiPhysics components without hard compile-time dependencies.
/// </summary>
public class PhysicsHelper : MonoBehaviour
{
    [System.Serializable]
    public class PhysicsParams
    {
        [Tooltip("Maximum distance the bone can move from its root.")]
        public float moveRadius = 0.05f;
        [Tooltip("How stiff the spring is. Higher = faster return to origin.")]
        public float stiffness = 0.8f;
        [Tooltip("Strength of gravity pulling down.")]
        public float gravityPower = 0.2f;
        [Tooltip("Air resistance. Higher = slower, smoother movement.")]
        public float dragForce = 0.1f;
    }

    [Header("Physics Profiles")]
    public PhysicsParams breastProfile = new PhysicsParams { moveRadius = 0.05f, stiffness = 0.8f, gravityPower = 0.2f, dragForce = 0.1f };
    public PhysicsParams buttProfile = new PhysicsParams { moveRadius = 0.07f, stiffness = 0.9f, gravityPower = 0.2f, dragForce = 0.1f };
    public PhysicsParams hairProfile = new PhysicsParams { moveRadius = 0.1f, stiffness = 0.5f, gravityPower = 0.1f, dragForce = 0.2f };

    [Header("Settings")]
    public bool setupOnStart = true;
    public float setupDelay = 0.5f;

    private Type kawaiiPhysicsType;

    public void ApplyPhysicsToAll(Transform breastL, Transform breastR, Transform buttL, Transform buttR, Transform hair)
    {
        // Resolve type via reflection to avoid compile errors if asset is missing.
        kawaiiPhysicsType = GetKawaiiPhysicsType();

        if (kawaiiPhysicsType == null)
        {
            Debug.LogWarning("PhysicsHelper: KawaiiPhysics script not found in project. Please import KawaiiPhysics.");
            return;
        }

        Debug.Log("PhysicsHelper: Applying Physics Profiles...");

        // Setup Breasts
        if (breastL != null) AddKawaiiPhysicsToBone(breastL, breastProfile);
        if (breastR != null) AddKawaiiPhysicsToBone(breastR, breastProfile);

        // Setup Butts
        if (buttL != null) AddKawaiiPhysicsToBone(buttL, buttProfile);
        if (buttR != null) AddKawaiiPhysicsToBone(buttR, buttProfile);

        // Setup Hair
        if (hair != null) AddKawaiiPhysicsToBone(hair, hairProfile);
    }

    private void AddKawaiiPhysicsToBone(Transform bone, PhysicsParams profile)
    {
        if (kawaiiPhysicsType == null || bone == null) return;

        // Check if it already has the component
        Component existingComponent = bone.gameObject.GetComponent(kawaiiPhysicsType);
        if (existingComponent != null) return;

        Component kawaiiComp = bone.gameObject.AddComponent(kawaiiPhysicsType);

        // Attempt to set fields if they match KawaiiPhysics structure
        SetFieldValue(kawaiiComp, "rootBone", bone);
        SetFieldValue(kawaiiComp, "radius", profile.moveRadius); // Often maps to radius or limit
        SetFieldValue(kawaiiComp, "stiffness", profile.stiffness);

        // Setup Gravity Vector (down)
        Vector3 gravity = new Vector3(0, -profile.gravityPower, 0);
        SetFieldValue(kawaiiComp, "gravity", gravity);

        SetFieldValue(kawaiiComp, "damping", profile.dragForce); // Damping is commonly used instead of drag in springs

        Debug.Log($"PhysicsHelper: Added KawaiiPhysics to {bone.name}");
    }

    private void SetFieldValue(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    private Type GetKawaiiPhysicsType()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType("KawaiiPhysics.KawaiiPhysics");
            if (type != null) return type;
        }
        return null;
    }
}
