using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Boss {
    ///<summary>
    /// The basic boss state. It gives access to the bossai class from the state to access its data. Also it debugs the gizmos
    ///</summary>
    public abstract class BaseBossState : IState{
    public BossAI bossAI {get; set;}
    protected Boss Boss { get{ return bossAI.Boss; }}
    protected BossBody Body { get{ return bossAI.Boss.Body; }}
    protected BossPositioner Positioner { get{ return bossAI.Boss.BossPositioner; }}

    public ILiveStateDelegate OnStateSwitch { get; set; }
    private GUIStyle debugStyle;
    public string stateName = "[no state name specified]";
        public virtual void DrawDebug()
        {
#if UNITY_EDITOR
            debugStyle = new GUIStyle();
            debugStyle.normal.textColor = Color.white;
            debugStyle.fontSize = 10;
            debugStyle.border = new RectOffset(5,5,5,5);
            GUI.backgroundColor = Color.black;
            Handles.Label(bossAI.transform.position, stateName, debugStyle);
#endif        
        }

        public virtual void Exit()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void Start()
        {

        }
    }
}