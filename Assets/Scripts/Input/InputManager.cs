using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private FPMovement movement;

    [SerializeField]
    private PickupThrow pickupThrow;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) pickupThrow.pickup();
        if (Input.GetButtonUp("Fire1")) pickupThrow.release();

        if (Input.GetKeyDown(KeyCode.Space)) movement.Jump();
        if (Time.timeScale == 1) movement.SetMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        else movement.SetMovement(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        
        movement.setMouseDelta(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }
}
