using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [RequireComponent(typeof(MeshRenderer))]
    public class TowerColorSetter : MonoBehaviour
    {
        private MeshRenderer m_meshRenderer;

        private Material m_meshMaterial;

        private LegionType m_color;

        private bool m_isStart;

        private void Start()
        {
            m_isStart = true;
            m_meshRenderer = GetComponent<MeshRenderer>();
            m_meshMaterial = m_meshRenderer.material;
            UpdateMaterial();
        }

        public void SetColor(LegionType state)
        {
            if (m_color == state)
            {
                return;
            }
            m_color = state;
            if (!m_isStart)
            {
                return;
            }
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (LegionGame.instance)
            {
                m_meshMaterial.SetTexture("_TextureSample1", LegionGame.instance.ColorTextures[(int)m_color]);
            }
        }
    }
}