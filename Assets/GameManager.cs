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
    private void Awake()
    {
        //프레임이 일정하게 하기
        Application.targetFrameRate = 60;

        //인스펙터의 interpolate: 이전 프레임을 비교하여 움직임을 부드럽게 보정
    }
    void Start()
    {
        NextDongle();
    }
    //다음 동글을 가져올 함수
    void NextDongle()
    {
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        //동글이의 레벨 값을 0~ maxLevel중에 랜덤으로 설정
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);
        StartCoroutine(WaitNext());
    }
    //동글이 떨어지기를 기다리는 코루틴 
    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;  
        }
        //yield return null; //한 프레임을 쉬는 코드
        yield return new WaitForSeconds(2.5f); //2.5초 쉬는 코드
        NextDongle();
    }
   
    Dongle GetDongle()
    {
        //이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        //동글 생성
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
}
