using UnityEngine;


public class Screenlayout : MonoBehaviour
{
    [SerializeField]
    RectTransform m_TexInput;
    [SerializeField]
    TEXInput TexInput;
    [SerializeField]
    TEXDraw m_TexDraw;

    float m_TexWidth;
    float Unit
    {
        get { return m_TexDraw.size; }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_TexWidth = m_TexInput.rect.width;
    }
    public void TextChanged()
    {
        if (m_TexDraw.preferredWidth <= m_TexWidth - Unit )
        {
            m_TexInput.offsetMin = new Vector2(0, m_TexInput.offsetMin.y);
            m_TexInput.offsetMax = new Vector2(0, m_TexInput.offsetMax.y);
        }
        else if (m_TexDraw.preferredWidth > m_TexInput.rect.width - Unit)
        {
            m_TexInput.offsetMin = new Vector2(m_TexInput.offsetMin.x - m_TexDraw.preferredWidth - Unit + m_TexInput.rect.width, m_TexInput.offsetMin.y);
        }
        else if (m_TexDraw.preferredWidth < m_TexInput.rect.width - Unit * 2 && m_TexDraw.preferredWidth > m_TexWidth + Unit)
        {
            m_TexInput.offsetMax = new Vector2(m_TexInput.offsetMax.x + m_TexDraw.preferredWidth - m_TexInput.rect.width, m_TexInput.offsetMin.y);
        }
        else { Debug.Log(0); }
    }
}