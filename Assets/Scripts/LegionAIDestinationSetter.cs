using System;
using Pathfinding;
using UnityEngine;

public class LegionAIDestinationSetter : VersionedMonoBehaviour {
    /// <summary>The object that the AI should move to</summary>
    public Transform target;
    IAstarAI ai;

    private bool m_isMoving;

    public Action OnMoveComplete;

    void OnEnable () {
        ai = GetComponent<IAstarAI>();
        // Update the destination right before searching for a path as well.
        // This is enough in theory, but this script will also update the destination every
        // frame as the destination is used for debugging and may be used for other things by other
        // scripts as well. So it makes sense that it is up to date every frame.
        if (ai != null) ai.onSearchPath += Update;
    }

    void OnDisable () {
        if (ai != null) ai.onSearchPath -= Update;
    }

    private void Start()
    {
        this.StartPath();
    }

    public void StartPath()
    {
        m_isMoving = true;
    }

    /// <summary>Updates the AI's destination every frame</summary>
    void Update () {
        if (target != null && ai != null)
        {
            ai.destination = target.position;
            if (m_isMoving && ai.reachedEndOfPath)
            {
                m_isMoving = false;
                target = null;
                if (OnMoveComplete != null)
                    OnMoveComplete();
                Debug.Log("Move End");
            }
        }
    }
}