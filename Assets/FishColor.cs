using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishColor : MonoBehaviour
{
    private int m_colorNumber;
    private SpriteRenderer m_sprite;
    
    void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();
        m_colorNumber = Random.Range(1, 4);
        if(m_colorNumber == 1){
            m_sprite.color = new Color(0.13f, 0.33f, 0.18f);
        }
        else if(m_colorNumber == 2){
            m_sprite.color = new Color(0.24f, 0.54f, 0.30f);
        }
        else if(m_colorNumber == 3){
            m_sprite.color = new Color(0.28f, 0.85f, 0.48f);
        }
    }
}
