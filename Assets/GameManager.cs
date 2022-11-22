using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public int maxLevel;
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
    //���� ���� �Լ�
    Dongle GetDongle()
    {
        GameObject instant = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instant.GetComponent<Dongle>();
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
}
