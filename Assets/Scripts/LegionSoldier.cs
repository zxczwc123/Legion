using UnityEngine;

public class LegionSoldier : MonoBehaviour
{
    private int m_count;
    public int Count
    {
        set
        {
            m_count = value;
            Text.text = m_count.ToString();
        }
        get
        {
            return m_count;
        }
    }

    public int m_legion;

    public int Legion
    {
        get
        {
            return m_legion;
        }
        set
        {
            m_legion = value;
            Sprite.color = LegionUtil.GetColor(m_legion);
        }
    }
    
    private TextMesh m_text;
    protected TextMesh Text
    {
        get
        {
            if (m_text == null)
            {
                m_text = transform.Find("Text").GetComponent<TextMesh>();
            }
            return m_text;
        }
    }
    
    private SpriteRenderer m_sprite;
    protected SpriteRenderer Sprite
    {
        get
        {
            if (m_sprite == null)
            {
                m_sprite = transform.GetComponent<SpriteRenderer>();
            }
            return m_sprite;
        }
    }

    private LegionTower m_target;

    private float m_speed = 3f;

    public void SetTarget(LegionTower tower)
    {
        m_target = tower;
    }

    private void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (m_target == null)
        {
            return;
        }
        transform.position += (m_target.transform.position - transform.position).normalized * m_speed * Time.deltaTime;
        if (Vector2.Distance(transform.position, m_target.transform.position) < 0.1)
        {
            OnMoveComplete();
        }
    }

    private void OnMoveComplete()
    {
        if (m_target.Legion != Legion)
        {
            if (m_target.Count < Count)
            {
                m_target.Legion = Legion;
                m_target.Count = Count - m_target.Count;
            }
            else
            {
                m_target.Count -= Count;
            }
        }
        else
        {
            m_target.Count += Count;
        }
        gameObject.SetActive(false);
        LegionSoldierManager.Instance.Release(this);
    }
}