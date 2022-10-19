using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class CanvasGroupExtensionsTest
    {
        //  Properties ------------------------------------
        
        
        //  Fields ----------------------------------------

        
        //  Unity Methods----------------------------------
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Executes BEFORE ALL test methods of this test class
        }
        
        [SetUp]
        public void Setup()
        {
            // Executes BEFORE EACH test methods of this test class
        }
        
        [TearDown]
        public void TearDown()
        {
            // Executes AFTER EACH test methods of this test class
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Executes AFTER ALL test methods of this test class
        }

        
        //  General Methods -------------------------------
        [UnityTest]
        public IEnumerator SetIsVisible_AlphaIs0_WhenSetIsVisibleIsFalse()
        {
            // Arrange
            GameObject go = new GameObject();
            CanvasGroup canvasGroup = go.AddComponent<CanvasGroup>();
            yield return null; //Optional, For Demo Sake: Wait a frame
            
            // Act
            canvasGroup.SetIsVisible(false);
            float alpha = canvasGroup.alpha;
                
            // Assert
            Assert.That(alpha, Is.EqualTo(0));
        }
        
        
        [UnityTest]
        public IEnumerator SetIsVisible_AlphaIs1_WhenSetIsVisibleIsTrue()
        {
            // Arrange
            GameObject go = new GameObject();
            CanvasGroup canvasGroup = go.AddComponent<CanvasGroup>();
            yield return null; //Optional, For Demo Sake: Wait a frame
            
            // Act
            canvasGroup.SetIsVisible(true);
            float alpha = canvasGroup.alpha;
                
            // Assert
            Assert.That(alpha, Is.EqualTo(1));
        }
        
        
        //  Event Handlers --------------------------------
    }
}
