using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    
    public float moveSpeed;
    public float jumpForce;
    public float gravityScale = 5f;
    
    public float rotateSpeed = 5f;

    private Vector3 moveDirection;

    public  CharacterController charController;
    public  Camera playerCamera;
    public  GameObject playerModel;

    public  Animator animator;

    public bool isKnocking;
    public float knockBackLength = -0.5f;
    private float knockBackCounter;
    public Vector2 knockBackPower;

    public GameObject[] playerPieces;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!isKnocking){
            
            float yStore = moveDirection.y;

            //movimiento
            moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            moveDirection.Normalize();
            moveDirection = moveDirection * moveSpeed;
            moveDirection.y = yStore;


            //salto
            if (charController.isGrounded){
                moveDirection.y = -1f;
                if (Input.GetButtonDown("Jump"))
                {
                    moveDirection.y = jumpForce;
                }
            }
            moveDirection.y += Physics.gravity.y * Time.deltaTime * gravityScale;

            charController.Move(moveDirection * Time.deltaTime);


            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                transform.rotation = Quaternion.Euler(0f, playerCamera.transform.rotation.eulerAngles.y, 0f);
                Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime); 
            }
        }
        if(isKnocking){
            knockBackCounter -= Time.deltaTime;

            float yStore = moveDirection.y;
            moveDirection = transform.forward * knockBackPower.x;
            moveDirection.y = yStore;
            moveDirection.y += Physics.gravity.y * Time.deltaTime * gravityScale;
            charController.Move(moveDirection * Time.deltaTime);

            if (knockBackCounter <= 0)
            {
                isKnocking = false;
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z));
        animator.SetBool("Grounded", charController.isGrounded);
    }
    public void KnockBack(){
        isKnocking = true;
        knockBackCounter = knockBackLength;
        Debug.Log("Knockback");
        moveDirection.y = knockBackPower.y;
        charController.Move(moveDirection * Time.deltaTime);
    }
}
