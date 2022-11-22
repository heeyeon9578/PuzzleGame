using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public int level;
    public bool isDrag = false;
    public bool isMerge =false;
    public bool isAttach = false;
    public Rigidbody2D rigid;
    Animator anim;
    CircleCollider2D circleCollider;    
    public GameManager gameManager;
    public ParticleSystem effect;
    SpriteRenderer spriteRenderer;
    float deadTime;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();  
        anim = GetComponent<Animator>();   
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    //활성화 할 때마다 호출
    private void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    void Update()
    {
        if (isDrag)
        {
            //마우스의 포지션 받아오기
            //스크린 좌표계 - 캔버스
            //월드 좌표계 - 게임 속
            //스크린 좌표계를 월드 좌표계로 바꾸는 함수 -> ScreenToWorldPoint
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x / 2f;
            float rightBorder = 4.2f - transform.localScale.x / 2f;

            //x,y,z축 고정
            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }
            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);// Lerp -> 0~1 사이 값만 가능
        }
      
    }

    public void Drag()
    {
        isDrag = true;
    }
    public void Drop()
    {
        isDrag = false;
        //인스펙터 창에서는 꺼두기
        //물리작용을 받게 하기
        rigid.simulated = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameManager.SfxPlay(GameManager.Sfx.Attach);
        StartCoroutine(AttachRoutine());
    }
    IEnumerator AttachRoutine()
    {
        if (isAttach)
        {
            yield break;
        }
        isAttach = true;
        yield return new WaitForSeconds(0.2f);
        isAttach = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();
            //동글 합치기 로직 실행
            if (level == other.level&&!isMerge &&!other.isMerge&&level <7)
            {
                //나와 상대편 위치 가져오기
                //나의 위치
                float meX = transform.position.x;
                float meY = transform.position.y;
                //상대편의 위치
                float otherX = other.transform.position.x;   
                float otherY = other.transform.position.y;  
                //1.내가 아래에 있을 때
                //2.동일한 높이일 때, 내가 오른쪽에 있을때
                if(meY<otherY || (meY==otherY && meX > otherX))
                {
                    //상대방은 숨기기
                    other.Hide(transform.position);
                    //나는 레벨업
                    LevelUp();
                }
            }
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circleCollider.enabled = false;
        if(targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }
        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while(frameCount < 20) //마치 업데이트처럼 작동
        {
            frameCount++;
            if(targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);

            }else if(targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);

            }
            
            yield return null;

        }
        isMerge = false;
        gameManager.score+= (int)Mathf.Pow(2, level); //pow 는 float
        gameObject.SetActive(false);
    }
    //나는 레벨업
    void LevelUp()
    {
        isMerge = true;
        //레벨업 중에 방해가 될 수 있는 물리속도를 제거하기
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        StartCoroutine(LevelUpRoution());

    }

    IEnumerator LevelUpRoution()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("Level", level + 1);
        EffectPlay();
        gameManager.SfxPlay(GameManager.Sfx.LevelUp);
        yield return new WaitForSeconds(0.3f);
        level++;    
        gameManager.maxLevel =Mathf.Max(gameManager.maxLevel,level);    
        isMerge = false;    

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;
            if (deadTime > 2)
            {
                //spriteRenderer.color = Color.red; 
                spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
            }

            if (deadTime > 5)
            {
                
                gameManager.GameOver();
            }
        }
    }
    //경계선 탈출시 로직 작성
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;

        }
    }
    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

    private void OnDisable()
    {
        //동글 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        //동글 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        //동글 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circleCollider.enabled = true;

    }
}
