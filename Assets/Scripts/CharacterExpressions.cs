using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EmotionBlendShape
{
    public string emotionName; // e.g., "smile", "sad", "angry", "surprise"
    [Tooltip("The names of the BlendShapes to modify for this emotion.")]
    public List<string> blendShapeNames;
    [Tooltip("Target values for the BlendShapes (0-100)")]
    public List<float> targetValues;
}

public class CharacterExpressions : MonoBehaviour
{
    [Header("Face Mesh")]
    public SkinnedMeshRenderer faceMesh;

    [Header("Emotions")]
    public List<EmotionBlendShape> emotions = new List<EmotionBlendShape>();
    public float transitionSpeed = 5f;

    private Dictionary<string, EmotionBlendShape> emotionDictionary;
    private Dictionary<int, float> currentBlendShapeWeights;
    private EmotionBlendShape activeEmotion;
    private Coroutine transitionCoroutine;

    void Start()
    {
        if (faceMesh == null)
        {
            Debug.LogWarning("CharacterExpressions: No face SkinnedMeshRenderer assigned.");
            return;
        }

        emotionDictionary = new Dictionary<string, EmotionBlendShape>();
        foreach (var emotion in emotions)
        {
            emotionDictionary[emotion.emotionName.ToLower()] = emotion;
        }

        currentBlendShapeWeights = new Dictionary<int, float>();
        for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
        {
            currentBlendShapeWeights[i] = faceMesh.GetBlendShapeWeight(i);
        }
    }

    public void SetEmotion(string emotionName)
    {
        if (faceMesh == null) return;

        string key = emotionName.ToLower().Trim();

        if (key == "neutral" || key == "none")
        {
            activeEmotion = null;
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionToNeutral());
            return;
        }

        if (emotionDictionary.ContainsKey(key))
        {
            activeEmotion = emotionDictionary[key];
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionToEmotion(activeEmotion));
            Debug.Log($"CharacterExpressions: Emotion set to {key}");
        }
        else
        {
            Debug.LogWarning($"CharacterExpressions: Emotion '{emotionName}' not found.");
        }
    }

    private IEnumerator TransitionToEmotion(EmotionBlendShape targetEmotion)
    {
        bool isTransitioning = true;

        // Find indices
        List<int> targetIndices = new List<int>();
        for (int i = 0; i < targetEmotion.blendShapeNames.Count; i++)
        {
            int index = faceMesh.sharedMesh.GetBlendShapeIndex(targetEmotion.blendShapeNames[i]);
            if (index != -1) targetIndices.Add(index);
            else targetIndices.Add(-1);
        }

        while (isTransitioning)
        {
            isTransitioning = false;

            // Fade out others
            for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
            {
                int listIndex = targetIndices.IndexOf(i);
                float targetWeight = 0f;

                if (listIndex != -1)
                {
                    targetWeight = targetEmotion.targetValues[listIndex];
                }

                float currentWeight = faceMesh.GetBlendShapeWeight(i);
                if (Mathf.Abs(currentWeight - targetWeight) > 0.1f)
                {
                    isTransitioning = true;
                    faceMesh.SetBlendShapeWeight(i, Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed));
                }
            }
            yield return null;
        }
    }

    private IEnumerator TransitionToNeutral()
    {
        bool isTransitioning = true;
        while (isTransitioning)
        {
            isTransitioning = false;
            for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
            {
                float currentWeight = faceMesh.GetBlendShapeWeight(i);
                if (currentWeight > 0.1f)
                {
                    isTransitioning = true;
                    faceMesh.SetBlendShapeWeight(i, Mathf.Lerp(currentWeight, 0f, Time.deltaTime * transitionSpeed));
                }
            }
            yield return null;
        }
    }
}
