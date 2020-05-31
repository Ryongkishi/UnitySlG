using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YounGenTech.HealthScript {
    [AddComponentMenu("YounGen Tech/Health/Other/Attack")]
    public class Attack : MonoBehaviour {

        [SerializeField]
        private LayerMask _raycastLayer;

        [SerializeField]
        private float _damage = 1;

        public UnityEvent OnAttack;
        public UnityEvent OnEndAttack;

        #region Properties

        public float Damage {
            get { return _damage; }
            set { _damage = value; }
        }

        public LayerMask RaycastLayer {
            get { return _raycastLayer; }
            set { _raycastLayer = value; }
        }
        #endregion

        public void OnMouseUp() {
            StartAttack();
        }


        public void StartAttack() {
            if(enabled)
                if(OnAttack != null) OnAttack.Invoke();
        }

        public void EndAttack() {
            RaycastDamage(Damage);
            if(OnEndAttack != null) OnEndAttack.Invoke();
        }

 
        public void RaycastDamage(float damageAmount) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * .5f, transform.right * Mathf.Sign(transform.localScale.x), 2, RaycastLayer);

            Debug.DrawRay(transform.position, transform.right, Color.red, 5);

            if(hit.collider) {
                Health health = hit.collider.GetComponentInParent<Health>();

                if(health)
                    health.Damage(new HealthEvent(gameObject, damageAmount));
            }
        }
    }
}