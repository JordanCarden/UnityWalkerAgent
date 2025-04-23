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
    private const float successDistance = 2f;

    void FixedUpdate()
    {
        RequestDecision();
    }

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
        transform.localPosition = new Vector3(2f, 2.1f, 0f);
        agentRb.linearVelocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].transform.localRotation = Quaternion.Euler(0f, 0f, initialAngles[i]);
            joints[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        Target.localPosition = new Vector3(-2f, 2f, 0f);
        previousDistance = Vector3.Distance(transform.localPosition, Target.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        AddSafeObs(sensor, transform.localPosition.x);
        AddSafeObs(sensor, transform.localPosition.y);
        AddSafeObs(sensor, transform.localPosition.z);
        AddSafeObs(sensor, Target.localPosition.x);
        AddSafeObs(sensor, Target.localPosition.y);
        AddSafeObs(sensor, Target.localPosition.z);
        AddSafeObs(sensor, agentRb.linearVelocity.x);
        AddSafeObs(sensor, agentRb.linearVelocity.y);
        AddSafeObs(sensor, agentRb.linearVelocity.z);
        for (int i = 0; i < joints.Length; i++)
        {
            AddSafeObs(sensor, joints[i].angle / 180f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var cont = actions.ContinuousActions;
        for (int i = 0; i < joints.Length; i++)
        {
            var m = joints[i].motor;
            m.force = 100f;
            m.targetVelocity = cont[i] * 500f;
            joints[i].motor = m;
            joints[i].useMotor = true;
        }
        float currentDist = Vector3.Distance(transform.localPosition, Target.localPosition);
        float progress = previousDistance - currentDist;
        SetReward(progress * 0.1f);
        previousDistance = currentDist;
        float uprightDot = Vector3.Dot(transform.up, Vector3.up);
        AddReward((uprightDot - 0.5f) * 0.01f);
        if (currentDist < successDistance)
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
        var cont = actionsOut.ContinuousActions;
        var kb = Keyboard.current;
        cont[0] = kb.qKey.isPressed  ? 1f : kb.aKey.isPressed ? -1f : 0f;
        cont[1] = kb.wKey.isPressed  ? 1f : kb.sKey.isPressed ? -1f : 0f;
        cont[2] = kb.eKey.isPressed  ? 1f : kb.dKey.isPressed ? -1f : 0f;
        cont[3] = kb.rKey.isPressed  ? 1f : kb.fKey.isPressed ? -1f : 0f;
        cont[4] = kb.tKey.isPressed  ? 1f : kb.gKey.isPressed ? -1f : 0f;
        cont[5] = kb.yKey.isPressed  ? 1f : kb.hKey.isPressed ? -1f : 0f;
        cont[6] = kb.uKey.isPressed  ? 1f : kb.jKey.isPressed ? -1f : 0f;
        cont[7] = kb.iKey.isPressed  ? 1f : kb.kKey.isPressed ? -1f : 0f;
    }

    private void AddSafeObs(VectorSensor sensor, float v)
    {
        if (float.IsNaN(v) || float.IsInfinity(v)) v = 0f;
        sensor.AddObservation(Mathf.Clamp(v, -5f, 5f));
    }
}