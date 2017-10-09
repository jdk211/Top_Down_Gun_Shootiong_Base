// 어떤 프로젝트에도 일반적으로 도움이되는 메소드를 가진다.
using System.Collections;
using System.Collections.Generic;

public static class Utility {

    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        for(int i = 0; i < array.Length - 1; i++) // 마지막 루프는 생략
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
	
}
