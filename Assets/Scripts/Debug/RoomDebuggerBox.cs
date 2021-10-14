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

#if UNITY_EDITOR
        //to get the label and field next to each other
        public float valueField(float yPos, string header, float val, float min, float max) 
        {
            GUI.Label(new Rect(pos.x + horizontalOffset, yPos, size.x - horizontalOffset * 2, 15), header + " ");
            float value = GUI.HorizontalSlider(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), val, min, max);
            EditorGUIUtility.AddCursorRect(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), MouseCursor.Link);
            return value;
        }

        //to get the label and field next to each other
        public int valueField(float yPos, string header, int val, int min, int max)
        {
            GUI.Label(new Rect(pos.x + horizontalOffset, yPos, size.x - horizontalOffset * 2, 15), header);
            int value = EditorGUI.IntSlider(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), val, min, max);
            EditorGUIUtility.AddCursorRect(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), MouseCursor.Link);
            return value;
        }
#endif

        //draw function of the box
        public void Draw(Room currentRoom)
        {
#if UNITY_EDITOR

            style = new GUIStyle();
            style.normal.textColor = Color.white;

            pos = new Vector2(5, 5);
            size = new Vector2(200, 100);

            Handles.BeginGUI();

            GUI.color = Color.white;
            GUI.skin.box.fontStyle = FontStyle.Bold;
            GUI.Box(new Rect(pos.x, pos.y, size.x, size.y), "Room changes");
            GUI.color = Color.white;
            float currentPos = pos.y;


            if (currentRoom == null) return;
            
            foreach(Change change in currentRoom.ChangeHandler.Changes) {
                GUI.color = change.active ? Color.green : Color.red;
                currentPos += 30;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.Label(new Rect(pos.x + horizontalOffset, currentPos, size.x, 30), change.word + " | " + change.television.changeType, style);
                GUI.skin.label.fontStyle = FontStyle.Normal;
            }
            Handles.EndGUI();
#endif
        }
    }
    
