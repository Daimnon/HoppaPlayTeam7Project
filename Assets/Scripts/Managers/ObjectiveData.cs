using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectiveData", menuName = "Objective Data", order = 51)]
public class ObjectiveData : ScriptableObject
{
    public string objectiveDescription;
    public ObjectiveType objectiveType;
    public int completionCondition;
    public string notificationText;
}
