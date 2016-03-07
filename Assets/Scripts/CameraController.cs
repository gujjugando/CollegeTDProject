﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    // Use this for initialization
    //Transform kingTransform;
    bool isFirstPerson = false;
    Vector3 cameraPosition;
    Quaternion cameraRotation;
    public float minimumX;
    public float maximumX;
    public float minimumZ;
    public float maximumZ;

    //GameObject king;
	void Start () {
        //store initial values of camera's transform
        cameraPosition = this.transform.position;
        cameraRotation = this.transform.rotation;
      //  kingTransform = GameObject.FindGameObjectWithTag("King").transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (isFirstPerson) {
         //   this.transform.position = kingTransform.position;
          //  this.transform.rotation = kingTransform.rotation;
        }
        else
        {
            float xMove = Input.GetAxis("Horizontal") * Time.deltaTime * 10;
            xMove = xMove + transform.position.x;
            xMove = Mathf.Clamp(xMove, minimumX, maximumX);
            float zMove = Input.GetAxis("Vertical") * Time.deltaTime * 10;
            zMove = zMove + transform.position.z;
            zMove = Mathf.Clamp(zMove, minimumZ, maximumZ);
            this.transform.position = new Vector3(xMove, transform.position.y, zMove);
        }
	}

    public void ChangeView() {//called from button changeView from scene and changes the view
        
        if (!isFirstPerson)
        {
            cameraPosition = this.transform.position;
            cameraRotation = this.transform.rotation;
            isFirstPerson = true;
         //   kingTransform.gameObject.GetComponent<King>().isFirstPerson = true;
        }else {
            this.transform.position = cameraPosition;
            this.transform.rotation = cameraRotation;
            isFirstPerson = false;
       //     kingTransform.gameObject.GetComponent<King>().isFirstPerson = false;
        }
    }

}
