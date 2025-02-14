using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform viewPoint;
    [SerializeField] float mouseSensitive = 1f;

    private float verticalRotationStored;
    private Vector2 mouseInput;
    private Camera cam;

    public int selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            PlayerMovement();
                if(Input.GetMouseButtonDown(0))
                {
                    GetClickableObject();
                }
            CursorUnlockWhenESC();
        }
    }

    private void GetClickableObject()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("HIT" + hit.collider.gameObject.name);
            if(hit.collider.gameObject.tag == "Card")
            {
                selectedCard = int.Parse(hit.collider.gameObject.name);
            }
            
            IClicked click = hit.collider.gameObject.GetComponent<IClicked>();
            if(click != null)
            {
                click.OnClick(selectedCard);
            }
        }
    }

    private void PlayerMovement()
    {
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitive;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotationStored += mouseInput.y;
        verticalRotationStored = Mathf.Clamp(verticalRotationStored, -60, 60);
        viewPoint.rotation = Quaternion.Euler(-verticalRotationStored, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private static void CursorUnlockWhenESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0) && !UIController.instance.optionScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }

    }


}

// PhotonNetwork.LocalPlayer.ActorNumber = get actor number