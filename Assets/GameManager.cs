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
    public int maxLevel;
    public int score;
    public bool isOver;

    private void Awake()
    {
        //�������� �����ϰ� �ϱ�
        Application.targetFrameRate = 60;

        //�ν������� interpolate: ���� �������� ���Ͽ� �������� �ε巴�� ����
    }
    void Start()
    {
        NextDongle();
    }
    //���� ������ ������ �Լ�
    void NextDongle()
    {
        if (isOver)
        {
            return;
        }
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        //�������� ���� ���� 0~ maxLevel�߿� �������� ����
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);
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
        //����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        //���� ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.effect = instantEffect;
        return instantDongle;

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
    }

}
