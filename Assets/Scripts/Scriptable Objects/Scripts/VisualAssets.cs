using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualAssets", menuName = "Scriptable Objects/Visual/VisualAssets")]
public class VisualAssets : ScriptableObject
{
    [field: SerializeField] public Material NormalMaterial { get; private set; }
    [field: SerializeField] public Material StealthMaterial { get; private set; }
}
