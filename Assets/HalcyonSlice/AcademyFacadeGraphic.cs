using System;
using UnityEngine;
using UnityEngine.UI;

namespace Halcyon.FirstWeather
{
    // The source is RGB. Precomputed sprite geometry omits its painted checkerboard.
    // This preserves the source bitmap and lets independent scenery show around the facade.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class AcademyFacadeGraphic : MaskableGraphic
    {
        [Serializable] sealed class MeshRows { public int width, height; public int[] spans; }
        static MeshRows rows;
        public Texture2D texture;
        public override Texture mainTexture => texture != null ? texture : Texture2D.whiteTexture;
        protected override void Awake() { base.Awake(); useLegacyMeshGeneration = false; }
        static MeshRows Geometry()
        {
            if (rows != null) return rows;
            var source = Resources.Load<TextAsset>("HalcyonAcademy/AcademyFacadeMesh");
            if (source != null) rows = JsonUtility.FromJson<MeshRows>(source.text);
            return rows;
        }
        public static bool MeshIsComplete
        {
            get
            {
                var data = Geometry();
                if (data == null || data.width != 1536 || data.height != 1024 || data.spans == null || data.spans.Length < 1500 || data.spans.Length % 3 != 0) return false;
                for (int i = 0; i < data.spans.Length; i += 3)
                    if (data.spans[i] < 0 || data.spans[i] >= data.height || data.spans[i + 1] < 0 || data.spans[i + 2] > data.width || data.spans[i + 1] >= data.spans[i + 2]) return false;
                return data.spans.Length / 3 * 4 < 65000;
            }
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); var data = Geometry(); if (data == null) return;
            var r = rectTransform.rect;
            for (int i = 0; i < data.spans.Length; i += 3)
            {
                float top = data.spans[i] / (float)data.height, bottom = (data.spans[i] + 1f) / data.height;
                float left = data.spans[i + 1] / (float)data.width, right = data.spans[i + 2] / (float)data.width;
                int at = vh.currentVertCount;
                vh.AddVert(new Vector3(r.xMin + left * r.width, r.yMax - top * r.height), color, new Vector2(left, 1 - top));
                vh.AddVert(new Vector3(r.xMin + right * r.width, r.yMax - top * r.height), color, new Vector2(right, 1 - top));
                vh.AddVert(new Vector3(r.xMin + right * r.width, r.yMax - bottom * r.height), color, new Vector2(right, 1 - bottom));
                vh.AddVert(new Vector3(r.xMin + left * r.width, r.yMax - bottom * r.height), color, new Vector2(left, 1 - bottom));
                vh.AddTriangle(at, at + 1, at + 2); vh.AddTriangle(at, at + 2, at + 3);
            }
        }
    }
}
