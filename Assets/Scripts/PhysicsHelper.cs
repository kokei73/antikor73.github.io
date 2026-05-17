using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A helper script to automatically setup KawaiiPhysics components.
/// Uses reflection so the project compiles even if the KawaiiPhysics asset is not yet imported.
/// </summary>
public class PhysicsHelper : MonoBehaviour
{
    [Header("Bones References")]
    public Transform leftBreastBone;
    public Transform rightBreastBone;
    public Transform leftButtBone;
    public Transform rightButtBone;
    public Transform[] hairRoots;

    [Header("Settings")]
    public bool setupOnStart = true;
    public float setupDelay = 0.5f;

    private Type kawaiiPhysicsType;

    public void SetupAllPhysics()
    {
        // Try to find the KawaiiPhysics component type in loaded assemblies
        kawaiiPhysicsType = GetKawaiiPhysicsType();

        if (kawaiiPhysicsType == null)
        {
            Debug.LogWarning("PhysicsHelper: KawaiiPhysics script not found in project. Please import KawaiiPhysics.");
            return;
        }

        Debug.Log("PhysicsHelper: KawaiiPhysics found. Initializing...");

        SetupBreastPhysics();
        SetupButtPhysics();
        SetupHairPhysics();
    }

    public void SetupBreastPhysics()
    {
        if (leftBreastBone != null) AddKawaiiPhysicsToBone(leftBreastBone, 0.05f, 0.8f, 0.2f);
        if (rightBreastBone != null) AddKawaiiPhysicsToBone(rightBreastBone, 0.05f, 0.8f, 0.2f);
    }

    public void SetupButtPhysics()
    {
        if (leftButtBone != null) AddKawaiiPhysicsToBone(leftButtBone, 0.07f, 0.9f, 0.1f);
        if (rightButtBone != null) AddKawaiiPhysicsToBone(rightButtBone, 0.07f, 0.9f, 0.1f);
    }

    public void SetupHairPhysics()
    {
        if (hairRoots != null)
        {
            foreach (Transform hairRoot in hairRoots)
            {
                if (hairRoot != null)
                {
                    AddKawaiiPhysicsToBone(hairRoot, 0.02f, 0.5f, 0.1f);
                }
            }
        }
    }

    /// <summary>
    /// Adds the KawaiiPhysics component via Reflection to prevent compiler errors
    /// if the asset isn't imported yet.
    /// </summary>
    private void AddKawaiiPhysicsToBone(Transform bone, float radius, float damping, float stiffness)
    {
        if (kawaiiPhysicsType == null) return;

        // Check if it already has the component
        Component existingComponent = bone.gameObject.GetComponent(kawaiiPhysicsType);
        if (existingComponent != null) return; // Already setup

        Component kawaiiComp = bone.gameObject.AddComponent(kawaiiPhysicsType);

        // Attempt to set basic fields if they match KawaiiPhysics structure
        SetFieldValue(kawaiiComp, "rootBone", bone);
        SetFieldValue(kawaiiComp, "radius", radius);
        SetFieldValue(kawaiiComp, "damping", damping);
        SetFieldValue(kawaiiComp, "stiffness", stiffness);

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
