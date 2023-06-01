using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionRoom : Room
{
    public PotionSpawner potionSpawner;

    public  override void OnRoomEnter(Player _player, bool loadSaveData = false) {
        potionSpawner.potionData = roomLevel.potionInfo;
        base.OnRoomEnter(_player, loadSaveData);
    }
}
