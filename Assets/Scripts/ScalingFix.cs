using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingFix : MonoBehaviour
{
    [SerializeField]
    RectTransform right;
    [SerializeField]
    RectTransform left;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform gameObject = GetComponent<RectTransform>();
        //Debug.Log($"width:{gameObject.rect.width}\nleft:{left.offsetMin.x},{left.offsetMax.x}\nright:{right.offsetMin.x},{right.offsetMax.x}");
        if (gameObject.rect.width == left.rect.width)
            left.offsetMax = new Vector2(-(gameObject.rect.width - right.offsetMin.x), left.offsetMax.y);
        else if (gameObject.rect.width == right.rect.width)
            right.offsetMin = new Vector2(gameObject.rect.width - left.offsetMax.x, right.offsetMin.y);
    }
}
