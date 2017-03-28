using UnityEngine;
using System.Collections;
using Http;
using UnityEngine.UI;

namespace ViewController
{
    /// <summary>
    /// Panel purchase.
    /// </summary>
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField]
        private Text _itemName;
        
        [SerializeField]
        private Text _description;

        [SerializeField]
        private Text _point;

        [SerializeField]
        private Text _servicePoint;

        [SerializeField]
        private Text _amount;

        /// <summary>
        /// Init the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Init (PurchaseItemListEntity.PurchaseItem item) 
        {
            this.gameObject.name = item.product_id;
            _itemName.text = item.name;
            _point.text    = item.point;
            _servicePoint.text = item.service_point;
            _amount.text = item.amount;
        }
    }
}