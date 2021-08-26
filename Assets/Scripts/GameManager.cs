using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }
   [SerializeField] private GameObject player;

   private void Awake()
   {
      Application.targetFrameRate = 30;
      Instance = this;
   }

   public GameObject Player => player;
}
