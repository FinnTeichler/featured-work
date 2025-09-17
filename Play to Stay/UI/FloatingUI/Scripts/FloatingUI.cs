using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FinnTeichler.UI
{
    public class FloatingUI : MonoBehaviour
    {
        public Image image;
        public TMP_Text text;

        public struct Data
        {
            public Data(Camera _camera, Transform _transform, Vector3 _offset, bool _active)
            {
                camera = _camera;
                target = _transform;
                offset = _offset;
                active = _active;
            }

            public Camera camera;
            public Transform target;
            public Vector3 offset;
            public bool active;
        }
        private Data data;

        private void Update()
        {
            if (data.active)
            {
                Vector3 position = data.camera.WorldToScreenPoint(data.target.position + data.offset);

                if (transform.position != position)
                    transform.position = position;
            }
        }

        private bool ActivationIsPossible()
        {
            if (!data.camera)
            {
                Debug.LogError($"{this} can not operate: data is missing a Camera component.");
                return false;
            }

            if (!data.target)
            {
                Debug.LogError($"{this} can not operate: data is missing a Target transform.");
                return false;
            }

            return true;
        }

        public void SetaData(Data data)
        {
            this.data = data;
            image.gameObject.SetActive(data.active);
        }

        public void SetTarget(Transform target)
        {
            data.target = target;
        }

        public void SetTarget(Transform target, Vector3 offset)
        {
            data.target = target;
            data.offset = offset;
        }

        public void SetOffset(Vector3 offset)
        {
            data.offset = offset;
        }

        public void SetCamera(Camera camera)
        {
            data.camera = camera;
        }

        public void SetActivation(bool active)
        {
            data.active = active;
            image.gameObject.SetActive(data.active);
        }

        public void SetReset()
        {
            data.camera = null;
            data.target = null;
            data.offset = Vector3.zero;
            enabled = false;
        }
    }
}