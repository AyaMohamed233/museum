using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// كمبوننت للتحكم في الـ Border Radius من الـ Inspector.
/// يعمل كـ IMeshModifier بجانب الـ Image الموجود.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Image))]
[DisallowMultipleComponent]
public class RoundedImage : MonoBehaviour, IMeshModifier
{
    [Header("Corner Radius")]
    [Range(0, 128)]
    public float cornerRadius = 16f;

    [Header("Fill")]
    public Color fillColor = Color.white;
    public Color borderColor = new Color(1f, 1f, 1f, 0f);

    [Range(0, 8)]
    public float borderThickness = 0f;

    [Range(1, 20)]
    public int cornerSegments = 8;

    private Graphic _graphic;

    private Graphic CachedGraphic
    {
        get
        {
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            return _graphic;
        }
    }

    void OnEnable()
    {
        if (CachedGraphic != null)
        {
            CachedGraphic.SetVerticesDirty();
        }
    }

    void OnDisable()
    {
        if (CachedGraphic != null)
        {
            CachedGraphic.SetVerticesDirty();
        }
    }

    void OnValidate()
    {
        if (CachedGraphic != null)
            CachedGraphic.SetVerticesDirty();
    }

    public void ModifyMesh(Mesh mesh)
    {
        using (VertexHelper vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }

    public void ModifyMesh(VertexHelper vh)
    {
        if (!isActiveAndEnabled) return;

        Graphic g = CachedGraphic;
        if (g == null) return;

        Rect r = g.rectTransform.rect;
        if (r.width <= 0 || r.height <= 0) return;

        // مسح الشكل الافتراضي وإعادة رسمه
        vh.Clear();

        float radius = Mathf.Min(cornerRadius, Mathf.Min(r.width / 2f, r.height / 2f));
        float border = Mathf.Min(borderThickness, Mathf.Min(r.width / 2f, r.height / 2f));

        // Multiply by the graphic's current color to support Button ColorTint transitions!
        Color finalFillColor = fillColor * g.color;
        Color finalBorderColor = borderColor * g.color;

        if (radius <= 0f)
        {
            AddQuad(vh, r, finalFillColor);
            if (border > 0)
                AddBorderQuads(vh, r, border, finalBorderColor);
            return;
        }

        GenerateRoundedRect(vh, r, radius, finalFillColor);

        if (border > 0f)
            GenerateRoundedBorder(vh, r, radius, border, finalBorderColor);
    }

    public void Refresh()
    {
        if (CachedGraphic != null) CachedGraphic.SetVerticesDirty();
    }

    // =========================================================
    // GEOMETRY
    // =========================================================

    private void AddVert(VertexHelper vh, Rect r, float x, float y, Color c)
    {
        UIVertex v = UIVertex.simpleVert;
        v.color = c;
        v.position = new Vector3(x, y, 0);
        // UV مضبوط — يمتد من 0 إلى 1 حسب موقع النقطة في المستطيل
        v.uv0 = new Vector4(
            (x - r.xMin) / r.width,
            (y - r.yMin) / r.height,
            0, 0
        );
        vh.AddVert(v);
    }

    private void AddQuad(VertexHelper vh, Rect r, Color c)
    {
        int s = vh.currentVertCount;
        AddVert(vh, r, r.xMin, r.yMin, c); // 0 BL
        AddVert(vh, r, r.xMin, r.yMax, c); // 1 TL
        AddVert(vh, r, r.xMax, r.yMax, c); // 2 TR
        AddVert(vh, r, r.xMax, r.yMin, c); // 3 BR

        vh.AddTriangle(s, s + 1, s + 2);
        vh.AddTriangle(s + 2, s + 3, s);
    }

    private void AddBorderQuads(VertexHelper vh, Rect r, float b, Color c)
    {
        AddQuad(vh, r, new Rect(r.xMin, r.yMin, r.width, b), c);
        AddQuad(vh, r, new Rect(r.xMin, r.yMax - b, r.width, b), c);
        AddQuad(vh, r, new Rect(r.xMin, r.yMin + b, b, r.height - 2 * b), c);
        AddQuad(vh, r, new Rect(r.xMax - b, r.yMin + b, b, r.height - 2 * b), c);
    }

    // Overload: AddQuad with a sub-rect but UVs relative to parent rect
    private void AddQuad(VertexHelper vh, Rect parentRect, Rect subRect, Color c)
    {
        int s = vh.currentVertCount;
        AddVert(vh, parentRect, subRect.xMin, subRect.yMin, c);
        AddVert(vh, parentRect, subRect.xMin, subRect.yMax, c);
        AddVert(vh, parentRect, subRect.xMax, subRect.yMax, c);
        AddVert(vh, parentRect, subRect.xMax, subRect.yMin, c);

        vh.AddTriangle(s, s + 1, s + 2);
        vh.AddTriangle(s + 2, s + 3, s);
    }

    private void GenerateRoundedRect(VertexHelper vh, Rect r, float radius, Color c)
    {
        Vector2 cBL = new Vector2(r.xMin + radius, r.yMin + radius);
        Vector2 cBR = new Vector2(r.xMax - radius, r.yMin + radius);
        Vector2 cTR = new Vector2(r.xMax - radius, r.yMax - radius);
        Vector2 cTL = new Vector2(r.xMin + radius, r.yMax - radius);

        // Center vertex
        int centerIdx = vh.currentVertCount;
        AddVert(vh, r, r.center.x, r.center.y, c);

        int perimStart = vh.currentVertCount;

        // CCW perimeter: BR→TR→TL→BL
        AddCornerVerts(vh, r, cBR, radius, 270f, 360f, c);
        AddCornerVerts(vh, r, cTR, radius, 0f, 90f, c);
        AddCornerVerts(vh, r, cTL, radius, 90f, 180f, c);
        AddCornerVerts(vh, r, cBL, radius, 180f, 270f, c);

        int perimCount = vh.currentVertCount - perimStart;

        for (int i = 0; i < perimCount; i++)
        {
            int cur = perimStart + i;
            int nxt = perimStart + ((i + 1) % perimCount);
            vh.AddTriangle(centerIdx, cur, nxt);
        }
    }

    private void GenerateRoundedBorder(VertexHelper vh, Rect r, float radius, float border, Color c)
    {
        float innerR = Mathf.Max(0f, radius - border);

        Vector2 cBL = new Vector2(r.xMin + radius, r.yMin + radius);
        Vector2 cBR = new Vector2(r.xMax - radius, r.yMin + radius);
        Vector2 cTR = new Vector2(r.xMax - radius, r.yMax - radius);
        Vector2 cTL = new Vector2(r.xMin + radius, r.yMax - radius);

        int outerStart = vh.currentVertCount;
        AddCornerVerts(vh, r, cBR, radius, 270f, 360f, c);
        AddCornerVerts(vh, r, cTR, radius, 0f, 90f, c);
        AddCornerVerts(vh, r, cTL, radius, 90f, 180f, c);
        AddCornerVerts(vh, r, cBL, radius, 180f, 270f, c);
        int outerCount = vh.currentVertCount - outerStart;

        int innerStart = vh.currentVertCount;
        AddCornerVerts(vh, r, cBR, innerR, 270f, 360f, c);
        AddCornerVerts(vh, r, cTR, innerR, 0f, 90f, c);
        AddCornerVerts(vh, r, cTL, innerR, 90f, 180f, c);
        AddCornerVerts(vh, r, cBL, innerR, 180f, 270f, c);

        for (int i = 0; i < outerCount; i++)
        {
            int nxt = (i + 1) % outerCount;
            vh.AddTriangle(outerStart + i, outerStart + nxt, innerStart + i);
            vh.AddTriangle(innerStart + i, outerStart + nxt, innerStart + nxt);
        }
    }

    private void AddCornerVerts(VertexHelper vh, Rect r, Vector2 center, float radius, float startAngle, float endAngle, Color c)
    {
        float step = (endAngle - startAngle) / cornerSegments;
        for (int i = 0; i <= cornerSegments; i++)
        {
            float angle = (startAngle + i * step) * Mathf.Deg2Rad;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;
            AddVert(vh, r, x, y, c);
        }
    }
}
