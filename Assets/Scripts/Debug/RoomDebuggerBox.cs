using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomDebuggerBox
{
 //position and size of the box
    public Vector2 pos;
    public Vector2 size;

    //to make it more fancy!
    public GUIStyle style;
    public float horizontalOffset = 5;
    public string word = "";

    private Vector2 scrollViewVector = Vector2.zero;
    private string[] changeTypes;
    int n,i,selectedChangeType = 0;

#if UNITY_EDITOR

    protected void CreateBox(Rect rect, string text) {
        GUI.color = Color.white;
        GUI.skin.box.fontStyle = FontStyle.Bold;
        GUI.Box(rect, text);
        GUI.color = Color.white;

    }

    private void DrawDropDown(Vector2 pos) {
        changeTypes =  Enum.GetNames(typeof(ChangeType));
        
            
        
        if(GUI.Button(new Rect(pos.x + 100,pos.y,25,25), "")){
            if(n==0)n=1;
            else n=0;        
        }
    
        if(n==1){
            scrollViewVector = GUI.BeginScrollView (new Rect (pos.x, pos.y, 100, 115), scrollViewVector, new Rect (0, 0, 300, 500));
            GUI.Box(new Rect(0,0,300,500), ""); 
            for(i=0;i<4;i++){
                if(GUI.Button(new Rect(0,i*25,300,25), "")){
                n=0;selectedChangeType=i;        
                }              
                GUI.Label(new Rect(5,i*25,300,25), changeTypes[i]);           
            }
            GUI.EndScrollView();        
        }else{
            GUI.Label(new Rect(pos.x + 5, pos.y,300,25), changeTypes[selectedChangeType]);
        }            
    }


    //draw function of the box
    public void Draw(Room currentRoom, Mirror tv)
    {

        style = new GUIStyle();
        style.normal.textColor = Color.white;

        pos = new Vector2(5, 5);
        size = new Vector2(200, 150);

        Handles.BeginGUI();

        CreateBox(new Rect(pos.x, pos.y, size.x, size.y), "Room changes");

        float currentPos = pos.y;

        currentPos += 30;

        if (currentRoom != null) {

            foreach(MirrorChange change in currentRoom.ChangeHandler.MirrorChanges) {
                GUI.color = change.active ? Color.green : Color.red;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(pos.x + horizontalOffset, currentPos, size.x, 30), change.word + " | " + change.mirror.changeType, style);
                GUI.skin.label.fontStyle = FontStyle.Normal;
                currentPos += 30;
            }
        }
        
        CreateBox(new Rect(pos.x + size.x, pos.y, size.x, size.y), "Edit changes");
        word = GUI.TextField(new Rect(pos.x + size.x + horizontalOffset, 30, size.x, 30), word, 20);       

        if (GUI.Button(new Rect(pos.x + size.x + horizontalOffset, 60, size.x * .4f, 30), "ADD"))
        {
            currentRoom.Animated = true;
            tv.changeType = (ChangeType)selectedChangeType;
            // tv.changeType = Enum.Parse(typeof(ChangeType), changeTypes[selectedChangeType]);// ChangeType.missing;
            MirrorChange newChange = new MirrorChange() { word = word, mirror = tv, active = true};
            currentRoom.AddChangeInRoomObjects(newChange);
            currentRoom.ChangeHandler.MirrorChanges.Add(newChange);
        }        
        if (GUI.Button(new Rect(pos.x + size.x + horizontalOffset + size.x * .6f, 60, size.x * .4f, 30), "REMOVE"))
        {
            currentRoom.Animated = true;
            MirrorChange removedChange = currentRoom.ChangeHandler.MirrorChanges.Find(x => x.word == word);
            currentRoom.RemoveChangeInRoomObjects(removedChange);
            currentRoom.ChangeHandler.MirrorChanges.Remove(removedChange);
        } 
        DrawDropDown(new Vector2 (pos.x + size.x, 100));

        Handles.EndGUI();

    }
#endif
}

