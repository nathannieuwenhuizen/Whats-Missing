using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private FPMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.SetMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }
}
