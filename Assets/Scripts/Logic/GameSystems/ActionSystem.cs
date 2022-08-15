using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public static ActionSystem Instance { get; private set; }
   [SerializeField] private CharacterController character;

}
