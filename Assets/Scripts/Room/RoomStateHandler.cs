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
    /// Loads the savedata with all the television states and the cordinates of the roomobjects
    ///</summary>
    public void LoadState(SaveData data) {
        room.Player.transform.position = data.playerCordinates.position;
        room.Player.transform.rotation = data.playerCordinates.rotation;
        List<PickableRoomObjectCordinates> cordinates = data.cordinates.ToList<PickableRoomObjectCordinates>();
        List<TVState> tvStates = data.tvStates.ToList<TVState>();
        foreach (PickableRoomObject item in room.GetAllObjectsInRoom<PickableRoomObject>())
        {
            // Debug.Log("item id" + item.id);
            PickableRoomObjectCordinates itemCordinate = cordinates.Find(x => x.id == item.id);
            item.transform.position = itemCordinate.position;
            item.transform.rotation = itemCordinate.rotation;
        }
        foreach (RoomTelevision tv in room.allTelevisions)
        {
            TVState tvState = tvStates.Find(x => x.id == tv.id);
            tv.MirrorCnvas.DeselectLetters();
            tv.Word = tvState.word;
            // tv.UpdateAnswerTextPosition();
        }
    }

}
