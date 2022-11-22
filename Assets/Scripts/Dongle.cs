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

    //Ȱ��ȭ �� ������ ȣ��
    private void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    void Update()
    {
        if (isDrag)
        {
            //���콺�� ������ �޾ƿ���
            //��ũ�� ��ǥ�� - ĵ����
            //���� ��ǥ�� - ���� ��
            //��ũ�� ��ǥ�踦 ���� ��ǥ��� �ٲٴ� �Լ� -> ScreenToWorldPoint
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x / 2f;
            float rightBorder = 4.2f - transform.localScale.x / 2f;

            //x,y,z�� ����
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
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);// Lerp -> 0~1 ���� ���� ����
        }
      
    }

    public void Drag()
    {
        isDrag = true;
    }
    public void Drop()
    {
        isDrag = false;
        //�ν����� â������ ���α�
        //�����ۿ��� �ް� �ϱ�
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
            //���� ��ġ�� ���� ����
            if (level == other.level&&!isMerge &&!other.isMerge&&level <7)
            {
                //���� ����� ��ġ ��������
                //���� ��ġ
                float meX = transform.position.x;
                float meY = transform.position.y;
                //������� ��ġ
                float otherX = other.transform.position.x;   
                float otherY = other.transform.position.y;  
                //1.���� �Ʒ��� ���� ��
                //2.������ ������ ��, ���� �����ʿ� ������
                if(meY<otherY || (meY==otherY && meX > otherX))
                {
                    //������ �����
                    other.Hide(transform.position);
                    //���� ������
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
        while(frameCount < 20) //��ġ ������Ʈó�� �۵�
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
        gameManager.score+= (int)Mathf.Pow(2, level); //pow �� float
        gameObject.SetActive(false);
    }
    //���� ������
    void LevelUp()
    {
        isMerge = true;
        //������ �߿� ���ذ� �� �� �ִ� �����ӵ��� �����ϱ�
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
    //��輱 Ż��� ���� �ۼ�
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
        //���� �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        //���� Ʈ������ �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        //���� ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circleCollider.enabled = true;

    }
}
