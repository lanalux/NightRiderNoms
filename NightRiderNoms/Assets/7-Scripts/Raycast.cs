using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
 
    [SerializeField] LayerMask layerMask;
    float interactionDistance = 2.5f; 

    void Update(){
        if(HUDControls.Instance.gameIsPaused){
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask)){
            GameObject targetObject = hit.transform.gameObject;

            // DENY ORDER
            DenyOrderTrigger denyTrigger = targetObject.GetComponent<DenyOrderTrigger>();
            if(Input.GetMouseButtonDown(0) && denyTrigger!=null){
                DeliveryControls.Instance.RemoveOrder(denyTrigger.uiPrefab);
            }

            // ACCEPT ORDER
            PickUpTrigger pickUpTrigger = targetObject.GetComponent<PickUpTrigger>();
            if(Input.GetMouseButtonDown(0) && pickUpTrigger!=null){
                DeliveryControls.Instance.SwitchToWaitingToPickUp(pickUpTrigger.prefab, pickUpTrigger.destinationNum);
            }

            // DRIVE TO PICKUP
            DestinationTrigger trigger = targetObject.GetComponent<DestinationTrigger>();
            if(Input.GetMouseButtonDown(0) && trigger!=null){
                DeliveryControls.Instance.SwitchToPickUp(trigger.prefab);
                NavigationControls.Instance.SetPickupDestination(trigger.destinationNum, trigger.prefab);
            }

            // DRIVE TO PICKUP
            StartDeliveryTrigger startDeliveryTrigger = targetObject.GetComponent<StartDeliveryTrigger>();
            if(Input.GetMouseButtonDown(0) && startDeliveryTrigger!=null){
                DeliveryControls.Instance.SwitchToDroppingOff(startDeliveryTrigger.prefab);
                NavigationControls.Instance.SetDropOffDestination(startDeliveryTrigger.destinationNum, startDeliveryTrigger.prefab);
            }
        }
    }
}
