using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

    public GameObject effectPrefab;
    public Transform effectGroup;

    //������Ʈ Ǯ��
    public List<Dongle> donglePool;
    public List<ParticleSystem> effectPool;
    [Range(0,30)]
    public int poolSize;
    public int poolCursor;


    //���ھ�, ���ӿ���
    public int maxLevel;
    public int score;
    public bool isOver;
    

    //����
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, Over};
    int sfxCursor;
    private void Awake()
    {
        //�ν������� interpolate: ���� �������� ���Ͽ� �������� �ε巴�� ����
        //�������� �����ϰ� �ϱ�
        Application.targetFrameRate = 60;

        donglePool = new List<Dongle>();   
        effectPool = new List<ParticleSystem>();
        for(int i = 0; i < poolSize; i++)
        {
            MakeDongle();
        }


    }
    void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }

    Dongle MakeDongle()
    {
        //����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect "+effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        //���� ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        instantDongleObj.name = "Dongle " + donglePool.Count;
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.gameManager = this;
        instantDongle.effect = instantEffect;
        donglePool.Add(instantDongle); 
        return instantDongle;
    }


    //���� ������ ������ �Լ�
    void NextDongle()
    {
        if (isOver)
        {
            return;
        }
        
        lastDongle = GetDongle();
       
        //�������� ���� ���� 0~ maxLevel�߿� �������� ����
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);
        SfxPlay(Sfx.Next);
        StartCoroutine(WaitNext());
    }
    //������ �������⸦ ��ٸ��� �ڷ�ƾ 
    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;  
        }
        //yield return null; //�� �������� ���� �ڵ�
        yield return new WaitForSeconds(2.5f); //2.5�� ���� �ڵ�
        NextDongle();
    }
   
    Dongle GetDongle()
    {
        for(int i=0; i<donglePool.Count; i++)
        {
            poolCursor = (poolCursor + 1)% donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf)
            {
                return donglePool[poolCursor]; 
            }
        }
        return MakeDongle();

    }
    public void TouchDown()
    {
        if(lastDongle == null)
        {
            return;
        }
        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null)
        {
            return;
        }
        lastDongle.Drop();
        lastDongle = null;
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }
        isOver = true;
       StartCoroutine(GameOverRountine());
        
    }

    IEnumerator GameOverRountine()
    {
        //1. ��� �ȿ� Ȱ��ȭ �Ǿ� �ִ� ��� ������ ��������
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();
        //2. ����� ���� ��� �������� ���� ȿ�� ��Ȱ��ȭ
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].rigid.simulated = false; 
            
        }
        //3. 1���� ����� �ϳ��� �����ؼ� ����� 
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].Hide(Vector3.up * 100); //���� �÷��� �߿� ���� �� ���� ū ���� �����ϰ� �����
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        SfxPlay(Sfx.Over);

    }
    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(3, 6)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case Sfx.Attach: //��� �繰�̵� �ε����� ���� �Ҹ�
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            default:
                break;
        }
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

}
