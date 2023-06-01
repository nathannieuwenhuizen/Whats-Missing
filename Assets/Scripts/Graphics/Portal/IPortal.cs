using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortal {
    public void Teleport(Player _player);
    public IPortal ConnectedPortal{ get; }
    public bool InsidePortal { get; set; }
    public void OnPortalEnter(Player _player);
    public void OnPortalLeave();

}