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
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, Over};
    int sfxCursor;
    private void Awake()
    {
        //프레임이 일정하게 하기
        Application.targetFrameRate = 60;

        //인스펙터의 interpolate: 이전 프레임을 비교하여 움직임을 부드럽게 보정
    }
    void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }
    //다음 동글을 가져올 함수
    void NextDongle()
    {
        if (isOver)
        {
            return;
        }
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        //동글이의 레벨 값을 0~ maxLevel중에 랜덤으로 설정
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);
        SfxPlay(Sfx.Next);
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
        //1. 장면 안에 활성화 되어 있는 모든 동글이 가져오기
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();
        //2. 지우기 전에 모든 동글이의 물리 효과 비활성화
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].rigid.simulated = false; 
            
        }
        //3. 1번의 목록을 하나씩 접근해서 지우기 
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].Hide(Vector3.up * 100); //게임 플레이 중에 나올 수 없는 큰 값을 전달하고 숨기기
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
            case Sfx.Attach: //어느 사물이든 부딪힐때 나는 소리
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            default:
                break;
        }
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

}
