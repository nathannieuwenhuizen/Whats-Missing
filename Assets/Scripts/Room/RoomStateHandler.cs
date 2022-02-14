using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RoomStateHandler
{
    private Room room;

    public RoomStateHandler(Room _room) {
        room = _room;
    }

    ///<summary>
    /// Loads the savedata with all the mirror states and the cordinates of the roomobjects
    ///</summary>
    public void LoadState(SaveData data) {
    }

}
