using BNG;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Allows the Player to climb objects by Grabbing them
/// </summary>
public class Climbable : Grabbable
{

    public PlayerClimbing playerClimbing;

    public bool centauri
    {
        get
        {
            if (SceneManager.GetActiveScene().name == "XR Demo")
            {
                return false;
            }
            else { return true; }
        }
    }

    void Start()
    {
        // Make sure Climbable is set to dual grab
        SecondaryGrabBehavior = OtherGrabBehavior.DualGrab;

        // Make sure we don't try tp keep this in our hand
        GrabPhysics = GrabPhysics.None;

        CanBeSnappedToSnapZone = false;

        TwoHandedDropBehavior = TwoHandedDropMechanic.None;

        // Disable Break Distance entirely if default from Grabbable was used
        if (BreakDistance == 1)
        {
            BreakDistance = 0;
        }

        if (player != null && !centauri)
        {
            playerClimbing = player.gameObject.GetComponentInChildren<PlayerClimbing>();
        }
    }

    public override void Update()
    {
        if (playerClimbing == null & centauri)
            playerClimbing = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerClimbing>();

        base.Update();
    }

    public override void GrabItem(Grabber grabbedBy)
    {

        //Add the climber so we can track it's position for Character movement
        if (playerClimbing)
        {
            playerClimbing.AddClimber(this, grabbedBy);
        }

        base.GrabItem(grabbedBy);
    }

    public override void DropItem(Grabber droppedBy)
    {
        if (droppedBy != null && playerClimbing != null)
        {
            playerClimbing.RemoveClimber(droppedBy);
        }

        base.DropItem(droppedBy);
    }
}