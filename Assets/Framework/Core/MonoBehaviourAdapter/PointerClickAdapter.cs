using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core.MonoBehaviourAdapter {
    public class PointerClickAdapter : MonoBehaviour, IPointerClickHandler {


        public Action<PointerEventData> onPointerClick;

        public void OnPointerClick(PointerEventData eventData) {
            if (onPointerClick != null) onPointerClick(eventData);
        }
    }

    public class PointerDownUpAdapter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Action<PointerEventData> onPointerUp;

        public Action<PointerEventData> onPointerDown;

        public void OnPointerDown(PointerEventData eventData) {
            if (onPointerDown != null) onPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (onPointerUp != null) onPointerUp(eventData);
        }
    }

    public class PointerEnterExitAdapter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public Action<PointerEventData> onPointerEnter;

        public Action<PointerEventData> onPointerExit;

        public void OnPointerEnter(PointerEventData eventData) {
            if (onPointerEnter != null) onPointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (onPointerExit != null) onPointerExit(eventData);
        }

    }
}
