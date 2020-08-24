using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class Pedestrian : MonoBehaviour {
	
	Vector3 start;
	Vector3 target;
	private float speed;

    private List<PedestrianPosition> positions = new List<PedestrianPosition>();
    private PlaybackControl pc;
    private Vector3 lastPos;

	private AgentView agentView = null;
#pragma warning disable 108
    private Animator animator;
	#pragma warning restore 108
	private LODGroup lodGroup;
	private bool targetReached = true;


    public void init() {
        gameObject.SetActive(true);

        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("paused", true);

        lodGroup = GetComponentInChildren<LODGroup>();

        AgentView agentViewComponent = GameObject.Find("CameraMode").GetComponent<AgentView>();
		if (agentViewComponent.enabled)
			agentView = agentViewComponent;

		reset();
        lastPos = transform.position;
	}

    public void init(int id, PedestrianPosition pos) {
        this.name = "Pedestrian_" + id;
        positions.Add(pos);
    }

    internal int getCurrentFloorID(float currentTime) {
        return GetClosestPosition(currentTime).getFloorID();
    }

    internal void dev() {
        foreach (PedestrianPosition pos in positions) {
            Debug.Log(pos.getTime() + ": " + pos.getX() + ", " + pos.getY() + ", " + pos.getZ());
        }
    }

    public int getPositionsCount() {
		return positions.Count;
	}

    public void addPos(PedestrianPosition pos) {
        PedestrianPosition ceil = positions.FirstOrDefault(i => i.getTime() > pos.getTime());
        PedestrianPosition floor = positions.LastOrDefault(i => i.getTime() < pos.getTime());
        if (ceil != null)
            positions.Insert(positions.IndexOf(ceil), pos);
        else if (floor != null)
            positions.Insert(positions.IndexOf(floor) + 1, pos);
        else
            positions.Insert(0, pos);
    }

    internal void pause(Boolean playing) {
        animator.SetBool("paused", !playing);
    }

    internal bool reachedTarget() {
        return targetReached;
    }

    public void reset()
    {
        animator.SetBool("paused", true);
        targetReached = false;
        PedestrianPosition pos = positions[0];
		transform.position = new Vector3 (pos.getX (), pos.getZ(), pos.getY ());
	}

	private bool showPed() {
		if (agentView != null)
			return agentView.getCurrentPed () != gameObject;
		return true;
	}

    private PedestrianPosition GetClosestPosition(float currentTime) {
        return positions.LastOrDefault(i => i.getTime() < currentTime);
     }

    public void move (float currentTime) {
        PedestrianPosition pos = GetClosestPosition(currentTime);

        if (pos == null || positions.IndexOf(pos) >= positions.Count - 2) { // = target reached
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>()) {
                r.enabled = false;
            }
            animator.SetBool("paused", false);
            targetReached = true;
            return;
        } else {
            targetReached = false;
        }

        if (!targetReached) {
            PedestrianPosition pos2 = positions[positions.IndexOf(pos)+1];
  			start = new Vector3 (pos.getX (), pos.getZ(), pos.getY ()); // the y-coord in Unity is the z-coord from the kernel: the up and down direction
            // it is on purpose that the y position stays the same as position to avoid kippen of peds
			target = new Vector3 (pos2.getX (), pos2.getZ(), pos2.getY ());
            Vector3 targetForLook = new Vector3(pos2.getX(), pos.getZ(), pos2.getY());
            float time = currentTime;
			float timeStepLength = Mathf.Clamp (pos2.getTime () - pos.getTime (), 0.1f, 50f); // We don't want to divide by zero. OTOH, this results in pedestrians never standing still.
			float movement_percentage = (time - pos.getTime ()) / timeStepLength;

            Vector3 newPosition = Vector3.Lerp (start, target, movement_percentage);
			Vector3 relativePos = target - start;
			speed = relativePos.magnitude;
			animator.SetFloat("walkingSpeed", speed / timeStepLength);
            // TODO: not needed for cylinders
            if (start != targetForLook)
              transform.rotation = Quaternion.LookRotation (targetForLook - start);
			transform.position = newPosition;
		}				
	}	
}
