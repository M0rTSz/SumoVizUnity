using UnityEngine;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{

    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    /* Changes by Daniel Büchele:
	 * - changed 'end' key to 'escape' or 'tab' key (use escape or tab to toggle cursor locking)
	 */

    /* Changes by Florian Sesser:
	 * - Update to Unity 5 APIs
	 * - Do not capture cursor on start (since it does not work properly in dev mode, only in the built game)
	 */

    public float cameraSensitivity = 90.0f;
    public float climbSpeed = 4.0f;
    public float normalMoveSpeed = 10.0f;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;


    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.Translate((normalMoveSpeed * fastMoveFactor) * Input.GetAxisRaw("Horizontal") * Time.deltaTime,
                0, (normalMoveSpeed * fastMoveFactor) * Input.GetAxisRaw("Vertical") * Time.deltaTime, Space.Self);
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.Translate((normalMoveSpeed * slowMoveFactor) * Input.GetAxisRaw("Horizontal") * Time.deltaTime,
                0, (normalMoveSpeed * slowMoveFactor) * Input.GetAxisRaw("Vertical") * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.Translate(normalMoveSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime,
                0, normalMoveSpeed * Input.GetAxisRaw("Vertical") * Time.deltaTime, Space.Self);
        }

        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }

        // set light always at the same position as camera
        //GameObject.Find("LightSource").transform.position = transform.position;
        //GameObject.Find("LightSource").transform.rotation = transform.rotation;
        //if (transform.localPosition.y < 0)
        //transform.localPosition = new Vector3 (transform.localPosition.x,0.3f,transform.localPosition.z);
    }
}
