using UnityEngine;
using System.Collections;

namespace YounGenTech.HealthScript {

    [AddComponentMenu("YounGen Tech/Health/Effects/Destroy Event")]
    public class DestroyEvent : MonoBehaviour {

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("destroyThis")]
        GameObject _destroyThis;

        public GameObject DestroyThis {
            get { return _destroyThis; }
            set { _destroyThis = value; }
        }

        public void Destroy() {

            this.GetComponent<ChessControl>().currentPart.GetComponent<Part>().isControl = false;
            Destroy(DestroyThis);
            //GameManager.instance.initChess_list();
            //Debug.Log("check_"+GameManager.instance.getPlayerNum());
            GameManager.instance.game_state.check_victor();
            GameManager.instance.game_state.update_Townlist();
        }

        void Reset() {
            DestroyThis = gameObject;
        }
    }
}