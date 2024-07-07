using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewObjectiveData", menuName = "Objective Data")]
public class ObjectiveData : ScriptableObject
{
    [SerializeField] private string _objectiveDescription;
    public string ObjectiveDescription => _objectiveDescription;

    [SerializeField] private ObjectiveType _objectiveType;
    public ObjectiveType ObjectiveType => _objectiveType;
    
    [SerializeField] private int _completionCondition;
    public int CompletionCondition => _completionCondition;

    [SerializeField] private string _notificationText;
    public string NotificationText => _notificationText;

    [SerializeField] private UnityEvent _conditionEvent;
    public UnityEvent ConditionEvent => _conditionEvent;
}
