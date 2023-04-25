using System.Collections; 


namespace UAI{ 
    public class CoroutineHelper : SingletonBehaviour<CoroutineHelper>
    {   
        public static void StartCor(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }

        public static void StopCor(IEnumerator coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }
    }
}
 