using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class WalkerAgent : Agent
{
    public Transform Target;
    private Rigidbody agentRb;
    private HingeJoint[] joints;
    private float[] initialAngles;
    private float previousDistance;

    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        joints = GetComponentsInChildren<HingeJoint>();
        initialAngles = new float[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            initialAngles[i] = joints[i].angle;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f, 0.3f, 0f);
        agentRb.linearVelocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].transform.localRotation = Quaternion.Euler(0f, 0f, initialAngles[i]);
            joints[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        Target.localPosition = new Vector3(-3f, 2f, 0f);
        previousDistance = Vector3.Distance(transform.localPosition, Target.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(agentRb.linearVelocity);
        for (int i = 0; i < joints.Length; i++)
        {
            sensor.AddObservation(joints[i].angle / 180f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var cont = actions.ContinuousActions;
        for (int i = 0; i < joints.Length; i++)
        {
            JointMotor motor = joints[i].motor;
            motor.force = 100f;
            motor.targetVelocity = cont[i] * 500f;
            motor.freeSpin = false;
            joints[i].motor = motor;
            joints[i].useMotor = true;
        }

        float currentDist = Vector3.Distance(transform.localPosition, Target.localPosition);
        float progress = previousDistance - currentDist;
        SetReward(progress * 0.1f);
        previousDistance = currentDist;

        float uprightDot = Vector3.Dot(transform.up, Vector3.up);
        AddReward((uprightDot - 0.5f) * 0.01f);

        if (currentDist < 1f)
        {
            SetReward(1f);
            EndEpisode();
        }
        else if (transform.localPosition.y < 0f)
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var keyboard = Keyboard.current;
        var cont = actionsOut.ContinuousActions;
        cont[0] = keyboard.qKey.isPressed ?  1f : keyboard.aKey.isPressed ? -1f : 0f;
        cont[1] = keyboard.wKey.isPressed ?  1f : keyboard.sKey.isPressed ? -1f : 0f;
        cont[2] = keyboard.eKey.isPressed ?  1f : keyboard.dKey.isPressed ? -1f : 0f;
        cont[3] = keyboard.rKey.isPressed ?  1f : keyboard.fKey.isPressed ? -1f : 0f;
        cont[4] = keyboard.tKey.isPressed ?  1f : keyboard.gKey.isPressed ? -1f : 0f;
        cont[5] = keyboard.yKey.isPressed ?  1f : keyboard.hKey.isPressed ? -1f : 0f;
        cont[6] = keyboard.uKey.isPressed ?  1f : keyboard.jKey.isPressed ? -1f : 0f;
        cont[7] = keyboard.iKey.isPressed ?  1f : keyboard.kKey.isPressed ? -1f : 0f;
    }

    void FixedUpdate()
    {
        RequestDecision();
    }
}