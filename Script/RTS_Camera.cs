using UnityEngine;
using System.Collections;

namespace RTS_Cam
{
    public class RTS_Camera : MonoBehaviour
    {

        private Transform m_Transform; 
        public bool useFixedUpdate = false; 

        #region Movement
        public float keyboardMovementSpeed = 5f; 
        public float screenEdgeMovementSpeed = 3f; 
        public float followingSpeed = 5f; 
        public bool isoverBound;
        #endregion
        #region Height
        public bool autoHeight = true;
        public LayerMask groundMask = -1; 
        #endregion
        #region MapLimits
        public bool limitMap = true;
        public float limitX = 50f;
        public float limitY = 50f;
        #endregion

        #region Targeting

        public Transform targetFollow; 
        public Vector3 targetOffset;
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }
        #endregion
        #region Input
        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;
        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;
        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }


        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        #endregion

        #region Unity_Methods
        private void Start()
        {
            m_Transform = transform;
            isoverBound = false;
        }
        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }
        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }
        #endregion
        #region RTSCamera_Methods
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();
            LimitPosition();
        }
        private void Move()
        {

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);
                if (!isoverBound)
                    m_Transform.Translate(desiredMove, Space.Self);
            }       
        
        }
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }
        private void LimitPosition()
        {
            if (!limitMap)
                return;

            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }
        public void ResetTarget()
        {
            targetFollow = null;
        }
        #endregion
    }
}