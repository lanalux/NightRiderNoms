using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] 
public class Order{
    public string locationName;
    public int locationNum;
    public Transform locationTransform;
    public float minPayment;
    public float maxPayment;
    public float minTimeLimit;
    public float maxTimeLimit;
    public float timeLeft;
}

[System.Serializable] 
public class OrderUIPrefab{
    public int orderNum;
    public string locationName;
    public string customer;
    public GameObject orderUIPrefab;
    public float fullTime;
    public float timeLeftToAccept;
    public float payment;
    public OrderState orderState = OrderState.Initial;

    public OrderUIPrefab (int orderNum, string locationName, string customer, GameObject orderUIPrefab, float fullTime, float timeLeftToAccept, float payment, OrderState orderState){
        orderNum = this.orderNum;
        locationName = this.locationName;
        customer = this.customer;
        orderUIPrefab = this.orderUIPrefab;
        fullTime = this.fullTime;
        timeLeftToAccept = this.timeLeftToAccept;
        payment = this.payment;
        orderState = this.orderState;
    }
}

public enum OrderState{Initial, WaitingToPickup, PickingUp, WaitingToDeliver, Delivering}

public class DeliveryControls : MonoBehaviour{

    public static DeliveryControls Instance {get;set;}
    [SerializeField] List<Order> allOrders = new List<Order>();
    [SerializeField] List<OrderUIPrefab> activeOrders = new List<OrderUIPrefab>();

    int maxOrders = 5;
    [SerializeField] Transform orderParent;
    [SerializeField] GameObject orderPrefab, waitingToPickupPrefab, pickupPrefab, waitingToDeliverPrefab, deliveringPrefab;
    Coroutine orderRoutine;

    float minTimeBetweenOrders = 5.0f;
    float maxTimeBetweenOrders = 20.0f;
    float orderHeight = 0.15f;

    float timerFullWidth = 300.0f;
    float timerHeight = 5.5f;

    public OrderUIPrefab activeOrder;

    List<string> customerNames = new List<string>(){
        "Timmy",
        "Jimmy",
        "Matt",
        "Frank",
        "John",
        "Lillani",
        "Randy",
        "Jeff",
        "Lane",
        "Jamila",
        "Tatiana",
        "Jason",
        "Matt II",
        "Blazej",
        "Ryan",
        "Geo",
        "Charlie",
        "Khayra",
        "Mark",
        "Stella",
        "Jesus",
        "Bieber",
        "Sarah",
        "Andy",
        "Eric",
        "Andreas",
        "Koray",
        "Juan Carlos",
        "Hiro",
        "Horatio",
        "Arnold",
        "Neo",
        "Blaze",
        "Lloyd",
        "Agrippa",
        "Dwight",
        "Willy"
    };


    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Start(){
        orderRoutine = StartCoroutine(StartOrderQueue());
    }

    void Update(){
        List<OrderUIPrefab> ordersToRemove = new List<OrderUIPrefab>();
        List<GameObject> gameObjectsToRemove = new List<GameObject>();

        for(int i=0; i<activeOrders.Count; i++){
            if(activeOrders[i].orderState == OrderState.Initial){
                if(activeOrders[i].timeLeftToAccept>0){
                    activeOrders[i].timeLeftToAccept -= Time.deltaTime;
                } else {
                    ordersToRemove.Add(activeOrders[i]);
                    gameObjectsToRemove.Add(activeOrders[i].orderUIPrefab);
                }
            }
        }

        RemoveOrders(ordersToRemove, gameObjectsToRemove);
        UpdateTimers();
    }

    IEnumerator StartOrderQueue(){
        while(true){
            if(activeOrders.Count<maxOrders){
                StartNewRandomOrder();
            }
            yield return new WaitForSeconds(Random.Range(minTimeBetweenOrders, maxTimeBetweenOrders));
        }
    }

    void UpdateTimers(){
        for (int i=0; i<activeOrders.Count; i++){
            
            float newTimerHeight = (activeOrders[i].timeLeftToAccept/activeOrders[i].fullTime)*timerFullWidth;
            activeOrders[i].orderUIPrefab.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(newTimerHeight, timerHeight);
        }
    }

    void StartNewRandomOrder(){
        int currentOrderNum = Random.Range(0, allOrders.Count);

        float payment = Random.Range(allOrders[currentOrderNum].minPayment, allOrders[currentOrderNum].maxPayment); 

        string customer = customerNames[Random.Range(0, customerNames.Count)];
        string locationName = allOrders[currentOrderNum].locationName;

        GameObject newOrder = Instantiate(orderPrefab, orderParent);
        newOrder.transform.position += new Vector3(0,orderHeight*activeOrders.Count,0);
        newOrder.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = allOrders[currentOrderNum].locationName;
        newOrder.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "Order for " + customer;

        float timeLeftOnOrder = Random.Range(allOrders[currentOrderNum].minTimeLimit, allOrders[currentOrderNum].maxTimeLimit);
        OrderUIPrefab newUIPrefab = new OrderUIPrefab(currentOrderNum, locationName, customer, newOrder, timeLeftOnOrder, timeLeftOnOrder, payment, OrderState.Initial);
        newUIPrefab.orderNum = currentOrderNum;
        newUIPrefab.locationName = locationName;
        newUIPrefab.customer = customer;
        newUIPrefab.orderUIPrefab = newOrder;
        newUIPrefab.fullTime = timeLeftOnOrder;
        newUIPrefab.timeLeftToAccept = timeLeftOnOrder;
        newUIPrefab.payment = payment;
        newUIPrefab.orderState = OrderState.Initial;
        
        activeOrders.Add(newUIPrefab);

        PickUpTrigger pickupTrigger = newOrder.transform.GetChild(1).GetComponent<PickUpTrigger>();
        DenyOrderTrigger denyTrigger = newOrder.transform.GetChild(2).GetComponent<DenyOrderTrigger>();

        if(pickupTrigger!=null){
            pickupTrigger.prefab = newUIPrefab;
            pickupTrigger.destinationNum = allOrders[currentOrderNum].locationNum;
        }
        if(denyTrigger!=null){
            denyTrigger.uiPrefab = newUIPrefab;
            denyTrigger.go = newOrder;
        }
    }

    public void RemoveOrder(OrderUIPrefab prefab){
        Destroy(prefab.orderUIPrefab);
        activeOrders.Remove(prefab);
        RepositionPrefabs();
    }

    public void SwitchToWaitingToPickUp(OrderUIPrefab prefab, int locationNum){
        int index = prefab.orderUIPrefab.transform.GetSiblingIndex();
        GameObject newPickUp = Instantiate(waitingToPickupPrefab, orderParent);
        Destroy(prefab.orderUIPrefab);
        newPickUp.transform.SetSiblingIndex(index);
        newPickUp.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = prefab.locationName + " (" + prefab.customer + ")";
        prefab.orderUIPrefab = newPickUp;
        
        // SET NEXT STEP
        
        DestinationTrigger trigger = newPickUp.transform.GetChild(1).GetComponent<DestinationTrigger>();
        if(trigger!=null){
            trigger.destinationNum = locationNum;
            trigger.prefab = prefab;
        }
        prefab.orderState = OrderState.WaitingToPickup;
    }

    public void SwitchToPickUp(OrderUIPrefab prefab){
        CancelCurrentPickUps();
        activeOrder = prefab;
        int index = prefab.orderUIPrefab.transform.GetSiblingIndex();
        GameObject newPickUp = Instantiate(pickupPrefab, orderParent);
        Destroy(prefab.orderUIPrefab);
        newPickUp.transform.SetSiblingIndex(index);
        newPickUp.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = prefab.locationName + " (" + prefab.customer + ")";
        prefab.orderUIPrefab = newPickUp;
        prefab.orderState = OrderState.PickingUp;
    }

    public void SwitchToWaitingToDeliver(OrderUIPrefab prefab, int locationNum){
        int index = prefab.orderUIPrefab.transform.GetSiblingIndex();
        GameObject newDelivery = Instantiate(waitingToDeliverPrefab, orderParent);
        Destroy(prefab.orderUIPrefab);
        newDelivery.transform.SetSiblingIndex(index);
        prefab.orderUIPrefab = newDelivery;
        newDelivery.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = prefab.locationName + " (" + prefab.customer + ")";
        // SET NEXT STEP
        
        StartDeliveryTrigger trigger = newDelivery.transform.GetChild(1).GetComponent<StartDeliveryTrigger>();
        if(trigger!=null){
            trigger.destinationNum = locationNum;
            trigger.prefab = prefab;
        }
        prefab.orderState = OrderState.WaitingToDeliver;
    }
    
    public void SwitchToDroppingOff(OrderUIPrefab prefab){
        CancelCurrentPickUps();
        activeOrder = prefab;
        int index = prefab.orderUIPrefab.transform.GetSiblingIndex();
        GameObject newDelivery = Instantiate(deliveringPrefab, orderParent);
        Destroy(prefab.orderUIPrefab);
        newDelivery.transform.SetSiblingIndex(index);
        newDelivery.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = prefab.locationName + " (" + prefab.customer + ")";
        prefab.orderUIPrefab = newDelivery;
        prefab.orderState = OrderState.Delivering;
    }

    public void CancelCurrentPickUps(){
        for(int i=0; i<activeOrders.Count; i++){
            if(activeOrders[i].orderState==OrderState.PickingUp){
                SwitchToWaitingToPickUp(activeOrders[i], activeOrders[i].orderNum);
            }
        }
        
        for(int i=0; i<activeOrders.Count; i++){
            if(activeOrders[i].orderState==OrderState.Delivering){
                SwitchToWaitingToDeliver(activeOrders[i], activeOrders[i].orderNum);
            }
        }
    }

    void RemoveOrders(List<OrderUIPrefab> removeTheseOrders, List<GameObject> GOs){
        foreach(GameObject g in GOs){
            Destroy(g);
        }
        foreach(OrderUIPrefab i in removeTheseOrders){
            activeOrders.Remove(i);
        }
        RepositionPrefabs();
    }

    void RepositionPrefabs(){
        for(int i=0; i<activeOrders.Count; i++){
            activeOrders[i].orderUIPrefab.transform.position = orderParent.position + new Vector3(0,(orderHeight*i),0);
        }
    }
}
