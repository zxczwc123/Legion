using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Framework.Core.MonoBehaviourAdapter {
    public class PointerDragAdapter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        public Action<PointerEventData> onBeginDrag;

        public Action<PointerEventData> onDrag;

        public Action<PointerEventData> onEndDrag;


        public void OnBeginDrag(PointerEventData eventData) {
            if (onBeginDrag != null) onBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (onDrag != null) onDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (onEndDrag != null) onEndDrag(eventData);
        }

    }
}
