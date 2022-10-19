using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoralisUnity.Samples.Shared.Helpers;
using UnityEngine;
    
 #pragma warning disable CS0618
namespace MoralisUnity.Samples.Shared.Data.Types.Storage
{
    [CreateAssetMenu( menuName = SharedConstants.PathMoralisSharedCreateAssetMenu + Title, 
        fileName = Title, order = SharedConstants.PriorityMoralisTools_Primary)]
    public class SceneTransition: ScriptableObject
    {
        //  Properties ------------------------------------
    
        //  Fields ----------------------------------------
        private const string Title = "SceneTransition";

        [Header("Before")]
        [SerializeField]
        private float _delayBeforeSeconds = 0;

        [SerializeField] 
        private Ease _easeIn = Ease.Linear;

        [Header("During")]
        [SerializeField]
        private float _delayMidpointSeconds = 0;
        
        [SerializeField]
        private float _durationSeconds = 0.5f;

        [Header("After")]
        [SerializeField]
        private float _delayAfterSeconds = 0;

        [SerializeField] 
        private Ease _easeOut = Ease.Linear;

  
        //  Methods ---------------------------------------
        public async UniTask ApplyTransition(ImageAndCanvasView imageAndCanvasView, Action action)
        {
            //Half in / half out
            float halfDuration = _durationSeconds / 2;
            
            // BEFORE
            imageAndCanvasView.BlocksRaycasts = true;
            await TweenHelper.AlphaDoFade(imageAndCanvasView, 0, 1, 
                halfDuration,
                _delayBeforeSeconds,
                _easeIn);

            await UniTask.WaitForEndOfFrame(); 

            
            // DURING
            action.Invoke();
    
            // AFTER
            await TweenHelper.AlphaDoFade(imageAndCanvasView, 1, 0, 
                halfDuration,
                _delayMidpointSeconds,
                _easeOut);
            await UniTask.WaitForEndOfFrame();
            await UniTask.Delay((int)(_delayAfterSeconds*1000));
            imageAndCanvasView.BlocksRaycasts = false;
        }
    }
}