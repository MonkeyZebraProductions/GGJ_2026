/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

#nullable enable

namespace Yarn.Unity.Samples
{
    public class TimeoutBar : MonoBehaviour
    {
        [SerializeField] RectTransform? bar;
        Image barImage;
        private bool _barRed;

        private float originalSize = 0f;
        public void Start()
        {
            if (bar != null)
            {
                originalSize = bar.sizeDelta.x;
            }
            barImage = GetComponent<Image>();
        }

        public async YarnTask Shrink(float duration, CancellationToken cancellationToken)
        {
            if (bar == null)
            {
                return;
            }

            float accumulator = 0;
            var currentSize = bar.sizeDelta.x;

            while (accumulator < duration && !cancellationToken.IsCancellationRequested)
            {
                accumulator += Time.deltaTime;
                var newSize = Mathf.Lerp(currentSize, 0, accumulator / duration);
                bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize);
                if(accumulator > duration*0.6 && !_barRed)
                {
                    barImage.color = new Color(0.5f,0.09f,0.04f,0.9f);
                }
                await YarnTask.Yield();
            }
            bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        }
        public void ResetBar()
        {
            if (bar != null)
            {
                bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize);
            }
        }
    }
}
