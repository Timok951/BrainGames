using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    /*<summarry>
     * Class for animations like buttons or menu <summarry>
     */

    internal class AnimationDotWeen
    {
        private Tween playStartTween;
        private Tween playNextTween;

        /*Animate button*/
        public void Animate(GameObject target, System.Action onComplete, float duration = 1f)
        {

            if (playStartTween != null && playStartTween.IsActive())
            {
                playStartTween.Kill();
            }

            playStartTween = target.transform
            .DOScale(1.1f, 0.1f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo).OnComplete(() => onComplete?.Invoke());

            playStartTween.Play();
        }

        public void AnimateAndSwitch(Button button, System.Action switchAction)
        {
            if (button != null)
            {
                Animate(button.gameObject, switchAction);
            }
            else
            {
                Debug.LogError("Button is null");
                switchAction?.Invoke();
            }
        }

    }
}
