using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Models
{    
    public class PopupCapture : MonoBehaviour
    {
        Delegate editCellFunc;
        HexCell currentHexCell;
        public void ShowPopup(Delegate callback, HexCell cell)
        {
            editCellFunc = callback;
            currentHexCell = cell;
            gameObject.SetActive(true);
        }

        public void OnOkClick()
        {
            editCellFunc.DynamicInvoke(currentHexCell);
            gameObject.SetActive(false);
        }

        public void OnCancelClick()
        {
            gameObject.SetActive(false);
        }
    }
}
