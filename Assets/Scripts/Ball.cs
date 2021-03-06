using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour

{
    
    private int x = 0;
    public MyGameManager myGameManager;
    public GameSession gameSession;
    //public PlayerData playerData;
    private bool in_destroy_process = false;
    private bool ball_was_taken = false;
    private bool is_Hit = false;
    private int ID;
    
    private OVRPlayerController ovrPlayerController;
    public DrawDot drawDot;
    private bool is_grabbed = false;
    public GameObject wall;
    private GameObject leftHand, rightHand;
    private Transform leftHandPosition, rightHandPosition;
    // Start is called before the first frame update
    public GameObject trefferPrefab;
    public LineObj lineObj;
    
    public Parameter parameter;
    public Rigidbody rb;

    //* alle Variablen die mit einem "par_" beginnen muessen mit dem aktuellen
    //* Ball gespeichert werden und beschreiben die Wurfparameter
    private float par_target_radius;
    public float par_mass;
    public Vector3 par_gravity;
    public Vector3 par_offset_hand_pos;
    private Vector3 lastHandPos;
    //private GameObject OVRPlayerController;
private GameObject oVRCameraRig;
private GameObject forwardDirection;
private GameObject customRightHand;


private GameObject trackingSpace;
private GameObject centerEyeAnchor;
private GameObject trackerAnchor;

                private Vector3 ball_vel_frame_m1;
 private Vector3 ball_vel_frame_m0; 

private GameObject rightEye;
    void Start()
    {
        //OVRInput.Controller myRTouch = RTouch;
        print("start the BAll Script on Start of Object Ball");
        myGameManager = FindObjectOfType<MyGameManager>();
        gameSession = FindObjectOfType<GameSession>();
        parameter = FindObjectOfType<Parameter>();
        EventManager.StartListening ("Treffer", registerTreffer);
        ovrPlayerController = FindObjectOfType<OVRPlayerController>();
        wall = GameObject.Find("Wall");
        drawDot = FindObjectOfType<DrawDot>();
        //lineObj = FindObjectOfType<LineObj>();
        lineObj = myGameManager.lineObj.GetComponent<LineObj>();
		//FindObjectOfType<LineObj>();
        //lineObj = GameObject.Find("LineObj");

        //rightHand = GameObject.Find("CustomHandRight");
        rightHand = GameObject.Find("RightHandAnchor");
        customRightHand = GameObject.Find("CustomHandRight"); // has RigidBody (velocity)
        ball_vel_frame_m0 = new Vector3(0.0f, 0.0f, 0.0f);
        ball_vel_frame_m1 = new Vector3(0.0f, 0.0f, 0.0f);
        rightEye = GameObject.Find("RightEyeAnchor");


        rb = GetComponent<Rigidbody>(); 
        

        ID = myGameManager.get_current_Ball_ID();
        normalize_hand_controller();
        set_target_radius();
        set_physics();
        //set_hand_controller();
        
        //Debug.Log("wall scale = " + wall.transform.localScale.x ToString());
        //TODO Die Methode muss noch angepasst werden um alle Paramter mit zu speichern
        gameSession.register_new_Ball(
            ID, 
            parameter.current_target_radius,
            parameter.current_ball_mass, 
            parameter.current_gravity,
            parameter.current_force,
            parameter.current_offset_hand_pos, 
            parameter.current_offset_hand_vel,
            parameter.current_invert,
            parameter.current_tremor
            );
    }

    //* das Paramter Object wird durch den Gamemanager immer vor dem Spawnen 
    // eines neuen Balls geschrieben und ist zum Zeitpunkt des Balls
    private void set_target_radius()
    {
        par_target_radius = parameter.current_target_radius;
        Debug.Log("target_radius in ball = " + par_target_radius);
        lineObj.drawCircleX(par_target_radius, 0.051f);

    }

    private void set_physics()
    {

        par_mass = parameter.current_ball_mass;
        rb.mass = par_mass;
        rb.useGravity = true;
        //Debug.Log("par_mas = " + par_mas);
        par_gravity = parameter.get_gravitiy();
        Physics.gravity = par_gravity;

    }

    private void set_hand_controller()
    {
        Debug.Log("set Hand controller()...");
        Debug.Log("parameter.current_offset_hand_pos = " + parameter.current_offset_hand_pos);
        Debug.Log("parameter.current_offset_hand_vel = " + parameter.current_offset_hand_vel);
        Debug.Log("parameter.current_invert = " + parameter.current_invert);
        Debug.Log("parameter.current_tremor = " + parameter.current_tremor);
        OVRPlugin.carsten_offset_hand_pos = parameter.current_offset_hand_pos;
        OVRPlugin.carsten_offset_hand_vel = parameter.current_offset_hand_vel;
        OVRPlugin.carsten_invert = parameter.current_invert;
        OVRPlugin.carsten_tremor = parameter.current_tremor;       
        //rightHand.transform.position = rightHand.transform.position  + parameter.current_offset_hand_pos;
        //rightHand.transform.position = rightHand.transform.position  + new Vector3(0.2f, 0.0f, 0.0f);
        Debug.Log("offset = " + parameter.current_offset_hand_pos.ToString());
        Debug.Log("transform right hand to " + rightHand.transform.position.ToString());
        Debug.Log("transform ball       to " + transform.position.ToString());
        // Debug.Log("velocity ball       to " + rb.velocity.ToString());


        
    }

    private void normalize_hand_controller()
    {
        OVRPlugin.carsten_offset_hand_pos = new Vector3(0.0f, 0.0f, 0.0f);
        OVRPlugin.carsten_offset_hand_vel = new Vector3(1.0f, 1.0f, 1.0f);
        OVRPlugin.carsten_invert = new Vector3(0.0f, 0.0f, 0.0f);
        OVRPlugin.carsten_tremor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);       
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.RTouch)>0){
             //Debug.Log("RTouch");
             }
            Vector3 velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
             //Debug.Log(" XXX vel" + velocity.ToString());
            
            gameSession.add_Ball_Hand_Position(ID, 
            transform.position.x,
            transform.position.y,
            transform.position.z,
            rightHand.transform.position.x, 
            rightHand.transform.position.y, 
            rightHand.transform.position.z, 
            Time.time);
            //Debug.Log("transform right hand to " + rightHand.transform.position.ToString());
            //Debug.Log("transform ball       to " + transform.position.ToString());
            //Debug.Log("rb velocicty " + rb.velocity.ToString());
            // Debug.Log("RE_pos rightEye= " + rightEye.transform.position.ToString());
            // Debug.Log("RE_pos rightHand= " + rightHand.transform.position.ToString());
            // Debug.Log("RE_pos customRightHand= " + customRightHand.transform.position.ToString());

            // Debug.Log("RE_pos oVRCameraRig= " + oVRCameraRig.transform.position.ToString());
            // Debug.Log("RE_pos trackingSpace= " + forwardDirection.transform.position.ToString());
            // Debug.Log("RE_pos trackingSpace= " + trackingSpace.transform.position.ToString());
            // Debug.Log("RE_pos centerEyeAnchor= " + centerEyeAnchor.transform.position.ToString());
            // Debug.Log("RE_pos trackerAnchor= " + trackerAnchor.transform.position.ToString());
            // Debug.Log("transform right hand to " + rightHand.transform.position.ToString());
            // Debug.Log("transform ball       to " + transform.position.ToString());
            // Debug.Log("velocity ball       to " + rb.velocity.ToString());
            // Debug.Log("velocity handRB     to " + handRB.velocity.ToString());
            // Debug.Log("velocity VM     to " + vmRB.velocity.ToString());
            // Debug.Log("local vel = " + handRB.GetRelativePointVelocity(rightHand.transform.localPosition));
            // Debug.Log("customRightHand pos " + customRightHand.transform.position.ToString());

                // Debug.Log("velocity handAnchorRB     vel " + rb_rightHandAnchor.velocity.ToString());
//            lastHandPos = rightHand.transform.position;

        //Debug.Log("Grabbed = " + transform.GetComponent<OVRGrabbable>().isGrabbed);
        if (is_grabbed && transform.GetComponent<OVRGrabbable>().isGrabbed){
                // ball_vel_frame_m3 = ball_vel_frame_m2;
                // ball_vel_frame_m2 = ball_vel_frame_m1;
                if (rb.velocity == Vector3.zero){
                    //Debug.Log("grabbed and zero");
                }
                if (ball_vel_frame_m0 != Vector3.zero){
                    ball_vel_frame_m1 = ball_vel_frame_m0;
                }
                if (rb.velocity != Vector3.zero){
                    ball_vel_frame_m0 = rb.velocity;
                }

                //Debug.Log("grabbed adapt_velocity_to_old_ball_velocity rb.velocity" + rb.velocity.ToString());
                //Debug.Log("grabbed adapt_velocity_to_old_ball_velocity ball_vel_frame_m1" + ball_vel_frame_m1.ToString());
                //Debug.Log("grabbed adapt_velocity_to_old_ball_velocity ball_vel_frame_m0" + ball_vel_frame_m0.ToString());
        }        
        // test if Ball was grabbed
        if (!is_grabbed && transform.GetComponent<OVRGrabbable>().isGrabbed){
            is_grabbed = true;
            gameSession.set_pick_up_time(ID, Time.time);
            ball_was_taken = true; // this is set one time and markes the start of the recording of the ball position
            OVRPlugin.carsten_ball_grap_position = transform.position;
            intitialize_forces_on_ball();
            set_hand_controller();
            //rb.useGravity = true;
        }
        if (is_grabbed && !transform.GetComponent<OVRGrabbable>().isGrabbed){
            // The ball was grapped in the past and is now released
            // this is necessary if the ball was grapped a second time
            is_grabbed = false;
            gameSession.set_leave_the_Hand_Time(ID, Time.time);
            // Debug.Log("beim loslassen current invert " + parameter.current_invert.ToString());
            // Debug.Log("rb velocicty beim loslassen " + rb.velocity.ToString());
            // Debug.Log("hand velocicty beim loslassen " + handRB.velocity.ToString());
           // Debug.Log("VM velocicty beim loslassen " + vmRB.velocity.ToString());
           // Debug.Log("velocity handAnchorRB beim loslassen" + rb_rightHandAnchor.velocity.ToString());
            //Vector3 velocityx = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            // Debug.Log("velocity OVRInput beim loslassen" + velocityx.ToString());
            // Debug.Log("loslassen old Ball vel Frame 0 " + ball_vel_frame_m0.ToString());
            //rb.velocity = ball_vel_frame_m1;
            //adapt_velocity_from_invert();
            adapt_velocity_to_old_ball_velocity();
           // rb.velocity = handRB.velocity;
            // Debug.Log("rb velocicty beim loslassen2 " + rb.velocity.ToString());

            // Debug.Log("loslassen old Ball vel Frame 3 " + ball_vel_frame_m3.ToString());
            // Debug.Log("loslassen old Ball vel Frame 2 " + ball_vel_frame_m2.ToString());
            // Debug.Log("loslassen old Ball vel Frame 1 " + ball_vel_frame_m1.ToString());
            // ich verstehe nicht warum beim invert der Ball in die Falsche Richtung geht
            // daher nehme ich die vorzeichen der Ballgeschwindigkeit ein Frame vor dem loslassen
            // die Zahlen ersetze ich dann mit der aktuellen Ballgeschwindigkeit



  //          Vector3 currentPos = transform.position;
   //         transform.position = lastHandPos;
    //        transform.Translate(currentPos);
        }

    }

    

    private void adapt_velocity_to_old_ball_velocity(){
        Vector3 new_vel = new Vector3(1.0f, 1.0f, 1.0f);
        // normiere die alten Geschwindigkeiten auf 1 oder -1 
        //Debug.Log("adapt_velocity_to_old_ball_velocity ball_vel_frame_m1" + ball_vel_frame_m1.ToString());
        //Debug.Log("adapt_velocity_to_old_ball_velocity ball_vel_frame_m0" + ball_vel_frame_m0.ToString());
        //Debug.Log("adapt_velocity_to_old_ball_velocity rb.velocity" + rb.velocity.ToString());
        if (ball_vel_frame_m0 == Vector3.zero){
            Debug.Log("zero");
        }

        if (ball_vel_frame_m0 != Vector3.zero){
            new_vel = ball_vel_frame_m0;
        }else if (ball_vel_frame_m1!=Vector3.zero){
            new_vel = ball_vel_frame_m1;
        }else if (rb.velocity != Vector3.zero){
            new_vel = rb.velocity;
        }
        new_vel[0] = new_vel[0]/Mathf.Abs(new_vel[0]);
        new_vel[1] = new_vel[1]/Mathf.Abs(new_vel[1]);
        new_vel[2] = new_vel[2]/Mathf.Abs(new_vel[2]);

        // multipliziere diese korrekten Vorzeichen nun mit dem Betrag der korrekten Geschwindigkeit
        new_vel[0] = new_vel[0]*Mathf.Abs(rb.velocity[0]);
        new_vel[1] = new_vel[1]*Mathf.Abs(rb.velocity[1]);
        new_vel[2] = new_vel[2]*Mathf.Abs(rb.velocity[2]);

        // if (parameter.current_invert[0]>0){ multiplier[0] = parameter.current_invert[0]*-1.0f; }
        // if (parameter.current_invert[1]>0){ multiplier[1] = parameter.current_invert[1]*-1.0f; }
        // if (parameter.current_invert[2]>0){ multiplier[2] = parameter.current_invert[2]*-1.0f; }
        rb.velocity = new_vel;
//        if (parameter.current_invert[0]>0){ rb.velocity[0] = rb.velocity[0]*-1.0f; }
 //       if (parameter.current_invert[1]>0){ rb.velocity[1] = rb.velocity[1]*-1.0f; }
 //       if (parameter.current_invert[2]>0){ rb.velocity[2] = rb.velocity[2]*-1.0f; }
    }

    private void intitialize_forces_on_ball()
    {
        rb.AddForce(parameter.current_force);
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("OnTriggerEnter with "+ other.tag);
        switch (other.tag)
        {
            case "Ring":
                //print("HHHHHHHHHHHHHHHHHHHHHRingTriggerv XXXX");
                gameSession.set_Hit(ID, 1);
                EventManager.TriggerEvent("Treffer");
                if (!in_destroy_process){
                    in_destroy_process = true;
                    EventManager.TriggerEvent("Destroy");
                    EventManager.TriggerEvent("SpawnNewBall");
                }  
                break;

            
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("XXXXXXXXXXXXXXXXX   Collided with " + col.gameObject.tag);

        switch(col.gameObject.tag)
        {
            case "Ring":
                //print("Ring Collision.............................");
                break;
             case "Ground":
                //print("Ground Collision.............................");
                if (!in_destroy_process){
                    in_destroy_process = true;
                    EventManager.TriggerEvent("Destroy");
                    EventManager.TriggerEvent("SpawnNewBall");
                }            
                break;
            case "Wall":
                // check whether it is in the target radius
                ContactPoint contact = col.GetContact(0);
                float deviation = Vector3.Distance(contact.point, new Vector3(0f,0f,-0.1f))/((wall.transform.localScale.x+wall.transform.localScale.y)/2);
                Debug.Log("Hit with deviation of : " + deviation);
                bool is_hit = false;
                if (deviation<par_target_radius){
                    is_hit = true;
                    Debug.Log("Hit the Wall in Position x =" + transform.position.x + "y=" + transform.position.y + "z="+transform.position.z);
                    gameSession.set_Hit(ID, 1);
                    EventManager.TriggerEvent("Treffer");
                    //Instantiate(trefferPrefab, contact.point, new Quaternion() );
                }
                drawDot.drawTheDot(contact.point, is_hit);
                Debug.Log("deviation = " + deviation + "   vs. par_target_radius = " + par_target_radius);
                //print("HHHHHHHHHHHHHHHHHHHHHRingTriggerv XXXX");
                
                // print("Points colliding: " + col.contacts.Length);
                // print("First point that collided: " + col.contacts[0].point);
                // foreach (ContactPoint contact in col.contacts)
                // {
                //     print(contact.thisCollider.name + " hit " + contact.otherCollider.name + "at point: " + contact.point);
                // }
                if (!in_destroy_process){
                    in_destroy_process = true;
                    EventManager.TriggerEvent("Destroy");
                    EventManager.TriggerEvent("SpawnNewBall");
                }  
                break;

        }
    }

    void OnCollisionExit(Collision col)
    {
        //Debug.Log("XXXXXXXXXXXXXXXXX   Collsion Exit  " + col.gameObject.tag);

        switch(col.gameObject.tag)
        {
            case "Ring":
                //print("Ring Collision.............................");
                break;
             case "Tisch":
//                Debug.Log("Ball was taken from the table");
                break;
        }
    }

    void registerTreffer()
    {
        Debug.Log("HHHHHHHHHHHHHHHHHHHHHHHIIIIIIIIIIIITTTTTT a Hit was registered in the Game Manager");
        myGameManager.register_Hit();
        gameSession.SaveIntoJson();

    }


}


