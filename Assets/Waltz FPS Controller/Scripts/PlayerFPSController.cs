
//Waltz FPS Player Controller Developed by John Ellis, 2018.
//If you need any help, contact me at concerigamesbusiness@gmail.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPSController : MonoBehaviour {

    public static PlayerFPSController current; //Just call PlayerFPSController.current to access the current player's variables from any other script!

    /*
        NOTICE:
        - Player rotation is divided between two separate parts! The camera deals with pitch, and the body deals with yaw. Remember this!
        - Tooltips are there for a reason! Mouse over a variable in the inspector for more information on how to use it!
        - Bugs suck! Report anything you run into and I will get onto it as soon as possible!
        - For player sounds, the FPS Controller needs an AudioManager component attached to the same object! Just use the prefab provided for reference.
    */

    [Header("Controls")]
    [Tooltip("The axis used for strafing. \n\nCheck Edit>Project Settings>Input for more information.")] public string strafeAxis = "Horizontal";
    [Tooltip("The axis used for walking forward. \n\nCheck Edit>Project Settings>Input for more information.")] public string walkAxis = "Vertical";
    [Tooltip("The axis used for looking left and right. \n\nCheck Edit>Project Settings>Input for more information.")] public string lookAxisX = "Mouse X";
    [Tooltip("The axis used for looking up and down. \n\nCheck Edit>Project Settings>Input for more information.")] public string lookAxisY = "Mouse Y";
    [Tooltip("The key for jumping.")] public KeyCode jumpButton = KeyCode.Space;
    [Tooltip("The key for sprinting.")] public KeyCode sprintButton = KeyCode.LeftShift;
    [Tooltip("Changes how sprinting will behave.\n\nNone: Disables sprinting entirely.\n\nNormal: Sprinting engages when the sprint key is held down, and stops when you release.\n\nClassic: If you're walking, pressing the sprint key engages sprintng. Releasing it won't stop sprinting, it's only when you stop walking that sprinting will stop.")] public WaltzSprintSetting sprintMode = WaltzSprintSetting.normal;
    [Tooltip("The key for crouching.")] public KeyCode crouchButton = KeyCode.LeftControl;
    [Tooltip("Changes how crouching will behave.\n\nNormal: Crouching won't affect how sprinting behaves.\n\nNo Sprint: Sprinting cannot be enabled while you're crouching.")] public WaltzCrouchSetting crouchSetting = WaltzCrouchSetting.normal;

    [Header("Movement")]
    [Tooltip("Whether or not the player can walk around, sprint or jump.")] public bool lockMovement = false;
    [Tooltip("0.01 means that the player will control like they're on ice, and 1 means the player moves almost instantaneously in the direction you choose. Your input settings account for friction as well, so check Edit>Project Settings>Input for more information.")] [Range(0.01f, 1f)] public float movementShiftRate = 0.2f;
    [Tooltip("The amount of control a player gets in the air. 0 means absolutely none, 1 means same amount as on the ground.")] [Range(0f, 1f)] public float airControl = 1f;
    [Tooltip("How fast the player is when walking normally.")] public float moveSpeed = 5f;
    [Tooltip("How fast the player moves when sprinting.")] public float sprintSpeed = 8f;
    [Tooltip("The amount of force the player jumps with.")] public float jumpPower = 4f;
    [Tooltip("If this is enabled: sprinting will make your jump height more powerful.")] public bool enhancedJumping = true;
    [Tooltip("The amount of gravity the player experiences.")] public float gravity = 0.1f;
    [Tooltip("Whether or not the player is affected by slopes.")] public bool slidingOnSlopes = true;
    [Tooltip("At 0, the player can basically walk and jump up insanely steep surfaces. At 1, the player slides down surfaces that aren't completely flat and facing up.")] [Range(0f, 1f)] public float slopeBias = 0.7f;
    [Tooltip("A precentage of the player's current height that represents how low they can crouch.")] [Range(0.01f, 1f)] public float crouchPercent = 0.4f;

    [Header("Camera")]
    [Tooltip("Whether or not the player can look around or zoom in.")] public bool lockCamera = false;
    [Tooltip("If this is enabled: Left-Clicking on the screen will lock your cursor in. Pressing Escape unlocks the cursor.")] public bool cursorManagement = true;
    [Tooltip("A direct reference to the player's camera.")] public Camera playerCamera;
    [Tooltip("How sensitive the player's mouse is.")] public float mouseSensitivity = 1.5f;
    [Tooltip("The angles at which the player's camera can look up and down.")] public Vector2 verticalRestraint = new Vector2(-90f, 90f);
    [Space]
    [Tooltip("Whether or not the camera bobs at all.")] public bool enableViewbob;
    [Tooltip("The rate at which the player's camera bobs.")] public float viewBobRate = 1f;
    [Tooltip("The intensity at which the player's camera bobs.")] public float viewBobPower = 1f;
    [Space]
    [Tooltip("Whether or not the camera should respond to landing.")] public bool landingEffects;

    [Header("Zoom")]
    [Tooltip("The FOV the camera will set to when the player sprints. To disable this effect, just set it to the same value as the default FOV!")] public float sprintFOV = 75f;
    [Space]
    [Tooltip("Which mouse button allows the player to zoom in.")] public WaltzMouseSetting zoomButton = WaltzMouseSetting.rightMouse;
    [Tooltip("This value represents how many degrees the zoom changes. The higher this value is, the further in the camera will zoom.")] public float zoomIntensity = 30f;

    [Header("Sounds")]
    [Tooltip("Whether or not sounds will play from the player.")] public bool enableSounds = true;
    [Tooltip("How loud the player's sounds are.")] [Range(0f, 1f)] public float soundVolume = 1f;
    [Tooltip("How long the player will wait before allowing the landing sound to play again. Check m_landingTimer in the code for more details.")] public float landingSoundTimer = 1f;
    [Tooltip("The sound that plays whenever the player walks. The rate this plays at is scaled based on speed.")] public AudioClip[] walkSounds;
    [Tooltip("The sound that plays whenever the player jumps.")] public AudioClip jumpingSound;
    [Tooltip("The sound that plays whenever the player lands on the ground.")] public AudioClip landingSound;

    [Header("Misc.")]
    [Tooltip("Whether or not the game should have its framerate locked.")] public bool lockFramerate = true;
    [Tooltip("The framerate the game will be locked at whenever a player is spawned in. Requires lockFramerate to be active first.")] [Range(1, 60)] public int frameRate = 60;

    [HideInInspector]
    public bool isGrounded = false; //Internal boolean to tell whether or not the player is on the ground. Depends on several conditions seen in the code below.
    [HideInInspector]
    public bool isCrouching = false; //Internal boolean, name is self explanatory.
    [HideInInspector]
    public bool isSprinting; //Whether or not the player is sprinting. 

    /// Private variables.
    private float FOV = 60f; //The default FOV value the camera will be set to. 
    private bool m_stepped = false; //Used per-frame to play walking sounds. Prevents rapid-fire sound being played.
    private bool m_canJump; //Decided based on the angle of the ground below the player.
    private float m_zoomAdditive; //Added onto the current FOV value to decide how far the camera is zoomed in/out.
    private float m_topSpeed; //The maximum amount of speed the player may move at any time. Doesn't include vertical speed.
    private Vector3 m_movement; //The vector3 representing player motion.
    private float m_grav; //The current gravity affecting the player. Increases by the gravity value above every frame.
    private float m_lastGrav; //The gravity as of last frame. Used for a handful of calculations.
    private Vector3 m_goalAngles; //The true angles of the camera. Essentially where the camera will lerp to in some frames.
    private CharacterController m_char; //A reference to the character controller attached to the player object.
    private Vector3 m_camOrigin; //The original local position of the player's camera.
    private Vector3 m_camPosTracer; //Internal vector3 to lerp the camera's position with. Designed to smooth landing effects.
    private float m_landingTimer; //Internal timer to prevent the landing sound from playing 1000 times every time a series of landing events are registered. Comparable to a debouncing script.
    private float m_baseHeight; //The base height of the player. Helps with crouching.
    private float m_camOriginBaseHeight; //Essentially, the best method of keeping the camera from messing up while crouching is to modify the camOrigin's y value if we're crouched or not. This is the base.
    private float m_jumpTimer; //Prevents key bouncing issues;
    ///


    void Start() {

        FOV = playerCamera.fieldOfView;

        /// Initialization details.
        current = this; //The current player controller is assigned so you can access it whenever you need to.

        gameObject.GetComponent<AudioManager>().Init(); //The AudioManager is initialized so that player sounds can be objectpooled.

        m_topSpeed = moveSpeed; //The top speed is set to the default moveSpeed.
        m_char = gameObject.GetComponent<CharacterController>(); //The character controller reference is cached.

        m_goalAngles = playerCamera.gameObject.transform.rotation.eulerAngles; //Goal angles are based on the player camera's rotation.
        m_camOrigin = playerCamera.transform.localPosition; //The camera's origin is cached so bobbing and landing effects can be applied without the camera losing its default position.
        m_camOriginBaseHeight = m_camOrigin.y;
        m_camPosTracer = m_camOrigin; //This is where the camera truly lies in a given frame.

        Cursor.lockState = CursorLockMode.Locked; //Cursor is locked.
        Cursor.visible = false; //Cursor is hidden.
                                ///

        ///Framerate locking, if you so please.
        if (lockFramerate)
        {
            if (frameRate < 60) //If the framerate is below sixty, we have to disable V-Sync to forcibly change the framerate.
            {
                QualitySettings.vSyncCount = 0;
            }

            Application.targetFrameRate = frameRate; //Framerate is set at the value you specified above.
        }
        ///

        m_baseHeight = m_char.height;

    }

    void Update() {

        if (!Physics.SphereCast(new Ray(transform.position, Vector3.up), m_char.radius, m_baseHeight * 0.7f))
        {
            isCrouching = Input.GetKey(crouchButton); //Check if the player wants to crouch.

            if (lockMovement)
                isCrouching = false; //If our movement is locked, we can prevent crouching.
        }
        else
        {
            if (Input.GetKeyDown(crouchButton))
                isCrouching = true;

            if (Mathf.Abs(m_char.height - m_baseHeight) > 0.05f)
                isCrouching = true;
        }

        // MOVEMENT MANAGEMENT

        ///Crouch Logic
        if (isCrouching)
        {
            transform.position -= Vector3.up * Mathf.Abs((m_baseHeight * crouchPercent) - m_char.height) * Time.deltaTime * 2f;
            m_char.height += ((m_baseHeight * crouchPercent) - m_char.height) * 0.1f;
        }
        else
        {
            transform.position += Vector3.up * Mathf.Abs(m_baseHeight - m_char.height) * Time.deltaTime * 5f;
            m_char.height += (m_baseHeight - m_char.height) * 0.1f;
        }

        /// Sprinting. Code for managing the camera's FOV, the methods sprinting can be enabled, and the player's top speed.
        switch (sprintMode)
        {
            case (WaltzSprintSetting.normal):
                isSprinting = Input.GetKey(sprintButton);
                break;

            case (WaltzSprintSetting.classic):
                if (Input.GetKey(sprintButton))
                    isSprinting = true;
                if ((Mathf.Abs(Input.GetAxis(strafeAxis)) + Mathf.Abs(Input.GetAxis(walkAxis))) * 0.5f < 0.5f) //Player's intended movement is averaged on intensity and analyzed. If it falls below a threshold, sprinting turns off.
                    isSprinting = false;
                break;
        }

        if (crouchSetting == WaltzCrouchSetting.noSprint)
            isSprinting = false;

        if (isSprinting) { //If we're sprinting, lerp the top speed as well as the FOV to reflect that.
            m_topSpeed += (sprintSpeed - m_topSpeed) * 0.2f;
            playerCamera.fieldOfView += ((sprintFOV + m_zoomAdditive) - playerCamera.fieldOfView) * 0.2f;
        } else { //Otherwise put it back to normal!
            playerCamera.fieldOfView += ((FOV + m_zoomAdditive) - playerCamera.fieldOfView) * 0.2f;
            m_topSpeed += (moveSpeed - m_topSpeed) * 0.2f;
        }
        ///

        /// General Motion.
        if (!lockMovement) //If the movement isn't locked, manage player controls to figure out where they want to go.
        {


            if (isGrounded) //Friction is decided based on whether or not the player is on the ground. Your general friction can be changed with movementShiftRate, available now in a local inspector near you.
                m_movement += (gameObject.transform.TransformDirection(new Vector3(Input.GetAxis(strafeAxis) * m_topSpeed, 0f, Input.GetAxis(walkAxis) * m_topSpeed)) - m_movement) * movementShiftRate;
            else
                m_movement += (gameObject.transform.TransformDirection(new Vector3(Input.GetAxis(strafeAxis) * m_topSpeed, 0f, Input.GetAxis(walkAxis) * m_topSpeed)) - m_movement) * movementShiftRate * airControl;
        }
        else
            m_movement += (Vector3.zero - m_movement) * movementShiftRate; //Assuming movement's locked, the player is recursively slowed down to zero.
        ///

        m_movement.y = m_grav; //The player's current gravity is applied to the movement vector. This is done because CharacterControllers take control over the y value, and managing gravity in a different variable and reassigning gives us the control instead.

        isGrounded = (m_char.collisionFlags & CollisionFlags.CollidedBelow) != 0; //Using the character controller's built-in ground check, a simple examination is made! Below has more stuff related to this.

        /// This snaps the player down to a surface if the conditions are just right. Needed on slopes to prevent the player from sliding off of a surface and floating down to the ground instead of, you know, walking down the slope like a normal human.
        float snapDistance = (GetComponent<CharacterController>().height * 0.5f) + 0.5f; //How far down the player will search for snappable terrain.
        if (!isGrounded && m_grav < 0f) //If we're midair and we're also falling down.
        {

            RaycastHit hitInfo;

            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, snapDistance)) //A raycast is fired below the player.
            {
                if (!hitInfo.collider.isTrigger) //If the found object's collider ISN'T a trigger (prevents the player controller from snapping and grounding to trigger volumes).
                {
                    if (hitInfo.normal.y >= slopeBias) //If the surface is flat enough, 
                        m_char.Move(((hitInfo.point + (Vector3.up * m_char.height * 0.5f)) - transform.position) * 0.1f); //The player is shifted down to the surface for landing.
                    isGrounded = true;
                }
            }

        }
        ///

        /// Ceiling bumping. Prevents that obnoxious "sticky" feel you get whenever you jump into something above you.
        if ((m_char.collisionFlags & CollisionFlags.CollidedAbove) != 0) //If the collision flags for this frame contain info for a collision above, the code is run.
        {
            if (m_grav > 0f) m_grav = 0f; //If we're going up, clamp our vertical speed to zero!
        }
        ///


        m_grav -= gravity; //Gravity controls.


        if (m_jumpTimer > 0f)
            m_jumpTimer -= Time.deltaTime;

        /// Jumping management.
        if (!isGrounded)
        {
            if (!lockMovement)
            {
                if (m_grav > 0f && Input.GetKeyUp(jumpButton)) //Whenever the player is in midair and going up, we allow them to halve their vertical speed by releasing the jump button.
                    m_grav -= m_grav * 0.5f;
            }

        }
        else
        {

            if (!lockMovement)
            {
                if (m_canJump)
                {
                    if (Input.GetKeyDown(jumpButton))
                    {
                        if (m_jumpTimer <= 0f)
                        {
                            if (enableSounds)
                                AudioManager.PlaySound(jumpingSound, soundVolume);

                            if (enhancedJumping)
                                m_grav += jumpPower + ((isSprinting) ? jumpPower * 0.15f : 0f); //Jumppower is scaled up if enhanced jumping is enabled.
                            else
                                m_grav += jumpPower; //No matter what, you will always jump with consistent power when enhancedJumping is disabled.

                            m_jumpTimer = 0.1f;
                        }
                    }
                }
            }


            if (m_canJump)
                m_grav = Mathf.Clamp(m_grav, -1f, 1000f); //If the surface below us is flat enough to count as ground, we clamp the player's gravity to prevent it from stacking up and making the player fall through terrain!

        }
        ///

        // CAMERA MANAGEMENT

        m_camOrigin.y = m_camOriginBaseHeight * (m_char.height / m_baseHeight);

        /// Cursor Management. If you feel like you want to take control over the cursor more, you can disable cursorManagement in the inspector.
        if (cursorManagement)
        {
            if (Input.GetMouseButtonDown(0)) //Here we lock the mouse whenever the player clicks the screen.
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape)) //Press escape to unlock the cursor.
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        ///

        if (!lockCamera)
        {

            /// Controls landing effects (if enabled). Uses m_lastGrav to compare against the player's previous gravity and figure out if they've landed.
            if (m_landingTimer > 0f)
            {
                if (!isGrounded)
                    m_landingTimer -= Time.deltaTime; //If we're not on the ground, we count the timer down for landing effects! This makes it so that small drops don't cause the landing sound to play.
                else
                    m_landingTimer = landingSoundTimer; //The timer is reset if we hit the ground.
            }

            if (m_grav - m_lastGrav > 5f && m_lastGrav < -5f) //This checks to make sure the player is falling a certain speed before using landing effects.
            {
                if (enableSounds)
                {
                    if (m_landingTimer <= 0f)
                    {
                        AudioManager.PlaySound(landingSound, soundVolume); //If we're allowed to play sounds on landing, we do it here. The timer is reset to prevent spamming.
                        m_landingTimer = landingSoundTimer;
                    }
                }

                if (landingEffects)
                {
                    m_camPosTracer -= Vector3.up * -m_lastGrav * 0.3f; //If effects are enabled, the camera is bowed down for the landing.
                    m_camPosTracer.y = Mathf.Clamp(m_camPosTracer.y, -4f, 4f); //This effect is clamped to prevent the camera from bowing too low.
                }
            }
            ///

            /// Dedicated to controlling viewbobbing and walkSounds.
            if (!lockMovement)
            {
                if (isGrounded)
                {
                    if (new Vector3(Input.GetAxis(strafeAxis), 0f, Input.GetAxis(walkAxis)).magnitude > 0.3f)
                    {
                        /// Viewbob effects and management for the m_walkTime value. Scales relative to your current speed and however fast you normally move, meaning a low moveSpeed and high sprintSpeed result in quick footsteps.
                        float m_walkTime = 0f;
                        m_walkTime += viewBobRate * Time.time * ((isSprinting) ? (10f + (sprintSpeed / moveSpeed) * 2f) : 10f); //m_walkTime controls the viewbob's rate! It's scaled depending on speed.

                        if (enableViewbob)
                            m_camPosTracer += (m_camOrigin + (Vector3.up * viewBobPower * Mathf.Sin(m_walkTime) * ((isSprinting) ? 0.15f : 0.1f)) - m_camPosTracer) * 0.4f; //Bobbing effects.
                        else
                            m_camPosTracer += (m_camOrigin - m_camPosTracer) * 0.4f; //The camera is lerped to its default position if viewbobbing is disabled.
                        ///


                        /// This section is dedicated to step sounds, which play on the lowest part of the camera's viewbob curve. Disabling viewbob doesn't affect the m_walkTime value!
                        if (enableSounds)
                        {
                            if (Mathf.Sin(m_walkTime) < -0.8f)
                            {
                                if (!m_stepped)
                                {
                                    AudioManager.PlaySound(walkSounds[Random.Range(0, walkSounds.Length)], soundVolume);
                                    m_stepped = true;
                                }
                            }
                            else
                            {
                                m_stepped = false;
                            }
                        }
                        ///
                    }
                    else
                    {
                        m_camPosTracer += (m_camOrigin - m_camPosTracer) * 0.4f; //If the player isn't moving fast enough to be constituted as "walking", the camera returns to normal.
                    }
                }
                else
                {
                    m_camPosTracer += (m_camOrigin - m_camPosTracer) * 0.4f; //If the player isn't on the ground, the camera goes back to its default position.
                }
            }
            ///


            /// Various outputs are assigned. The individual comments explain this in detail.
            playerCamera.transform.localPosition += (m_camPosTracer - playerCamera.transform.localPosition) * 0.1f; //The position of the camera is shifted as needed.

            m_goalAngles += new Vector3(-Input.GetAxis(lookAxisY) * mouseSensitivity, Input.GetAxis(lookAxisX) * mouseSensitivity); //The current motion of the mouse is taken in, multiplied by the mouse sensitivity, and then added onto the goal camera angles.

            m_goalAngles.x = Mathf.Clamp(m_goalAngles.x, verticalRestraint.x, verticalRestraint.y); //The camera angles are restricted here, so the player can't flip their head completely down and snap their neck.

            gameObject.transform.rotation = Quaternion.Euler(0f, m_goalAngles.y, 0f); //The horizontal rotation is applied to the player's body.
            playerCamera.transform.rotation = Quaternion.Euler(m_goalAngles); //The vertical rotation is applied to the player's head.
            ///
        }


        /// Zoom Feature. Fairly straightforward, if the zoomButton isn't assigned to the none slot and we're pressing it, we zoom in. Otherwise we zoom out. Controlled with a lil' recursive linear interpolation formula.

        if (!lockCamera)
        {
            if ((int)zoomButton != 5)
            {
                if (Input.GetMouseButton((int)zoomButton))
                {
                    m_zoomAdditive += (-zoomIntensity - m_zoomAdditive) * 0.2f;
                }
                else
                {
                    m_zoomAdditive -= (m_zoomAdditive * 0.2f);
                }
            }
        }
        else
            m_zoomAdditive -= (m_zoomAdditive * 0.2f);
        ///

        m_lastGrav = m_grav; //The gravity from the last frame is set here. This is mostly used to compare against prior frame motion.

    }

    void FixedUpdate()
    {

        m_char.Move(m_movement * Time.deltaTime); //Movement is applied here. Motion is scaled on framerate to smooth things out.

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        /// This entire section focuses on preventing the player from climbing or jumping up slopes. While sliding can be disabled, for the sake of gameplay, you cannot change settings that prevent the player from jumping up hills. If it's too steep, it ain't happening.
        if (slidingOnSlopes)
        {
            if (hit.normal.y <= slopeBias) //If sliding on slopes is enabled, we check to see if the current surface is too steep. If so, the player can no longer jump, and they are shunted down the slope. If the slope isn't too steep, the player is then allowed to jump.
            {
                m_canJump = false;
                Vector3 m_pos = hit.normal;
                m_pos.y = -0.9f;
                m_movement += m_pos * 0.3f;
            }
            else
            {
                m_canJump = true;
            }
        }
        else
        {
            if (hit.normal.y > slopeBias)
            {
                m_canJump = true;
            }
        }
        ///

    }

    public enum WaltzMouseSetting { //Enumerator for mouse buttons.

        leftMouse = 0,
        rightMouse = 1,
        middleMouse = 2,
        extraMouse1 = 3,
        extraMouse2 = 4,
        none = 5

    }

    public enum WaltzSprintSetting { //Changes how sprint behaves.

        none = 0, //Self explanatory.
        normal = 1, //Hold sprint to sprint. When released, you will return to normal speed.
        classic = 2 //While you're running, pressing the sprint key will engage sprinting. Sprinting will only stop when you do.

    }

    public enum WaltzCrouchSetting { //Changes how crouching behaves.
        normal = 0, //Crouching doesn't affect whether or not the player can sprint. Get a move on!
        noSprint = 1 //Crouching disables the ability to sprint. Good if you're practical or whatever.
    }

}
