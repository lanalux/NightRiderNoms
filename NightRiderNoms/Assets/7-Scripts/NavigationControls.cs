using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum CarState {Idle, DrivingToPickup, DrivingToDropOff};

public class NavigationControls : MonoBehaviour {

    public static NavigationControls Instance {get;set;}
    [SerializeField] NavMeshAgent agent;
    [SerializeField] List<Transform> locationSpots = new List<Transform>();
    [SerializeField] List<Transform> dropOffSpots = new List<Transform>();

    NavMeshPath path;
    Coroutine arrivalRoutine;
    float distToArrival = 2.0f;
    float agentSpeed;

    public CarState carState = CarState.Idle;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Start(){
        path = new NavMeshPath();
        agentSpeed = agent.speed;
    }

    void Update(){
        switch(carState){
            case CarState.DrivingToPickup:
                float dist = Vector3.Distance(agent.destination, agent.transform.position);
                if(dist<distToArrival){
                    ArriveAtPickup();
                }
                break;
            case CarState.DrivingToDropOff:
                float dist2 = Vector3.Distance(agent.destination, agent.transform.position);
                if(dist2<distToArrival){
                    ArriveAtDropoff();
                }
                break;
        }
    }

    public void SetPickupDestination(int locationNum, OrderUIPrefab prefab){
        
        if(NavMesh.CalculatePath(agent.transform.position, locationSpots[locationNum].position, NavMesh.AllAreas, path)){
            agent.destination = locationSpots[locationNum].position;
            
            agent.speed = agentSpeed;
            carState = CarState.DrivingToPickup;
        } else {
            Debug.Log("No path");
        }
    }

    public void SetDropOffDestination(int locationNum, OrderUIPrefab prefab){
        int dropSpotNum = Random.Range(0,dropOffSpots.Count);
        
        if(NavMesh.CalculatePath(agent.transform.position, dropOffSpots[dropSpotNum].position, NavMesh.AllAreas, path)){
            agent.destination = dropOffSpots[dropSpotNum].position;
            
            agent.speed = agentSpeed;
            carState = CarState.DrivingToDropOff;
        } else {
            Debug.Log("No path");
        }
        
    }

    public void ArriveAtPickup(){
        carState = CarState.Idle;
        agent.speed = 0;
        arrivalRoutine = StartCoroutine(PickUpFade());
    }

    public void ArriveAtDropoff(){
        carState = CarState.Idle;
        agent.speed = 0;
        arrivalRoutine = StartCoroutine(DropOffFade());
    }

    IEnumerator PickUpFade(){
        HUDControls.Instance.PauseTime();
        HUDControls.Instance.overlay.DOFade(1.0f,1.0f).SetUpdate(UpdateType.Late, true);
        yield return new WaitForSecondsRealtime(2.3f);
        DeliveryControls.Instance.SwitchToWaitingToDeliver(DeliveryControls.Instance.activeOrder, DeliveryControls.Instance.activeOrder.orderNum);
        HUDControls.Instance.overlay.DOFade(0.0f,2.0f).SetUpdate(UpdateType.Late, true);
        
        HUDControls.Instance.pickUpOverlay.DOFade(1.0f,2.0f).SetUpdate(UpdateType.Late, true);
        yield return new WaitForSecondsRealtime(1.0f);
        HUDControls.Instance.pickUpOverlay.DOFade(0.0f,2.0f).SetUpdate(UpdateType.Late, true);
        yield return new WaitForSecondsRealtime(1.5f);
        HUDControls.Instance.ResumeTime();
    }

    IEnumerator DropOffFade(){
        HUDControls.Instance.PauseTime();
        HUDControls.Instance.overlay.DOFade(1.0f,1.0f).SetUpdate(UpdateType.Late, true);
        yield return new WaitForSecondsRealtime(2.3f);
        
        // Get payment
        HUDControls.Instance.AddPayment(DeliveryControls.Instance.activeOrder.payment);
        DeliveryControls.Instance.RemoveOrder(DeliveryControls.Instance.activeOrder);
        HUDControls.Instance.overlay.DOFade(0.0f,2.0f).SetUpdate(UpdateType.Late, true);
        HUDControls.Instance.dropOffOverlay.DOFade(1.0f,2.0f).SetUpdate(UpdateType.Late, true);
        yield return new WaitForSecondsRealtime(1.0f);
        HUDControls.Instance.dropOffOverlay.DOFade(0.0f,2.0f).SetUpdate(UpdateType.Late, true);;
        yield return new WaitForSecondsRealtime(1.5f);
        HUDControls.Instance.ResumeTime();


    }

}
