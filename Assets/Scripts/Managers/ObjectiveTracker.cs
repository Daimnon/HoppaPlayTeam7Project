using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    public LevelManager levelManager;

    private void Start()
    {
        foreach (var objective in levelManager.Objectives)
        {
            if (objective.ConditionEvent != null)
            {
                objective.ConditionEvent.AddListener(() => levelManager.UpdateObjective(objective.ObjectiveType));
            }
        }
    }
}
