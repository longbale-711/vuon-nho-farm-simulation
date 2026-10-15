using System.Collections;
using Orchard.Currency;
using UnityEngine;

namespace Orchard.World
{
    public sealed class FarmEffectsPresenter : MonoBehaviour
    {
        private WorldTextFactory _labels;
        public void Initialize(WorldTextFactory labels)
        {
            _labels = labels;
        }

        public void Effect(GameObject prefab, Vector3 point)
        {
            if (!prefab)
            {
                return;
            }

            var instance = Instantiate(prefab, point, Quaternion.identity);
            Destroy(instance, 3);
        }

        public void ShowPayment(Vector3 position, Money payment) => StartCoroutine(FloatPayment(position, payment));
        private IEnumerator FloatPayment(Vector3 position, Money payment)
        {
            var label = _labels.Create(transform, position, "+" + payment.Display() + " $", 4, 1, 5);
            label.color = FarmVisual.Forest;
            float time = 0;
            while (time < 1.3f)
            {
                time += Time.deltaTime;
                label.transform.position = position + Vector3.up * time;
                label.alpha = 1 - time / 1.3f;
                yield return null;
            }

            Destroy(label.gameObject);
        }
    }
}
