    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;


public class PlayerController : MonoBehaviour
    {
    
        public float jumpForce = 6f, turnSpeed = 80f;
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
        public float playerHealth = 2f;
        public bool isAttack = false;
        private EnemyController enemyController;
        public float lastHitTime, ignoreHitTime = 2.5f;

        [SerializeField]
        private Joystick leftJoystick, rightJoystick;
        private bool keyBoardControl = true;
        private GameObject androidUI;

        public Slider hpSlider;
        private GameObject losePanel;

        private float lastBlockTime, blockDelay = 2f;
        private bool isBlock = false;

        public float vampirismHeal = 0f;
        public float critChance = 0f;
        public float critMultiplier = 1.8f;
        public float regenAmount = 0f;     
        public float regenInterval = 2f;    
        private float regenTimer = 0f;

        public int upgradePoints = 0;
        public float animationSpeed = 1f;

        public int maxHpLevel = 0;


        private Boosts boosts;




        [System.Serializable]
        public class UpgradeBind
        {
            public KeyCode key;
            public string statName;
        }
        [SerializeField]
        public List<UpgradeBind> upgradeBinds = new List<UpgradeBind>
    {
        new UpgradeBind { key = KeyCode.Alpha1, statName = "Speed" },
        new UpgradeBind { key = KeyCode.Alpha2, statName = "Strength" },
        new UpgradeBind { key = KeyCode.Alpha3, statName = "Regeneration" },
        new UpgradeBind { key = KeyCode.Alpha4, statName = "Max-hp" },
        new UpgradeBind { key = KeyCode.Alpha5, statName = "Armor" },
        new UpgradeBind { key = KeyCode.Alpha6, statName = "Vampirism" },
        new UpgradeBind { key = KeyCode.Alpha7, statName = "Crits" }
    };

        private List<string> upgrades = new List<string>
    {
        "Speed",
        "Strength",
        "Regeneration",
        "Max-hp",
        "Armor",
        "Vampirism",
        "Crits"
    };



        private void Awake()
        {
            losePanel = GameObject.Find("PausePanel");
        }

        void Start()
        {
            boosts = FindObjectOfType<Boosts>();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boosts = FindObjectOfType<Boosts>();

        hpSlider = GameObject.Find("Slider").GetComponent<Slider>();
        hpSlider.maxValue = playerHealth;
        hpSlider.value = playerHealth;

        ApplyAllUpgrades();


        hpSlider = GameObject.Find("Slider").GetComponent<Slider>();
            hpSlider.maxValue = playerHealth;
            hpSlider.value = playerHealth;

            androidUI = GameObject.Find("MobileUI");
            keyBoardControl = PlayerPrefs.GetInt("Android") == 0;
            androidUI.SetActive(!keyBoardControl);

      
            if (!leftJoystick)
            {
                leftJoystick = FindObjectOfType<FloatingJoystick>();
                if (leftJoystick == null)
                    Debug.LogWarning("❗ FloatingJoystick не найден в сцене");
            }

            if (!rightJoystick)
            {
                rightJoystick = FindObjectOfType<VariableJoystick>();
                if (rightJoystick == null)
                    Debug.LogWarning("❗ VariableJoystick не найден в сцене");
            }

            if (keyBoardControl)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            enemyController = GameObject.Find("Enemy").GetComponent<EnemyController>();
            Random.InitState((int)(Time.deltaTime * 1000000));
        }


        private void FixedUpdate()
        {
            SetDirection(keyBoardControl);
            transform.Rotate(new Vector3(0f, angleY, 0f));
        }

        private void SetDirection(bool isKeyboard)
        {
            angleY = (isKeyboard ? Input.GetAxis("Mouse X") : rightJoystick.Horizontal) * turnSpeed * Time.fixedDeltaTime;
            dirZ = isKeyboard ? Input.GetAxis("Vertical") : leftJoystick.Vertical;
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

            if (Input.GetMouseButtonDown(0) && keyBoardControl)
                Attack();

            if (isEquiped && Time.time > lastAttackTime + attackWindow)
                animator.Play("Sword_Holster");

            isBlock = animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Right_Shield") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Left3") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_Left2") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Block_RightDown1");

            if (Input.GetMouseButtonDown(1) && Time.time > lastBlockTime + blockDelay)
            {
                lastBlockTime = Time.time;
                animator.Play(Random.Range(0, 4) switch
                {
                    0 => "Sword_Block_Right_Shield",
                    1 => "Sword_Block_Left3",
                    2 => "Sword_Block_Left2",
                    _ => "Sword_Block_RightDown1"
                });
            }

            RegenerateHealth();
            CheckUpgradeInput();
            animator.speed = animationSpeed;

            if (isBlock)
            {
                animator.applyRootMotion = false;
            }



        }

        public void EquipSword()
        {
            localSwordPosition = sword.transform.localPosition;
            localSwordRotation = sword.transform.localRotation;
            sword.transform.SetParent(transform.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand"));
            isEquiped = true;
        }

        public void UnEquipSword()
        {
            sword.transform.SetParent(transform.Find("Root/Hips"));
            sword.transform.localPosition = localSwordPosition;
            sword.transform.localRotation = localSwordRotation;
            isEquiped = false;
        }


        public void Attack()
        {
            float damage = 1f;

            bool isCrit = Random.value < critChance;
            if (isCrit)
            {
                damage *= critMultiplier;
                Debug.Log("💥 Критический удар! Урон: " + damage);
            }

            if (!isEquiped)
            {
                animator.Play("Sword_Equip");
                lastAttackTime = Time.time;
            }
            else
            {
                animator.Play("Sword_Attack_R", 0, 0);
                lastAttackTime = Time.time;

                if (enemyController && Vector3.Distance(transform.position, enemyController.transform.position) < 5f)
                {
                    enemyController.TakeDamage(damage);

               
                }
            }
        }




        public void HealPlayer(float amount)
        {
            playerHealth = Mathf.Min(playerHealth + amount, hpSlider.maxValue);
            hpSlider.value = playerHealth;
        }




        void JumpController(bool isKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isKeyboard)
                Jump();
            else
                animator.SetTrigger("IsGrounded");
        }

        void Move(float dir, string paramName, string altParamName, bool isKeyboard)
        {
            float threshold = isKeyboard ? 0 : 0.3f;
            animator.SetBool(paramName, dir > threshold);
            animator.SetBool(altParamName, dir < -threshold);
        }

        public void Dodge(bool isKeyboard)
        {
            if (!isKeyboard)
            {
                if (leftJoystick == null) return;

                if (leftJoystick.Horizontal < -0.8f)
                {
                    animator.Play("Sword_Dodgle_Left");
                }
                else if (leftJoystick.Horizontal > 0.8f)
                {
                    animator.Play("Sword_Dodge_Right");
                }
            }
            else
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


        void Sprint(bool isKeyboard)
        {
            animator.SetBool("IsRun", isKeyboard ? Input.GetKey(KeyCode.LeftShift) : Mathf.Abs(leftJoystick.Vertical) > 0.9f);
        }

        void Jump()
        {
            if (!isGrounded) return;

            animator.Play("Sword_Jump_Platformer_Start");
            animator.applyRootMotion = false;
            jumpDir = transform.TransformDirection(new Vector3(0f, jumpForce, dirZ * jumpForce / 2));
            rb.AddForce(jumpDir, ForceMode.Impulse);
            isGrounded = false;
        }

        void MoveInAir()
        {
            if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude < 1.1f)
                rb.velocity = transform.TransformDirection(new Vector3(0f, rb.velocity.y, dirZ));
        }

        void OnCollisionEnter(Collision collision)
        {
            isGrounded = true;
            animator.applyRootMotion = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EnemySword") && !isBlock && enemyController.isAttack && Time.time > lastHitTime + ignoreHitTime)
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

        private void RegenerateHealth()
        {
            if (regenAmount <= 0f) return;

            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                regenTimer = 0f;
                HealPlayer(regenAmount);
                Debug.Log("🧬 Регенерация: +" + regenAmount + " HP");
            }
        }


    public void UpgradeStat(string statName)
    {
        if (upgradePoints <= 0)
        {
            Debug.Log("❌ Нет очков для прокачки!");
            return;
        }

        upgradePoints--;
        Debug.Log("✅ Прокачка применена: " + statName);

        switch (statName)
        {
            case "Speed":
                animationSpeed += 0.1f;
                Debug.Log("📈 Скорость увеличена: " + animationSpeed);
                break;

            case "Strength":
                jumpForce += 2f;
                Debug.Log("💪 Прыжок усилен: " + jumpForce);
                break;

            case "Regeneration":
                regenAmount += 0.2f;
                Debug.Log("🧬 Регенерация + " + regenAmount + " HP каждые 2 сек.");
                break;

            case "Max-hp":
                maxHpLevel++;
                float hpGain = 1f + (maxHpLevel - 1) * 1.5f;
                playerHealth += hpGain;
                hpSlider.maxValue = playerHealth;
                hpSlider.value = playerHealth;
                Debug.Log($"❤️ Max HP прокачано: уровень {maxHpLevel} | +{hpGain} HP | Всего: {playerHealth}");
                break;

            case "Armor":
                ignoreHitTime = Mathf.Max(0.1f, ignoreHitTime - 0.1f);
                Debug.Log("🛡️ Защита усилена. Время между ударами: " + ignoreHitTime);
                break;

            case "Vampirism":
                vampirismHeal += 0.1f;
                Debug.Log("🧛 Вампиризм: хил за удар = " + vampirismHeal);
                break;

            case "Crits":
                critChance += 0.09f;
                Debug.Log("🎯 Шанс крита: " + (critChance * 100).ToString("F1") + "%");
                break;
        }

        FindObjectOfType<Boosts>()?.IncrementLevel(statName);
    }




    public void AddUpgradePoint()
        {
            upgradePoints++;
            Debug.Log("🪙 Очко улучшения добавлено! Всего: " + upgradePoints);
        }


        void CheckUpgradeInput()
        {
            if (upgradePoints <= 0) return;

            foreach (var bind in upgradeBinds)
            {
                if (Input.GetKeyDown(bind.key))
                {
                    UpgradeStat(bind.statName);
                    break;
                }
            }
        }

    //public void AddAdminPoints()
    //{
    //    upgradePoints += 1;
    //    Debug.Log($"🪙 Админ добавил себе поинты! Теперь: {upgradePoints}");
    //}
    public void ResetStats()
    {
        playerHealth = 2f;
        vampirismHeal = 0f;
        critChance = 0f;
        regenAmount = 0f;
        animationSpeed = 1f;
        maxHpLevel = 0;

        hpSlider.maxValue = playerHealth;
        hpSlider.value = playerHealth;

        Debug.Log("🧬 Характеристики гравця скинуті");
    }

    private void ApplyAllUpgrades()
    {
        boosts.ApplyAllUpgradesToPlayer();
    }

}
