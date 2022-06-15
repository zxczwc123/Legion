using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [RequireComponent(typeof(MeshRenderer))]
    public class TowerColorSetter : MonoBehaviour
    {
        private MeshRenderer m_MeshRenderer;

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (m_MeshMaterial == null)
                {
                    m_MeshRenderer = GetComponent<MeshRenderer>();
                    m_MeshMaterial = m_MeshRenderer.sharedMaterial;
                }
                return m_MeshRenderer;
            }
        }

        private Material m_MeshMaterial;
        private Material MeshMaterial
        {
            get
            {
                if (m_MeshMaterial == null)
                {
                    m_MeshRenderer = GetComponent<MeshRenderer>();
                    m_MeshMaterial = m_MeshRenderer.material;
                }
                return m_MeshMaterial;
            }
        }

        private LegionType m_color;

        private void Start()
        {
            UpdateMaterial();
        }

        public void SetColor(LegionType state)
        {
            if (m_color == state)
            {
                return;
            }
            m_color = state;
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            var legionConfig = Resources.Load<LegionConfig>("LegionConfig");
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                MeshMaterial.SetTexture("_TextureSample1", legionConfig.ColorTextures[(int)m_color]);
            }
            else
            {
                MeshRenderer.material = legionConfig.ColorMaterials[(int)m_color];
            }
#else 
            MeshMaterial.SetTexture("_TextureSample1", legionConfig.ColorTextures[(int)m_color]);
#endif
        }
    }
}