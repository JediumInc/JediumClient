using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syrinj;

public class BasicSceneInjector : MonoBehaviour
{

    public static BasicSceneInjector Instance;
    void Awake()
    {
        Instance = this;
        DependencyContainer.Instance.Reset();
        InjectScene();
    }

    void OnLevelWasLoaded()
    {
        InjectScene();
    }

    public void InjectScene()
    {
        var behaviours = GetAllBehavioursInScene();

        InjectBehaviours(behaviours);
    }

    private MonoBehaviour[] GetAllBehavioursInScene()
    {
        return GameObject.FindObjectsOfType<MonoBehaviour>();
    }

    private void InjectBehaviours(MonoBehaviour[] behaviours)
    {
        DependencyContainer.Instance.Inject(behaviours);
    }
}
