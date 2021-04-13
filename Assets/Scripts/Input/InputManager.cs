using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private FPMovement movement;

    [SerializeField]
    private Hands hands;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //mouse
        if (Input.GetButtonDown("Fire1")) {
            hands.Grab();
        }
        if (Input.GetButtonUp("Fire1"))  {
            hands.Release();
        }


        //controller
        if (Input.GetButtonDown("Fire1 Controller")) {
            hands.Grab();
        }
        if (Input.GetButtonUp("Fire1 Controller"))  {
            hands.Release();
            if (TelevisionButton.SELECTED_BUTTON != null) TelevisionButton.SELECTED_BUTTON.gameObject.GetComponent<Button>().onClick.Invoke();
        }
        
        //movement
        if (Input.GetButtonDown("Jump")) movement.Jump();
        if (Time.timeScale == 1) movement.SetMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        else movement.SetMovement(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        
        movement.setMouseDelta(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }
}
