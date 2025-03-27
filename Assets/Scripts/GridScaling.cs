using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScaling : MonoBehaviour
{
    [SerializeField]
    RectTransform Width;
    [SerializeField]
    GridLayoutGroup GridGroup;
    [SerializeField]
    float width;

    private void Start()
    {
        Width = GetComponent<RectTransform>();
        GridGroup = GetComponent<GridLayoutGroup>();
        width = GridGroup.cellSize.x * GridGroup.constraintCount + GridGroup.spacing.x * GridGroup.constraintCount;
        if (width >= Width.rect.width)
        {
            float ratio = Width.rect.width / width;
            GridGroup.cellSize = new Vector2(GridGroup.cellSize.x * ratio, GridGroup.cellSize.y);
            GridGroup.spacing = new Vector2(GridGroup.spacing.x * ratio, GridGroup.spacing.y);
        }
    }
}