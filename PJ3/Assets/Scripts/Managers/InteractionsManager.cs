using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security;
using UnityEngine;

// Every interactable object has to have this interface implemented
interface IInteractable{
    public void Interact();
}

public class InteractionsManager : MonoBehaviour
{
    // Origin Point of the interaction: our player
    public Transform interactorSource;

    // distance to which he can interact
    public float interactRange;

    public Camera cam;

    public Transform holdPos;

    public float throwForce = 200f; 
    
    private GameObject heldObj;

    private Rigidbody heldObjRb; //rigidbody of object we pick up


    void Update(){
        //keep object position the same as the holdPosition position
        if (heldObj != null){
            heldObj.transform.position = holdPos.transform.position;
            heldObj.transform.eulerAngles = holdPos.transform.eulerAngles;
        } 
    }

    public void Interaction(){
        // Interaction ray
        Ray r = new Ray(cam.transform.position, cam.transform.forward);
        // If the ray catches anything in the range, it will try to interact with it
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange)){
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj)){
                interactObj.Interact();
            }
        }
    }

    public void Picking_Up(){
        Ray r = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            
            //make sure pickup tag is attached
            GameObject interactObj = hitInfo.collider.gameObject;
            // if (hitInfo.collider.gameObject.TryGetComponent(out GameObject interactObj))
            // {
            //pass in object hit into the PickUpObject function
            if (interactObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
            {
                heldObj = interactObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
                
                heldObjRb = interactObj.GetComponent<Rigidbody>(); //assign Rigidbody
                heldObjRb.isKinematic = true;
                heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
                heldObj.layer = 7; //change the object layer to the holdLayer
                //make sure object doesnt collide with player, it can cause weird bugs
                Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), interactorSource.gameObject.GetComponent<Collider>(), true);
            }
            //Debug.Log("Picked up: " + heldObj.name);
            //}
        }
    }

    public void Droping(){
        if (heldObj != null){
            var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
            //have to use RaycastAll as object blocks raycast in center screen
            //RaycastAll returns array of all colliders hit within the cliprange
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, cam.transform.forward, clipRange);
            //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
            if (hits.Length > 1)
            {
                //change object position to camera position 
                heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
                //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
            }
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), interactorSource.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb.AddForce(cam.transform.forward * throwForce);
            heldObj = null;
        }
    }
}
