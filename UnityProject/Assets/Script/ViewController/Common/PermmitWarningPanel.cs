using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using uTools;

/// <summary>
/// Permmit warning panel.
/// </summary>
namespace ViewController {
    public class PermmitWarningPanel : SingletonMonoBehaviour<PermmitWarningPanel>
    {
         #region serialize variable
         [SerializeField]
         private GameObject _permmitWarningPanel;
         #endregion

        /// <summary>
        /// Open this instance.
        /// </summary>
        public void Open() {
            _permmitWarningPanel.GetComponent<uTweenPosition> ().from = _permmitWarningPanel.GetComponent<uTweenPosition> ().to;
            _permmitWarningPanel.GetComponent<uTweenPosition> ().to = Vector3.zero;
            _permmitWarningPanel.GetComponent<uTweenPosition> ().ResetToBeginning ();
            _permmitWarningPanel.GetComponent<uTweenPosition> ().enabled = true;
        }

        /// <summary>
        /// Close this instance.
        /// </summary>
        public void Close () {
            _permmitWarningPanel.GetComponent<uTweenPosition> ().to = _permmitWarningPanel.GetComponent<uTweenPosition> ().from;
            _permmitWarningPanel.GetComponent<uTweenPosition> ().from = Vector3.zero;
            _permmitWarningPanel.GetComponent<uTweenPosition> ().ResetToBeginning ();
            _permmitWarningPanel.GetComponent<uTweenPosition> ().enabled = true;
        }
    }
}