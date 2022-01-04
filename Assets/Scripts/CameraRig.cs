using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
   [SerializeField] private List<Transform> targets;
   [SerializeField] private Vector3 offset;
   [SerializeField] private float smoothTime = .5f;

   private Vector3 velocity;
   private GameObject knight;
   private GameObject mage;
   private bool bothExist = false;

   void Start() {
       DontDestroyOnLoad(gameObject);
   }


   void Update()
   {
       // Get references of mage and knight – we place this in update so that it can happen depending on spawn times.
       if (knight == null) knight = GameObject.FindGameObjectWithTag("Knight");
       if (mage == null) mage = GameObject.FindGameObjectWithTag("Mage");

       if (mage != null && knight != null) bothExist = true;
   }
   void LateUpdate()
   {
       if (targets.Count < 2 && bothExist) {
           targets.Add(knight.transform);
           targets.Add(mage.transform);
       }

       if (targets.Count > 0) MoveCamera();

   }

   void MoveCamera()
   {
       Vector3 centerPoint = GetCenterPoint();
       Vector3 newPosition = centerPoint + offset;
       transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
   }
   Vector3 GetCenterPoint()
   {
        if (targets.Count == 1) return targets[0].position;
        var bounds = new Bounds(targets[0].position, Vector3.zero);
       
        if (targets.Count > 1) 
        {
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }
        
        return bounds.center;
   }

}
