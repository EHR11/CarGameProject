using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL= "Horizontal";
    private const string VERTICAL= "Vertical";

    private float kph;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool isBraking;
    private bool handBrake;
    
    private float mt, speed, flb, frb, rlb, rrb, lfslip, rfslip, rrslip, lrslip;

    [SerializeField] private float maxAccel;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float turnSensitivity;
    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider rearLeftCollider;
    [SerializeField] private WheelCollider rearRightCollider;
    [SerializeField] private Vector3 centerOfMass;
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform rearLeftTransform;
    [SerializeField] private Transform rearRightTransform;

    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
    }

    private void FixedUpdate() {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        rrslip=rearRightCollider.sidewaysFriction.extremumValue/rearRightCollider.sidewaysFriction.stiffness;
    }
    
    private void GetInput(){
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.DownArrow);
        handBrake = Input.GetKey(KeyCode.Space);
    }
    
    private void HandleMotor(){
        speed=rearLeftCollider.rpm*(rearLeftCollider.radius * 2f * 3.14f)*60/1000;
        kph=rb.velocity.magnitude*3.6f;
        rearLeftCollider.motorTorque = verticalInput * maxAccel;
        rearRightCollider.motorTorque = verticalInput * maxAccel;
        ApplyBraking();
    }

    private void ApplyBraking(){
        currentBrakeForce= (isBraking && speed>0) || handBrake ? brakeForce : 0f;
        if(!handBrake){
            flb = frontLeftCollider.brakeTorque = currentBrakeForce;
            frb = frontRightCollider.brakeTorque = currentBrakeForce;
        }
        rlb = rearLeftCollider.brakeTorque = currentBrakeForce * 0.6f;
        rrb = rearRightCollider.brakeTorque = currentBrakeForce * 0.6f;
    }

    private void HandleSteering(){
        currentSteeringAngle = horizontalInput * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, currentSteeringAngle, turnSensitivity);
        frontRightCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, currentSteeringAngle, turnSensitivity);
    }

    private void UpdateWheels(){
        UpdateThisWheel(frontLeftCollider, frontLeftTransform);
        UpdateThisWheel(frontRightCollider, frontRightTransform);
        UpdateThisWheel(rearLeftCollider, rearLeftTransform);
        UpdateThisWheel(rearRightCollider, rearRightTransform);
    }
    
    private void UpdateThisWheel(WheelCollider wheelCollider, Transform wheelTransform){
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos,out rot);
        wheelTransform.rotation=rot;
        wheelTransform.position=pos;
    }
}
