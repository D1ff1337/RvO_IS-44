# project_for_proga
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 6f,
         turnSpeed = 80f;
    private float angleY, dirZ;
    private bool isGrounded = true;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 jumpDir;

    public GameObject sword;
    private Vector3 localSwordPosition;
    private Quaternion localSwordRotation;
    private bool isEquiped;
    private float lastAttackTime;
    private float attackWindow = 5f;

    public int playerHealth = 2;
    public bool isAttack = false;
    private EnemyController enemyController;

    private float lastHitTime, ignoreHitTime = 2.5f;

    [SerializeField]
    
    private bool keyBoardControl = true;
    private GameObject androidUI;

    private Slider hpSlider;
    private GameObject losePanel;

    private float lastBlockTime, blockDelay = 2f;

    private bool isBlock = false;

    private void Awake()
    {
        losePanel = GameObject.Find("PausePanel");
    }
    void Start()
    {

        hpSlider = GameObject.Find("Slider").GetComponent<Slider>();

        hpSlider.maxValue = playerHealth;
        hpSlider.value = playerHealth;

        androidUI = GameObject.Find("MobileUI");
        int androidControl = PlayerPrefs.GetInt("Android");
        if (androidControl == 0)
        {
            keyBoardControl = true;
            androidUI.SetActive(false);
        }
        else
        {
            keyBoardControl = false;
            androidUI.SetActive(true);
        }
        leftJoystick = FindObjectOfType<FloatingJoystick>();
        rightJoystick = FindObjectOfType<VariableJoystick>();   


        if (keyBoardControl)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        enemyController = GameObject.Find("Enemy").GetComponent<EnemyController>();
        Random.seed = (int)(Time.deltaTime) * 1000000;
    }
    private void FixedUpdate()
    {

    }
    private void SetDirection(bool isKeyboard)
    {
        if(isKeyboard)
        {
            angleY = Input.GetAxis("Mouse X") * turnSpeed * Time.fixedDeltaTime;
            dirZ = Input.GetAxis("Vertical");
        }
        else
        {
            angleY = rightJoystick.Horizontal * turnSpeed * Time.fixedDeltaTime;
            dirZ = leftJoystick.Vertical;
        }
    }
void Update()
    {
        isAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Attack_R");

        if (isGrounded)
        {
            JumpController(keyBoardControl);
            Move(dirZ, "IsWalkForward", "IsWalkBack", keyBoardControl);
            Sprint(keyBoardControl);
            Dodge(keyBoardControl);
        }
        else
        {
            MoveInAir();
        }
        if (Input.GetMouseButtonDown(0) && keyBoardControl) {
            Attack();
        }
        if (isEquiped && Time.time > lastAttackTime + attackWindow)
        {
            animator.Play("Sword_Holster");
        }











        isBlock = animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Right_Shield") ||
                  animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Left3") ||
                  animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Left2") ||
                  animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_RightDown1");

        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time > lastBlockTime + blockDelay)
            {
                lastBlockTime = Time.time;
                int randomNum = Random.Range(0, 4);
                Debug.Log(randomNum);
                switch (randomNum)
                {
                    case 0:
                        animator.Play("Sword_Block_Right_Shield");
                        break;
                    case 1:
                        animator.Play("Sword_Block_Left3");
                        break;
                    case 2:
                        animator.Play("Sword_Block_Left2");
                        break;
                    case 3:
                        animator.Play("Sword_Block_RightDown1");
                        break;
                }
            }
        }

    }
    public void Attack()
    {
        if (!isEquiped)
        {
            animator.Play("Sword_Equip");
            lastAttackTime = Time.time;
        }
        else
        {
            animator.Play("Sword_Attack_R");
            lastAttackTime = Time.time;
        }
    }
    public void JumpController(bool isKeyBoard)
    {
        if (Input.GetKeyDown(KeyCode.Space) && isKeyBoard)
        {
            Jump();
        }
        else
        {
            animator.SetTrigger("IsGrounded");
        }
    }
    public void Move(float dir, string paramName, string altParamName, bool isKeyboard)
    {
        float param = 0;
        if(!isKeyboard)
        {
            param = 0.3f;
        }
        if (dir > 0)
        {
            animator.SetBool(paramName, true);
        }
        else if (dir <  0)
        {
            animator.SetBool(altParamName, true);
        }
        else
        {
            animator.SetBool(paramName, false);
            animator.SetBool(altParamName, false);
        }
    }
    public void Dodge(bool isKeyboard)
    {
        if (isKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                animator.Play("Sword_Dodgle_Left");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                animator.Play("Sword_Dodge_Right");
            }
        }
      
    }
    private void Sprint(bool isKeyboard)
    {
       
    }
    public void Jump()
    {
        if (isGrounded)
        {
      
        }
    }
    public void MoveInAir()
    {
        float dir = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        if (dir < 1.1f)
        {
            jumpDir = new Vector3(0f, rb.velocity.y, dirZ);
            jumpDir = transform.TransformDirection(jumpDir);
            rb.velocity = jumpDir;
        }
    }

    public void EquipSword()
    {
        localSwordPosition = sword.transform.localPosition;
        localSwordRotation = sword.transform.localRotation;
        sword.transform.SetParent(transform.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand").transform);
        isEquiped = true;
    }
    public void UnEquipSword() {
        sword.transform.SetParent(transform.Find("Root/Hips").transform);
        sword.transform.localPosition = localSwordPosition;
        sword.transform.localRotation = localSwordRotation;
        isEquiped = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        animator.applyRootMotion = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword") && enemyController.isAttack && Time.time > lastHitTime + ignoreHitTime && !isBlock)
        {
            lastHitTime = Time.time;
            playerHealth--;
            hpSlider.value = playerHealth;
            animator.Play("Sword_Hit_L_2");

            if (playerHealth <= 0)
            {
                losePanel.SetActive(true);
                GameObject.Find("TitleText").GetComponent<Text>().text = "Game Over";
                Time.timeScale = 0f;
            }
        }
    }
}

